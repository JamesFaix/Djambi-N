using System.Collections.Generic;
using Djambi.Engine.Extensions;
using Djambi.Engine.Services;
using Djambi.Model;

namespace Djambi.Engine
{
    public static class Controller
    {
        private static readonly ValidationService _validationService;
        private static readonly GameInitializationService _gameInitializationService;
        private static readonly SelectionService _selectionService;
        
        static Controller()
        {
            _validationService = new ValidationService();
            _gameInitializationService = new GameInitializationService();
            _selectionService = new SelectionService();

            GameState = GameState.Empty;
            TurnState = TurnState.MainMenu;
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
                   TurnState = TurnState.Empty;
               })
               .Map(_ => Unit.Value);
        }

        public static Result<IEnumerable<Selection>> GetValidSelections() =>
            _selectionService.GetValidSelections(GameState, TurnState);

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
            TurnState = TurnState.Empty;
            return Unit.Value.ToResult();
        }
    }
}
