using System.Collections.Generic;
using Djambi.Model;

namespace Djambi.Engine.PieceStrategies
{
    interface IPieceStrategy
    {
        TurnState GetNextTurnState(GameState game, TurnState turn, Selection newSelection);

        Result<IEnumerable<Selection>> GetMoveDestinations(GameState game, Piece piece);

        Result<IEnumerable<Selection>> GetAdditionalSelections(GameState game, Piece piece, TurnState turn);
    }
}
