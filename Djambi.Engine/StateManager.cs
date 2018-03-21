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
        private static readonly GameInitializationService _gameInitializationService;
        
        static StateManager()
        {
            _validationService = new ValidationService();
            _gameInitializationService = new GameInitializationService();
            GameState = GameState.Empty;
            TurnState = TurnState.Create(
                TurnStatus.Paused, 
                Enumerable.Empty<Selection>(),
                false);
        }

        public static GameState GameState { get; private set; }

        public static TurnState TurnState { get; private set; }

        public static Result<Unit> StartGame(IEnumerable<string> playerNames)
        {
            return _gameInitializationService
               .InitializeGame(playerNames)
               .OnValue(state =>
               {
                   GameState = state;
                   TurnState = TurnState.Empty();
               })
               .Map(_ => Unit.Value);
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

        public static Result<Unit> ConfirmTurn()
        {
            /*
             * Check if TurnState is valid
             * If not, error
             * If valid, update GameState
             */

            return Unit.Value.ToResult();
        }

        public static Result<Unit> CancelTurn()
        {
            TurnState = TurnState.Empty();
            return Unit.Value.ToResult();
        }
    }
}
