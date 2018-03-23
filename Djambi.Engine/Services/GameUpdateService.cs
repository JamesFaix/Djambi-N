using System;
using System.Collections.Generic;
using System.Linq;
using System.Text;
using Djambi.Engine.Extensions;
using Djambi.Model;

namespace Djambi.Engine.Services
{
    class GameUpdateService
    {
        private readonly ValidationService _validationService = new ValidationService();

        public Result<GameState> UpdateGameState(GameState game, TurnState turn)
        {
            return _validationService.ValidateTurnState(game, turn)
                .Bind(_ =>
                {
                    var pieces    = game.Pieces.ToDictionary(p => p.Id, p => p);
                    var players   = game.Players.ToDictionary(p => p.Id, p => p);
                    var turnCycle = game.TurnCycle.ToList();

                    var s            = turn.Selections;
                    var origin       = s[0].Location;
                    var destination  = s[1].Location;
                    var subject      = game.PiecesIndexedByLocation[origin];
                    var target       = default(Piece);
                    var subjectOwner = players[game.TurnCycle[0]];

                    //Move subject
                    pieces[subject.Id] = subject.Move(destination);

                    //Kill and/or move target
                    if (s[1].Type == SelectionType.MoveDestinationWithTarget)
                    {
                        target = game.PiecesIndexedByLocation[destination];

                        switch (subject.Type)
                        {
                            case PieceType.Assassin:
                                pieces[target.Id] = target.Kill().Move(origin);
                                if (s[1].Location.IsMaze())
                                {
                                    pieces[subject.Id] = subject.Move(s[2].Location);
                                }
                                break;

                            case PieceType.Chief:
                            case PieceType.Militant:
                                pieces[target.Id] = target.Kill().Move(s[2].Location);
                                break;

                            case PieceType.Diplomat:
                            case PieceType.Necromobile:
                                pieces[target.Id] = target.Move(s[2].Location);
                                if (s[1].Location.IsMaze())
                                {
                                    pieces[subject.Id] = subject.Move(s[3].Location);
                                }
                                break;
                        }
                    }

                    if (subject.Type == PieceType.Reporter 
                     && s[2].Type == SelectionType.Target)
                    {
                        target = game.PiecesIndexedByLocation[s[2].Location];
                        pieces[target.Id] = target.Kill();
                    }
                    
                    //Capture pieces if Chief killed
                    if (target?.Type == PieceType.Chief 
                     && subject.Type != PieceType.Diplomat 
                     && subject.Type != PieceType.Necromobile)
                    {
                        //Safe to assume PlayerId.HasValue because a Chief cannot be abandoned
                        var targetOwner = players[target.PlayerId.Value];

                        players[targetOwner.Id] = targetOwner.Kill();

                        var capturedPieces = game.Pieces
                            .Where(p => p.PlayerId == targetOwner.Id);

                        foreach (var p in capturedPieces)
                        {
                            pieces[p.Id] = p.Capture(subjectOwner.Id);
                        }
                    }

                    //Update turn cycle if Chief enters or leaves the Maze
                    if (destination.IsMaze())
                    {
                        //If removing Chief from Maze
                        if (target?.Type == PieceType.Chief 
                         && destination.IsMaze())
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
                        }

                        //If moving Chief to Maze
                        if (subject.Type == PieceType.Chief 
                         && destination.IsMaze())
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
                        }
                    }

                    //TODO: Check for surrounded chiefs

                    //TODO: Check for stalemate

                    var newGameState = GameState.Create(
                        players.Values,
                        pieces.Values,
                        turnCycle);

                    return _validationService.ValidateGameState(newGameState)
                        .Map(__ => game);
                });           
        }
    }
}
