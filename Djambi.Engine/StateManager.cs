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
            Interaction = Interaction.Create(
                InteractionState.Paused, 
                Option.None<Piece>(), 
                Option.None<Location>(), 
                Option.None<Piece>());
        }

        public static GameState GameState { get; private set; }

        public static Interaction Interaction { get; private set; }

        public static Result<Unit> SetGameState(GameState state)
        {
            return _validator.ValidateGameState(state)
                .OnValue(_ => { GameState = state; });
        }

        public static Result<Unit> SetInteraction(Interaction interaction)
        {
            //TODO: Do some validation here
            Interaction = interaction;
            return Unit.Value.ToResult();
        }

        public static PossibleActions GetPossibleActions()
        {
            /*
             * Calculate all possible actions based on current interaction 
             */
        }

        public static Result<PossibleActions> SelectSubject(Location location)
        {
            /*
             * Find the piece at the given location
             * If there is none, error
             * Check if the piece belongs to the current player
             * If not, error
             * Get legal moves for piece
             * If none, error
             * Set Subject
             * Set state to AwaitingDestinationSelection or AwaitingTargetSelection
             * Return possible actions
             */
        }

        public static Result<PossibleActions> SelectDestination(Location location)
        {
            /*
             * Check if there is a Subject
             * If not, error
             * Check if the given location is a legal destination for the Subject
             * If not, error
             * Set Destination (and maybe Target)
             * Set state to AwaitingTargetSelection or AwaitingConfirmation
             * Return possible actions
             */
        }

        public static Result<PossibleActions> SelectTarget(Location location)
        {
            /*
             * Check if there is a Subject and Destination
             * If not, error
             * Check if there is a piece at the given location
             * If not, error
             * Check if the piece is a legal target for the Subject
             * If not, error
             * Set Target
             * Set state to AwaitingConfirmation
             * Return possible actions
             */
        }
    }
}
