using System;
using System.Collections.Generic;
using System.Linq;
using Djambi.Engine.Extensions;
using Djambi.Model;

namespace Djambi.Engine.Services.PieceStrategies
{
    class AssassinStrategy : PieceStrategyBase
    {
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
        
        public override Result<IEnumerable<Selection>> GetAdditionalSelections(GameState game, Piece piece, TurnState turn)
        {
            if (turn.Selections.Count == 2 
             && turn.Selections[1].Location.IsMaze())
            {
                //Escape Maze after killing Chief
                var preview = _gameUpdateService.PreviewGameUpdate(game, turn);
                var movedSubject = preview.PiecesIndexedByLocation[turn.Selections[1].Location];
                return GetMoveDestinations(preview, movedSubject);
            }
            else
            {
                return new Exception($"{PieceType.Assassin} can only make an additional selection to escape the Maze after killing a {PieceType.Chief} in power.")
                    .ToErrorResult<IEnumerable<Selection>>();
            }
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
