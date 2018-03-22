using System;
using System.Collections.Generic;
using System.Collections.Immutable;
using System.Linq;
using Djambi.Engine.Extensions;
using Djambi.Model;

namespace Djambi.Engine.Services
{
    class SelectionService
    {
        public Result<IEnumerable<Selection>> GetValidSelections(GameState game, TurnState turn)
        {
            var livingPlayers = game.Players
                .Where(p => p.IsAlive)
                .ToList();

            if (livingPlayers.Count == 1)
            {
                return new Exception($"Game over, {livingPlayers.Single().Name} wins.")
                    .ToErrorResult<IEnumerable<Selection>>();
            }

            var currentPlayerId = game.TurnCycle.First();
            var currentPlayersPieces = game.Pieces
                .Where(piece => piece.PlayerId == currentPlayerId)
                .ToList();

            if (currentPlayersPieces.Count == 0)
            {
                return new Exception($"Current player has 0 pieces")
                    .ToErrorResult<IEnumerable<Selection>>();
            }

            if (turn.Selections.Count == 0)
            {
                return currentPlayersPieces
                    .Where(piece => 
                    {
                        //Filter out pieces that would have no valid destinations (surrounded)
                        var destinations = GetPieceDestinations(piece, game);
                        return destinations.HasValue && destinations.Value.Any();
                    })
                    .Select(piece => Selection.Create(
                        piece.Location, 
                        SelectionType.MoveDestination, 
                        $"Use {piece.Type} at {piece.Location}."))
                    .ToResult();
            }
            else
            {
                var pieceToMove = game.Pieces
                    .Where(piece => piece.Location == turn.Selections[0].Location)
                    .SingleOrDefault();

                if (pieceToMove == null)
                {
                    return new Exception($"No piece at first selection's location.")
                        .ToErrorResult<IEnumerable<Selection>>();
                }

                if (turn.Selections.Count == 1)
                {
                    return GetPieceDestinations(pieceToMove, game);
                }
                else
                {
                    return GetAdditionalSelections(pieceToMove, game, turn.Selections);
                }
            }
        }

        #region Get next turn state

        public TurnState GetNextTurnState(GameState game, TurnState turn, Selection newSelection)
        {
            //The new selection is the piece to move, so the next selection is the destination
            if (turn.Selections.Count == 0)
            {
                return TurnState.Create(
                    TurnStatus.AwaitingSelection, 
                    turn.Selections.Add(newSelection));
            }

            var subject = game.PiecesIndexedByLocation[turn.Selections[0].Location];

            switch (subject.Type)
            {
                case PieceType.Assassin:
                    return GetNextTurnStatusForAssassin(game, turn, subject, newSelection);
                case PieceType.Chief:
                    return GetNextTurnStatusForAssassin(game, turn, subject, newSelection);
                case PieceType.Diplomat:
                    return GetNextTurnStatusForAssassin(game, turn, subject, newSelection);
                case PieceType.Militant:
                    return GetNextTurnStatusForAssassin(game, turn, subject, newSelection);
                case PieceType.Necromobile:
                    return GetNextTurnStatusForAssassin(game, turn, subject, newSelection);
                case PieceType.Reporter:
                    return GetNextTurnStatusForAssassin(game, turn, subject, newSelection);
                default:
                    throw new Exception($"Invalid {nameof(PieceType)} value ({subject.Type}).");
            }
        }

        private TurnState GetNextTurnStatusForAssassin(GameState game, TurnState turn, Piece subject, Selection newSelection)
        { 
            //A subject has already been selected, the new selection is a destination (possibly with target)
            if (turn.Selections.Count == 1)
            {
                if (newSelection.Type == SelectionType.MoveDestinationWithTarget)
                {
                    var target = game.PiecesIndexedByLocation[newSelection.Location];
                    
                    if (target.Type == PieceType.Chief && newSelection.Location.IsMaze())
                    {
                        //Must move out of maze
                        return TurnState.Create(
                            TurnStatus.AwaitingSelection,
                            turn.Selections.Add(newSelection));
                    }
                }
            }

            return TurnState.Create(
                TurnStatus.AwaitingConfirmation,
                turn.Selections.Add(newSelection));
        }
        
