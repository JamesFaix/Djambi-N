using System;
using System.Collections.Generic;
using System.Linq;
using Djambi.Engine.Extensions;
using Djambi.Model;

namespace Djambi.Engine.Services.PieceStrategies
{
    class AssassinStrategy : PieceStrategyBase
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

        public override TurnState GetNextTurnState(GameState game, TurnState turn, Selection newSelection)
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
    }
}
