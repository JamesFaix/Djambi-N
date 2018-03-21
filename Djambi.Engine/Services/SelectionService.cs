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
            /*
             * Colinear locations
             * Cannot be blocked by another piece
             * Cannot be Maze unless enemy Chief is there
             * Cannot contain allied piece or Corpse
             */
        }

        private Result<IEnumerable<Selection>> GetChiefDestinations(Piece piece, GameState state)
        {
            /*
             * Colinear locations
             * Cannot be blocked by another piece
             * Cannot contain allied piece or Corpse
             */
        }

        private Result<IEnumerable<Selection>> GetDiplomatDestinations(Piece piece, GameState state)
        {
            /*
             * Colinear locations
             * Cannot be blocked by another piece
             * Cannot be Maze unless enemy Chief is there
             * Cannot contain allied piece or Corpse
             */
        }

        private Result<IEnumerable<Selection>> GetMilitantDestinations(Piece piece, GameState state)
        {
            /*
             * Colinear locations with max distance of 2 
             * Cannot be blocked by another piece
             * Cannot be Maze
             * Cannot contain allied piece or Corpse
             */
        }

        private Result<IEnumerable<Selection>> GetNecromobileDestinations(Piece piece, GameState state)
        {
            /*
             * Colinear locations
             * Cannot be blocked by another piece
             * Cannot be Maze unless corpse is there
             * Cannot contain living piece
             */
        }

        private Result<IEnumerable<Selection>> GetReporterDestinations(Piece piece, GameState state)
        {
            /*
             * Colinear locations
             * Cannot be blocked by another piece
             * Cannot be Maze
             * Cannot contain any piece
             */
        }

        private Result<IEnumerable<Selection>> GetAdditionalSelections(Piece piece, GameState state, ImmutableList<Selection> selections)
        {
            //Find targets or cells to place corpse in
        }
    }
}
