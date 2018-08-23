using System;
using System.Collections.Generic;
using System.Linq;
using Djambi.Engine.Extensions;
using Djambi.Engine.Services;
using Djambi.Model;

namespace Djambi.Engine
{
    public static class Controller
    {
        private static readonly GameInitializationService _gameInitializationService;
        private static readonly GameUpdateService _gameUpdateService;
        private static readonly SelectionService _selectionService;
        private static readonly ValidationService _validationService;

        static Controller()
        {
            _validationService = new ValidationService();
            _gameInitializationService = new GameInitializationService();
            _gameUpdateService = new GameUpdateService(_validationService);
            _selectionService = new SelectionService(_gameUpdateService);

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
            if (TurnState.Status != TurnStatus.AwaitingSelection)
            {
                return new Exception($"Invalid turn status ({TurnState.Status}).")
                    .ToErrorResult<Unit>();
            }

            return GetValidSelections()
                .Bind(selections =>
                {
                    var selection = selections.SingleOrDefault(s => s.Location == location);

                    if (selection == null)
                    {
                        return new Exception($"Invalid selection {location}.")
                            .ToErrorResult<Unit>();
                    }
                    else
                    {
                        TurnState = _selectionService.GetNextTurnState(GameState, TurnState, selection);
                        return Unit.Value.ToResult();
                    }
                });            
        }

        public static Result<GameState> ConfirmTurn()
        {
            return _gameUpdateService.UpdateGameState(GameState, TurnState)
                .OnValue(game => 
                {
                    GameState = game;
                    TurnState = TurnState.Empty;
                });
        }

        public static Result<Unit> CancelTurn()
        {
            TurnState = TurnState.Empty;
            return Unit.Value.ToResult();
        }

        public static event EventHandler<string> OnLogWrite;
    }
}
