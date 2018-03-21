namespace Djambi.Model
{
	public class Selection
    {
		public Location Location { get; }

		public string Description { get; }

		private Selection(
			Location location,
			string description)
        {
            Location = location;
            Description = description;
        }

        public static Selection Create(
            Location location,
            string description) =>
            new Selection(location, description);
    }
}