namespace Djambi.Model
{
    public class Interaction
    {
        public InteractionState State { get; }

        public Option<Piece> Subject { get; }

        public Option<Location> Destination { get; }

        public Option<Piece> Target { get; }

        private Interaction(
            InteractionState state,
            Option<Piece> subject,
            Option<Location> destination,
            Option<Piece> target)
        {
            State = state;
            Subject = subject;
            Destination = destination;
            Target = target;
        }

        public static Interaction Create(
            InteractionState state,
            Option<Piece> subject,
            Option<Location> destination,
            Option<Piece> target) => 
            new Interaction(state, subject, destination, target);
    }
}
