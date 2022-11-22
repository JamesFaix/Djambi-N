﻿using System.Collections.Generic;
using System.Linq;
using Djambi.Engine.Extensions;
using Djambi.Model;

namespace Djambi.Engine.Services
{
    class GameUpdateService
    {
        private readonly ValidationService _validationService;

        public GameUpdateService(ValidationService validationService)
        {
            _validationService = validationService;
        }

        public Result<GameState> UpdateGameState(GameState game, TurnState turn)
        {
            return _validationService.ValidateTurnState(game, turn)
                .Bind(_ =>
                {
                    var newGameState = UpdateGameState(game, turn, false);
                    return _validationService.ValidateGameState(newGameState)
                        .Map(__ => newGameState);
                });           
        }

        public GameState PreviewGameUpdate(GameState game, TurnState turn) =>
            UpdateGameState(game, turn, true);

        private GameState UpdateGameState(GameState game, TurnState turn, bool isPreview)
        {
            /*
             * Preview is used by PieceStrategies to see what the board would look like
             * if the first few selections of a turn were applied to the game state,
             * so additional selections can be made from that intermediate state.
             */

            var pieces    = game.Pieces.ToDictionary(p => p.Id, p => p);
            var players   = game.Players.ToDictionary(p => p.Id, p => p);
            var turnCycle = game.TurnCycle.ToList();
            var log       = game.Log.ToList();

            var s            = turn.Selections;
            var origin       = s[0].Location;
            var destination  = s[1].Location;
            var subject      = game.PiecesIndexedByLocation[origin];
            var target       = default(Piece);
            var subjectOwner = players[game.TurnCycle[0]];

            //Move subject
            pieces[subject.Id] = subject.Move(destination);
            log.Add($"{subjectOwner.Name} moved {subject.Type} {subject.Id} to {destination}.");

            //Kill and/or move target
            if (s[1].Type == SelectionType.MoveWithTarget)
            {
                target = game.PiecesIndexedByLocation[destination];

                switch (subject.Type)
                {
                    case PieceType.Assassin:
                        pieces[target.Id] = target.Kill().Move(origin);
                        log.Add($"{subject.Type} {subject.Id} killed {target.Type} {target.Id} and moved its corpse to {origin}.");

                        if (s[1].Location.IsSeat()
                         && (!isPreview || s.Count > 2)) //s[2] might not exist if in preview
                        { 
                            pieces[subject.Id] = subject.Move(s[2].Location);
                            log.Add($"{subject.Type} {subject.Id} fled the Seat to {s[2].Location}.");
                        }
                        break;

                    case PieceType.Chief:
                    case PieceType.Thug:
                        if (!isPreview || s.Count > 2) //s[2] might not exist if in preview
                        {
                            pieces[target.Id] = target.Kill().Move(s[2].Location);
                            log.Add($"{subject.Type} {subject.Id} killed {target.Type} {target.Id} and moved its corpse to {s[2].Location}.");
                        }
                        else
                        { 
                            //Temporarily put at (0, 0) for preview
                            pieces[target.Id] = target.Kill().Move(Location.Default);
                        }
                        break;

                    case PieceType.Diplomat:
                    case PieceType.Undertaker:
                        if (!isPreview || s.Count > 2) //s[2] might not exist if in preview
                        {
                            pieces[target.Id] = target.Move(s[2].Location);
                            log.Add($"{subject.Type} {subject.Id} moved {(subject.Type == PieceType.Diplomat ? target.Type.ToString() : "corpse")} {target.Id} to {s[2].Location}.");
                            if (s[1].Location.IsSeat()
                             && (!isPreview || s.Count > 3)) //s[3] might not exist if in preview
                            {
                                pieces[subject.Id] = subject.Move(s[3].Location);
                                log.Add($"{subject.Type} {subject.Id} fled the Seat to {s[2].Location}.");
                            }
                        }
                        else
                        {
                            //Temporarily put at (0, 0) for preview
                            pieces[target.Id] = target.Kill().Move(Location.Default);
                        }
                        break;
                }
            }

            if (subject.Type == PieceType.Journalist
             && (!isPreview && s.Count > 2) //s[2] might not exist if in preview or if no target
             && s[2].Type == SelectionType.Target)
            {
                target = game.PiecesIndexedByLocation[s[2].Location];
                pieces[target.Id] = target.Kill();
                log.Add($"{subject.Type} {subject.Id} killed {target.Type} {target.Id}.");
            }

            //Capture pieces if Chief killed
            if (target?.Type == PieceType.Chief
             && subject.Type != PieceType.Diplomat
             && subject.Type != PieceType.Undertaker)
            {
                //Safe to assume PlayerId.HasValue because a Chief cannot be abandoned
                var targetOwner = players[target.PlayerId.Value];

                players[targetOwner.Id] = targetOwner.Kill();

                var capturedPieces = pieces.Values
                    .Where(p => p.PlayerId == targetOwner.Id)
                    .ToList();

                foreach (var p in capturedPieces)
                {
                    pieces[p.Id] = p.Capture(subjectOwner.Id);
                }

                log.Add($"{subjectOwner.Name} eliminated {targetOwner.Name}.");
                
                if (capturedPieces.Any())
                {
                    log.Add($"{subjectOwner.Name} enlisted {ListPiecesForLog(capturedPieces)}.");
                }
            }

            //Update turn cycle if Chief enters or leaves the Seat
            if (destination.IsSeat() || origin.IsSeat())
            {
                //If removing Chief from Seat
                if (target?.Type == PieceType.Chief
                 && destination.IsSeat())
                {
                    var targetOwnerTurnIndexes = turnCycle
                        .Select((playerId, index) => new
                        {
                            PlayerId = playerId,
                            Index = index
                        })
                        .Where(x => x.PlayerId == target.PlayerId)
                        .Select(x => x.Index)
                        .Reverse() //Must work down turn cycle backwards to preserve indexes
                        .ToList();

                    if (subject.Type == PieceType.Diplomat)
                    {
                        //If moving Chief with Diplomat, remove all but last turn in cycle for owner                                
                        foreach (var t in targetOwnerTurnIndexes.Skip(1))
                        {
                            turnCycle.RemoveAt(t);
                        }
                    }
                    else
                    {
                        //If killing Chief, remove all turns for its owner
                        foreach (var t in targetOwnerTurnIndexes)
                        {
                            turnCycle.RemoveAt(t);
                        }
                    }
                    log.Add($"{players[target.PlayerId.Value].Name} was ejected from the seat of power.");
                }

                //If Chief voluntarily left Seat
                if (origin.IsSeat())
                {
                    var subjectOwnerTurnIndexes = turnCycle
                        .Select((playerId, index) => new
                        {
                            PlayerId = playerId,
                            Index = index
                        })
                        .Where(x => x.PlayerId == subject.PlayerId)
                        .Select(x => x.Index)
                        .Reverse() //Must work down turn cycle backwards to preserve indexes
                        .ToList();

                    foreach (var t in subjectOwnerTurnIndexes.Skip(1))
                    {
                        turnCycle.RemoveAt(t);
                    }
                    log.Add($"{subjectOwner.Name} ceded the seat of power.");
                }

                //If moving Chief to Seat
                if (subject.Type == PieceType.Chief
                 && destination.IsSeat())
                {
                    //Insert turn for current player between each existing turn in cycle, except between 0 and 1
                    switch (turnCycle.Distinct().Count())
                    {
                        //No interleaved turns if only 2 players left

                        case 3:
                            turnCycle.Insert(2, subjectOwner.Id);
                            break;

                        case 4:
                            turnCycle.Insert(2, subjectOwner.Id);
                            turnCycle.Insert(4, subjectOwner.Id);
                            break;
                    }
                    log.Add($"{subjectOwner.Name} acquired the seat of power.");


                    //Enlist abandoned pieces
                    var abandonedPieces = pieces.Values
                        .Where(p => p.PlayerId == null)
                        .ToList();

                    if (abandonedPieces.Any())
                    {
                        foreach (var p in abandonedPieces)
                        {
                            pieces[p.Id] = p.Capture(subjectOwner.Id);
                        }
                        log.Add($"{subjectOwner.Name} enlisted {ListPiecesForLog(abandonedPieces)}.");
                    }
                }
            }

            //Check for surrounded Chiefs
            var chiefs = pieces.Values
                .Where(p => p.Type == PieceType.Chief && p.IsAlive)
                .ToList();

            foreach (var chief in chiefs)
            {
                var neighbors = chief.Location
                    .AdjacentLocations()
                    .LeftOuterJoin(pieces.Values, 
                        loc => loc, 
                        p => p.Location, 
                        (l, p) => p, 
                        l => null);

                if (neighbors.All(p => p?.IsAlive == false))
                {
                    var deadPlayer = players[chief.PlayerId.Value];

                    var abandonedPieces = pieces.Values
                        .Where(p => p.PlayerId == deadPlayer.Id)
                        .ToList();

                    var chiefInPower = pieces.Values
                            .SingleOrDefault(p1 => p1.Type == PieceType.Chief
                                && p1.IsAlive            //Don't let a dead chief enlist abanonded pieces
                                && p1.Location.IsSeat()
                                && p1.PlayerId != null); //Don't let a neutral chief enlist abandoned pieces

                    foreach (var p in abandonedPieces)
                    {
                        if (chiefInPower == null)
                        {
                            pieces[p.Id] = p.Abandon();
                        }
                        else
                        {
                            pieces[p.Id] = p.Capture(chiefInPower.PlayerId.Value);
                        }
                    }
                    pieces[chief.Id] = chief.Kill();
                    players[deadPlayer.Id] = deadPlayer.Kill();
                    
                    turnCycle = turnCycle
                        .Where(t => t != deadPlayer.Id)
                        .ToList();

                    log.Add($"{chief.Type} {chief.Id} was surrounded by corpses and died.");
                    log.Add($"{deadPlayer.Name} was eliminated.");

                    if (abandonedPieces.Any())
                    {
                        if (chiefInPower == null)
                        {
                            log.Add($"{ListPiecesForLog(abandonedPieces)} were abandoned.");
                        }
                        else
                        {
                            log.Add($"{players[chiefInPower.PlayerId.Value].Name} enlisted {ListPiecesForLog(abandonedPieces)}.");
                        }
                    }
                }
            }

            //TODO: Check if next player has 0 possible moves

            //TODO: Check for stalemate

            //Advance the turn cycle
            turnCycle.RemoveAt(0);
            turnCycle.Add(subjectOwner.Id);

            return GameState.Create(
                players.Values,
                pieces.Values,
                turnCycle,
                log);
        }

        private string ListPiecesForLog(List<Piece> pieces)
        {
            var strList = pieces
                .Take(pieces.Count - 1)
                .Select(p => $"{p.Type} {p.Id}, ")
                .ToList();

            var last = pieces.Last();
            strList.Add($"and {last.Type} {last.Id}");

            return string.Join("", strList);
        }
    }
}
