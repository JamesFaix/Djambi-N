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
            InteractionState = InteractionState.Paused;
        }

        public static GameState GameState { get; private set; }

        public static InteractionState InteractionState { get; private set; }

        public static Result<Unit> SetGameState(GameState state)
        {
            return _validator.ValidateGameState(state)
                .OnValue(_ => { GameState = state; });
        }

        public static Result<Unit> SetInteractionState(InteractionState state)
        {
            InteractionState = state;
            return Unit.Value.ToResult();
        }
    }
}
