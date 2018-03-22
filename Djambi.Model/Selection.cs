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

        public static Selection Create(
            Location location,
            SelectionType type,
            string description) =>
            new Selection(location, type, description);
    }
}