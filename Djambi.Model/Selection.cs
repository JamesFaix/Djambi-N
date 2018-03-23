namespace Djambi.Model
{
	public class Selection
    {
		public Location Location { get; }

        public SelectionType Type { get; }

		public string Description { get; }

		private Selection(
			Location location,
            SelectionType type,
			string description)
        {
            Location = location;
            Type = type;
            Description = description;
        }

        public static Selection Subject(Piece piece) =>
            new Selection(
                piece.Location, 
                SelectionType.Subject, 
                $"Use {piece.Type} at {piece.Location}.");

        public static Selection Move(Location location) =>
            new Selection(
                location,
                SelectionType.MoveDestination,
                $"Move to {location}.");

        public static Selection MoveWithTarget(Location location, Piece piece) =>
            new Selection(
                location,
                SelectionType.MoveDestinationWithTarget,
                $"Move to {location} and target {piece.Type}.");

        public static Selection Target(Location location, Piece piece) =>
            new Selection(
                location,
                SelectionType.Target,
                $"Target {piece.Type} at {location}.");

        public static Selection Drop(Location location) =>
            new Selection(
                location,
                SelectionType.DropDestination,
                 $"Drop target at {location}.");
    }
}