using System;
using System.Collections.Generic;
using System.Linq;
using Djambi.Engine.Extensions;
using Djambi.Engine.Services.PieceStrategies;
using Djambi.Model;

namespace Djambi.Engine.Services
{
    class SelectionService
    {
        private readonly Dictionary<PieceType, IPieceStrategy> _pieceStrategies = new Dictionary<PieceType, IPieceStrategy>
        {
            [PieceType.Assassin]    = new AssassinStrategy(),
            [PieceType.Chief]       = new ChiefStrategy(),
            [PieceType.Diplomat]    = new DiplomatStrategy(),
            [PieceType.Thug]    = new ThugStrategy(),
            [PieceType.Undertaker] = new UndertakerStrategy(),
            [PieceType.Journalist]    = new JournalistStrategy()
        };

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
                        var destinations = GetMoveDestinations(piece, game);
                        return destinations.HasValue && destinations.Value.Any();
                    })
                    .Select(Selection.Subject)
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
                    return GetMoveDestinations(pieceToMove, game);
                }
                else
                {
                    return GetAdditionalSelections(pieceToMove, game, turn);
                }
            }
        }
        
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

            return _pieceStrategies[subject.Type].GetNextTurnState(game, turn, newSelection);
        }
        
        private Result<IEnumerable<Selection>> GetMoveDestinations(Piece piece, GameState game) =>
            _pieceStrategies[piece.Type].GetMoveDestinations(game, piece);
        
        private Result<IEnumerable<Selection>> GetAdditionalSelections(Piece piece, GameState game, TurnState turn) =>
            _pieceStrategies[piece.Type].GetAdditionalSelections(game, piece, turn);        
    }
}
