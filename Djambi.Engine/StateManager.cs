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
            TurnState = TurnState.Create(
                TurnStatus.Paused, 
                Enumerable.Empty<Location>(),
                false);
        }

        public static GameState GameState { get; private set; }

        public static TurnState TurnState { get; private set; }

        public static Result<Unit> SetGameState(GameState state)
        {
            return _validator.ValidateGameState(state)
                .OnValue(_ => { GameState = state; });
        }

        public static Result<Unit> SetTurnState(TurnState state)
        {
            //TODO: Do some validation here
            TurnState = state;
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
             * If valid, update turn state & return
             */

            return Unit.Value.ToResult();
        }
    }
}
