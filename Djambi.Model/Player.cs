namespace Djambi.Model
{
    public class Player
	{
        public int Id { get; }

        public string Name { get; }

        private Player(int id, string name)
        {
            Id = id;
            Name = name;
        }

        public static Player Create(int id, string name) =>
            new Player(id, name);
	}
}
