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
        public Result<IEnumerable<Selection>> GetValidSelections(
            GameState gameState, 
            TurnState turnState)
        {
            var livingPlayers = gameState.Players
                .Where(p => p.IsAlive)
                .ToList();

            if (livingPlayers.Count == 1)
            {
                return new Exception($"Game over, {livingPlayers.Single().Name} wins.")
                    .ToErrorResult<IEnumerable<Selection>>();
            }

            var currentPlayerId = gameState.TurnCycle.First();
            var currentPlayersPieces = gameState.Pieces
                .Where(piece => piece.PlayerId == currentPlayerId)
                .ToList();

            if (currentPlayersPieces.Count == 0)
            {
                return new Exception($"Current player has 0 pieces")
                    .ToErrorResult<IEnumerable<Selection>>();
            }

            if (turnState.Selections.Count == 0)
            {
                return currentPlayersPieces
                    .Where(piece => 
                    {
                        //Filter out pieces that would have no valid destinations (surrounded)
                        var destinations = GetPieceDestinations(piece, gameState);
                        return destinations.HasValue && destinations.Value.Any();
                    })
                    .Select(piece => Selection.Create(piece.Location, "Move piece"))
                    .ToResult();
            }
            else
            {
                var pieceToMove = gameState.Pieces
                    .Where(piece => piece.Location == turnState.Selections[0].Location)
                    .SingleOrDefault();

                if (pieceToMove == null)
                {
                    return new Exception($"No piece at first selection's location.")
                        .ToErrorResult<IEnumerable<Selection>>();
                }

                if (turnState.Selections.Count == 1)
                {
                    return GetPieceDestinations(pieceToMove, gameState);
                }
                else
                {
                    return GetAdditionalSelections(pieceToMove, gameState, turnState.Selections);
                }
            }
        }

        private Result<IEnumerable<Selection>> GetPieceDestinations(Piece piece, GameState state)
        {
            switch (piece.Type)
            {
                case PieceType.Assassin:
                    return GetAssassinDestinations(piece, state);
                case PieceType.Chief:
                    return GetChiefDestinations(piece, state);
                case PieceType.Diplomat:
                    return GetDiplomatDestinations(piece, state);
                case PieceType.Militant:
                    return GetMilitantDestinations(piece, state);
                case PieceType.Necromobile:
                    return GetNecromobileDestinations(piece, state);
                case PieceType.Reporter:
                    return GetReporterDestinations(piece, state);
                default:
                    return new Exception($"Invalid {nameof(PieceType)} value ({piece.Type}).")
                        .ToErrorResult<IEnumerable<Selection>>();
            }            
        }

        private Result<IEnumerable<Selection>> GetAssassinDestinations(Piece piece, GameState state)
        {
            var index = IndexPiecesByLocation(state);

            return GetColinearNonBlockedLocations(piece, state)
                .Select(loc => GetLocationWithPiece(loc, index))
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

        private Result<IEnumerable<Selection>> GetChiefDestinations(Piece piece, GameState state)
        {
            var index = IndexPiecesByLocation(state);

            return GetColinearNonBlockedLocations(piece, state)
                .Select(loc => GetLocationWithPiece(loc, index))
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

        private Result<IEnumerable<Selection>> GetDiplomatDestinations(Piece piece, GameState state)
        {
            var index = IndexPiecesByLocation(state);

            return GetColinearNonBlockedLocations(piece, state)
                .Select(loc => GetLocationWithPiece(loc, index))
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

        private Result<IEnumerable<Selection>> GetMilitantDestinations(Piece piece, GameState state)
        {
            var index = IndexPiecesByLocation(state);

            return GetColinearNonBlockedLocations(piece, state)
                .Select(loc => GetLocationWithPiece(loc, index))
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

        private Result<IEnumerable<Selection>> GetNecromobileDestinations(Piece piece, GameState state)
        {
            var index = IndexPiecesByLocation(state);

            return GetColinearNonBlockedLocations(piece, state)
                .Select(loc => GetLocationWithPiece(loc, index))
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

        private Result<IEnumerable<Selection>> GetReporterDestinations(Piece piece, GameState state)
        {
            var index = IndexPiecesByLocation(state);

            return GetColinearNonBlockedLocations(piece, state)
                .Select(loc => GetLocationWithPiece(loc, index))
                .Where(lwp => !lwp.Location.IsMaze()
                            && lwp.Piece == null)
                .Select(CreateSelection)
                .ToResult();
        }

        private Result<IEnumerable<Selection>> GetAdditionalSelections(Piece piece, GameState state, ImmutableList<Selection> selections)
        {
            //Find targets or cells to place corpse in

            return Enumerable.Empty<Selection>().ToResult();
        }

        private IEnumerable<Location> GetColinearNonBlockedLocations(Piece piece, GameState state)
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
                if (state.Pieces.Any(p => p.Location == loc))
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

        private Dictionary<Location, Piece> IndexPiecesByLocation(GameState state) =>
            state.Pieces.ToDictionary(p => p.Location, p => p);

        private LocationWithPiece GetLocationWithPiece(Location location, Dictionary<Location, Piece> index) =>
            new LocationWithPiece(location, index.TryGetValue(location, out var p) ? p : null);

        private Selection CreateSelection(LocationWithPiece lwp)
        {
            var desc = lwp.Piece == null ? "Move" : $"Target {lwp.Piece.Type}";
            return Selection.Create(lwp.Location, desc);
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