        private TurnState GetNextTurnStatusForChief(GameState game, TurnState turn, Piece subject, Selection newSelection)
        {
            //A subject has already been selected, the new selection is a destination (possibly with target)
            if (turn.Selections.Count == 1)
            {
                if (newSelection.Type == SelectionType.MoveDestinationWithTarget)
                {
                    //The new selection is a move with target, so a drop destination is required
                    return TurnState.Create(
                        TurnStatus.AwaitingSelection,
                        turn.Selections.Add(newSelection));
                }
            }

            return TurnState.Create(
                TurnStatus.AwaitingConfirmation,
                turn.Selections.Add(newSelection));
        }

        private TurnState GetNextTurnStatusForDiplomat(GameState game, TurnState turn, Piece subject, Selection newSelection)
        {
            //A subject has already been selected, the new selection is a destination (possibly with target)
            if (turn.Selections.Count == 1)
            {
                if (newSelection.Type == SelectionType.MoveDestinationWithTarget)
                {
                    //The new selection is a move with target, so a drop destination is required
                    return TurnState.Create(
                        TurnStatus.AwaitingSelection,
                        turn.Selections.Add(newSelection));
                }
            }

            if (turn.Selections.Count == 2)
            {
                //If the last selection targeted a Chief in the Maze, the Diplomat must escape
                if (turn.Selections[1].Location.IsMaze())
                {
                    return TurnState.Create(
                        TurnStatus.AwaitingSelection,
                        turn.Selections.Add(newSelection));
                }
            }

            return TurnState.Create(
                TurnStatus.AwaitingConfirmation,
                turn.Selections.Add(newSelection));
        }
        
        private TurnState GetNextTurnStatusForMilitant(GameState game, TurnState turn, Piece subject, Selection newSelection)
        {
            //A subject has already been selected, the new selection is a destination (possibly with target)
            if (turn.Selections.Count == 1)
            {
                if (newSelection.Type == SelectionType.MoveDestinationWithTarget)
                {
                    //The new selection is a move with target, so a drop destination is required
                    return TurnState.Create(
                        TurnStatus.AwaitingSelection,
                        turn.Selections.Add(newSelection));
                }
            }

            return TurnState.Create(
                TurnStatus.AwaitingConfirmation,
                turn.Selections.Add(newSelection));
        }
        
        private TurnState GetNextTurnStatusForNecromobile(GameState game, TurnState turn, Piece subject, Selection newSelection)
        {
            //A subject has already been selected, the new selection is a destination (possibly with target)
            if (turn.Selections.Count == 1)
            {
                if (newSelection.Type == SelectionType.MoveDestinationWithTarget)
                {
                    //The new selection is a move with target, so a drop destination is required
                    return TurnState.Create(
                        TurnStatus.AwaitingSelection,
                        turn.Selections.Add(newSelection));
                }
            }

            if (turn.Selections.Count == 2)
            {
                //If the last selection targeted a Corpse in the Maze, the Necro must escape
                if (turn.Selections[1].Location.IsMaze())
                {
                    return TurnState.Create(
                        TurnStatus.AwaitingSelection,
                        turn.Selections.Add(newSelection));
                }
            }

            return TurnState.Create(
                TurnStatus.AwaitingConfirmation,
                turn.Selections.Add(newSelection));
        }

