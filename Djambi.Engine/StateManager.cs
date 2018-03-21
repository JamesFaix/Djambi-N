using System.Collections.Generic;
using System.Linq;
using Djambi.Engine.Extensions;
using Djambi.Engine.Services;
using Djambi.Model;

namespace Djambi.Engine
{
    public static class StateManager
    {
        private static readonly ValidationService _validationService;
        
        static StateManager()
        {
            _validationService = new ValidationService();
            GameState = GameState.Empty;
            TurnState = TurnState.Create(
                TurnStatus.Paused, 
                Enumerable.Empty<Selection>(),
                false);
        }

        public static GameState GameState { get; private set; }

        public static TurnState TurnState { get; private set; }

        public static Result<Unit> SetGameState(GameState state)
        {
            return _validationService.ValidateGameState(state)
                .OnValue(_ => { GameState = state; });
        }

        public static Result<Unit> SetTurnState(TurnState state)
        {
            //TODO: Do some validation here
            TurnState = state;
            return Unit.Value.ToResult();
        }

        public static IEnumerable<Selection> GetValidSelections()
        {
            /*
             * Calculate all possible actions based on current turn 
             */

            return Enumerable.Empty<Selection>();
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
