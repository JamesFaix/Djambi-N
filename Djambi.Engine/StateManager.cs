using System.Collections.Generic;
using System.Linq;
using Djambi.Model;

namespace Djambi.Engine
{
    public static class StateManager
    {
        private static readonly Validator _validator;
        
        static StateManager()
        {
            _validator = new Validator();
            GameState = GameState.Empty;
            Turn = Turn.Create(
                TurnState.Paused, 
                Enumerable.Empty<Location>(),
                false);
        }

        public static GameState GameState { get; private set; }

        public static Turn Turn { get; private set; }

        public static Result<Unit> SetGameState(GameState state)
        {
            return _validator.ValidateGameState(state)
                .OnValue(_ => { GameState = state; });
        }

        public static Result<Unit> SetTurn(Turn turn)
        {
            //TODO: Do some validation here
            Turn = turn;
            return Unit.Value.ToResult();
        }

        public static IEnumerable<Location> GetValidSelections()
        {
            /*
             * Calculate all possible actions based on current turn 
             */

            return Enumerable.Empty<Location>();
        }

        public static Result<Unit> MakeSelection(Location location)
        {
            /*
             * Check if the given location is a valid selection
             * If not, error
             * If valid, update turn & return
             */

            return Unit.Value.ToResult();
        }
    }
}