        private TurnState GetNextTurnStatusForReporter(GameState game, TurnState turn, Piece subject, Selection newSelection)
        {
            //A subject has already been selected, the new selection is a destination (possibly with target)
            if (turn.Selections.Count == 1)
            {
                //Check for adjacent enemy pieces, if any then another selection is required
                var targetOptions = GetTargetOptionsForReporter(game, subject, newSelection.Location)
                    .ToList();

                if (targetOptions.Any())
                {
                    return TurnState.Create(
                        TurnStatus.AwaitingSelection,
                        turn.Selections.Add(newSelection));
                }    
            }

            return TurnState.Create(
                TurnStatus.AwaitingConfirmation,
                turn.Selections.Add(newSelection));
        }

        #endregion

        private IEnumerable<Location> GetTargetOptionsForReporter(GameState game, Piece piece, Location destination)
        {
            bool IsAdjacent(Location a, Location b)
            {
                return (a.X == b.X && Math.Abs(a.Y - b.Y) == 1)
                    || (a.Y == b.Y && Math.Abs(a.X - b.X) == 1);
            }

            return game.Pieces
                .Where(p => p.PlayerId != piece.PlayerId
                    && p.IsAlive
                    && IsAdjacent(p.Location, destination))
                .Select(p => p.Location);
        }

        #region Select destination (2nd selection)

        private Result<IEnumerable<Selection>> GetPieceDestinations(Piece piece, GameState game)
        {
            switch (piece.Type)
            {
                case PieceType.Assassin:
                    return GetAssassinDestinations(piece, game);
                case PieceType.Chief:
                    return GetChiefDestinations(piece, game);
                case PieceType.Diplomat:
                    return GetDiplomatDestinations(piece, game);
                case PieceType.Militant:
                    return GetMilitantDestinations(piece, game);
                case PieceType.Necromobile:
                    return GetNecromobileDestinations(piece, game);
                case PieceType.Reporter:
                    return GetReporterDestinations(piece, game);
                default:
                    return new Exception($"Invalid {nameof(PieceType)} value ({piece.Type}).")
                        .ToErrorResult<IEnumerable<Selection>>();
            }            
        }

        private Result<IEnumerable<Selection>> GetAssassinDestinations(Piece piece, GameState game)
        {
            return GetColinearNonBlockedLocations(piece, game)
                .Select(loc => GetLocationWithPiece(loc, game.PiecesIndexedByLocation))
                .Where(lwp =>
                {
                    if (lwp.Location.IsMaze())
                    {
                        //Cannot be Maze unless enemy Chief is there
                        return lwp.Piece != null
                            ? lwp.Piece.PlayerId != piece.PlayerId 
                                && lwp.Piece.Type == PieceType.Chief                                
                            : false;
                    }
                    else
                    {
                        //Cannot contain allied piece or Corpse
                        return lwp.Piece != null
                            ? lwp.Piece.PlayerId != piece.PlayerId
                                && lwp.Piece.IsAlive
                            : true;
                    }
                })
                .Select(CreateSelection)
                .ToResult();
        }

        private Result<IEnumerable<Selection>> GetChiefDestinations(Piece piece, GameState game)
        {
            return GetColinearNonBlockedLocations(piece, game)
                .Select(loc => GetLocationWithPiece(loc, game.PiecesIndexedByLocation))
                .Where(lwp =>
                {
                    //Cannot contain allied piece or Corpse
                    return lwp.Piece != null
                        ? lwp.Piece.PlayerId != piece.PlayerId
                            && lwp.Piece.IsAlive
                        : true;
                })
                .Select(CreateSelection)
                .ToResult();
        }

        private Result<IEnumerable<Selection>> GetDiplomatDestinations(Piece piece, GameState game)
        {
            return GetColinearNonBlockedLocations(piece, game)
                .Select(loc => GetLocationWithPiece(loc, game.PiecesIndexedByLocation))
                .Where(lwp =>
                {
                    if (lwp.Location.IsMaze())
                    {
                        //Cannot be Maze unless enemy Chief is there
                        return lwp.Piece != null
                            ? lwp.Piece.PlayerId != piece.PlayerId
                                && lwp.Piece.Type == PieceType.Chief
                            : false;
                    }
                    else
                    {
                        //Cannot contain allied piece or Corpse
                        return lwp.Piece != null
                            ? lwp.Piece.PlayerId != piece.PlayerId
                                && lwp.Piece.IsAlive
                            : true;
                    }
                })
                .Select(CreateSelection)
                .ToResult();
        }

