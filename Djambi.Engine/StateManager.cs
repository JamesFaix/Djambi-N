using Djambi.Model;

namespace Djambi.Engine
{
    public static class StateManager
    {
        private static readonly Validator _validator;

        private static GameState _current;

        static StateManager()
        {
            _validator = new Validator();
            _current = GameState.Empty;
        }

        public static GameState Current => _current;

        public static Result<Unit> SetCurrentState(GameState state)
        {
            return _validator.ValidateGameState(state)
                .OnValue(_ => { _current = state; });
        }
    }
}
