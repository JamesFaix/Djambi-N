using System;
using System.Collections.Generic;
using System.Linq;
using Djambi.Engine.Extensions;
using Djambi.Model;

namespace Djambi.Engine.Services.PieceStrategies
{
    class NecromobileStrategy : PieceStrategyBase
    {
        public override Result<IEnumerable<Selection>> GetMoveDestinations(GameState game, Piece piece)
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

        public override Result<IEnumerable<Selection>> GetAdditionalSelections(GameState game, Piece piece, TurnState turn)
        {
            if (turn.Status == TurnStatus.AwaitingSelection)
            {
                if (turn.Selections.Count == 2)
                {
                    //Drop target
                    var preview = _gameUpdateService.PreviewGameUpdate(game, turn);
                    var movedSubject = preview.PiecesIndexedByLocation[turn.Selections[1].Location];
                    return GetEmptyLocations(preview)
                        .Select(Selection.Drop)
                        .ToResult();
                }

                if (turn.Selections.Count == 3
                 && turn.Selections[1].Location.IsMaze())
                {
                    //Escape Maze after dropping Chief
                    var preview = _gameUpdateService.PreviewGameUpdate(game, turn);
                    var movedSubject = preview.PiecesIndexedByLocation[turn.Selections[2].Location];
                    return GetMoveDestinations(preview, movedSubject);
                }
            }

            return Enumerable.Empty<Selection>().ToResult();
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
    }
}