        private Result<IEnumerable<Selection>> GetMilitantDestinations(Piece piece, GameState game)
        {
            return GetColinearNonBlockedLocations(piece, game)
                .Select(loc => GetLocationWithPiece(loc, game.PiecesIndexedByLocation))
                .Where(lwp =>
                {
                    if (piece.Location.Distance(lwp.Location) > 2)
                    {
                        return false;
                    }

                    if (lwp.Location.IsMaze())
                    {
                        return false;
                    }
                    
                    //Cannot contain allied piece or Corpse
                    return lwp.Piece != null
                        ? lwp.Piece.PlayerId != piece.PlayerId
                            && lwp.Piece.IsAlive
                        : true;
                })
                .Select(CreateSelection)
                .ToResult();
        }

        private Result<IEnumerable<Selection>> GetNecromobileDestinations(Piece piece, GameState game)
        {
            return GetColinearNonBlockedLocations(piece, game)
                .Select(loc => GetLocationWithPiece(loc, game.PiecesIndexedByLocation))
                .Where(lwp =>
                {
                    if (lwp.Location.IsMaze())
                    {
                        //Cannot be Maze unless Corpse is there
                        return lwp.Piece != null 
                           && !lwp.Piece.IsAlive;
                    }
                    else
                    {
                        //Cannot contain living piece
                        return lwp.Piece == null 
                           || !lwp.Piece.IsAlive;
                    }
                })
                .Select(CreateSelection)
                .ToResult();
        }

        private Result<IEnumerable<Selection>> GetReporterDestinations(Piece piece, GameState game)
        {
            return GetColinearNonBlockedLocations(piece, game)
                .Select(loc => GetLocationWithPiece(loc, game.PiecesIndexedByLocation))
                .Where(lwp => !lwp.Location.IsMaze()
                            && lwp.Piece == null)
                .Select(CreateSelection)
                .ToResult();
        }

        #endregion

        private Result<IEnumerable<Selection>> GetAdditionalSelections(Piece piece, GameState game, ImmutableList<Selection> selections)
        {
            //Find targets or cells to place corpse in

            return Enumerable.Empty<Selection>().ToResult();
        }

        private IEnumerable<Location> GetColinearNonBlockedLocations(Piece piece, GameState game)
        {
            foreach (var dir in EnumUtility.GetValues<Directions>())
            {
                var loc = piece.Location.Offset(dir, 1);

            //TODO: Refactor to not use GOTO
            next:
                if (!loc.IsValid())
                {
                    continue;
                }
                if (game.Pieces.Any(p => p.Location == loc))
                {
                    yield return loc;
                    continue;
                }
                else
                {
                    yield return loc;
                    loc = loc.Offset(dir, 1);
                    goto next;
                }
            }
        }
        
        private LocationWithPiece GetLocationWithPiece(Location location, ImmutableDictionary<Location, Piece> index) =>
            new LocationWithPiece(location, index.TryGetValue(location, out var p) ? p : null);

        private Selection CreateSelection(LocationWithPiece lwp)
        {
            if (lwp.Piece == null)
            {
                return Selection.Create(
                    lwp.Location,
                    SelectionType.MoveDestination,
                    $"Move to {lwp.Location}.");
            }
            else
            {
                return Selection.Create(
                    lwp.Location,
                    SelectionType.MoveDestinationWithTarget,
                    $"Move to {lwp.Location} and target {lwp.Piece.Type}.");
            }
        }
        
        private class LocationWithPiece
        {
            public Location Location { get; }

            public Piece Piece { get; }

            public LocationWithPiece(Location location, Piece piece)
            {
                Location = location;
                Piece = piece;
            }
        }
    }
}
