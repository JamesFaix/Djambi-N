using System;
using System.Collections.Generic;
using System.Linq;
using Djambi.Engine.Extensions;
using Djambi.Engine.Services;
using Djambi.Model;

namespace Djambi.Engine.PieceStrategies
{
    class JournalistStrategy : PieceStrategyBase
    {
        public JournalistStrategy(GameUpdateService gameUpdateService)
            : base(gameUpdateService) { }

        public override Result<IEnumerable<Selection>> GetMoveDestinations(GameState game, Piece piece)
        {
            return GetColinearNonBlockedLocations(piece, game)
                .Select(loc => GetLocationWithPiece(loc, game.PiecesIndexedByLocation))
                .Where(lwp => !lwp.Location.IsSeat()
                            && lwp.Piece == null)
                .Select(CreateSelection)
                .ToResult();
        }

        public override Result<IEnumerable<Selection>> GetAdditionalSelections(GameState game, Piece piece, TurnState turn)
        {
            if (turn.Status == TurnStatus.AwaitingSelection
             && turn.Selections.Count == 2)
            {
                //Select target if possible
                return GetTargetOptions(game, piece, turn.Selections[1].Location)
                    .Select(loc =>
                    {
                        var target = game.PiecesIndexedByLocation[loc];
                        return Selection.Target(loc, target);
                    })
                    .ToResult();
            }

            return Enumerable.Empty<Selection>().ToResult();
        }

        public override TurnState GetNextTurnState(GameState game, TurnState turn, Selection newSelection)
        {
            //A subject has already been selected, the new selection is a destination (possibly with target)
            if (turn.Selections.Count == 1)
            {
                var subject = game.PiecesIndexedByLocation[turn.Selections[0].Location];

                //Check for adjacent enemy pieces, if any then another selection is required
                var targetOptions = GetTargetOptions(game, subject, newSelection.Location)
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

        private IEnumerable<Location> GetTargetOptions(GameState game, Piece piece, Location destination)
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
    }
}
