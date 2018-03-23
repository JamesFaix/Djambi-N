using System;
using System.Collections.Generic;
using System.Linq;
using Djambi.Engine.Extensions;
using Djambi.Model;

namespace Djambi.Engine.Services.PieceStrategies
{
    class MilitantStrategy : PieceStrategyBase
    {
        public override Result<IEnumerable<Selection>> GetAdditionalSelections(GameState game, Piece piece, TurnState turn)
        {
            throw new NotImplementedException();
        }

        public override Result<IEnumerable<Selection>> GetMoveDestinations(GameState game, Piece piece)
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

        public override TurnState GetNextTurnState(GameState game, TurnState turn, Selection newSelection)
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
    }
}
