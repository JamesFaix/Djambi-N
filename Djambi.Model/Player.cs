namespace Djambi.Model
{
    public class Player
	{
        public int Id { get; }

        public string Name { get; }

        public bool IsAlive { get; }

        private Player(
            int id, 
            string name, 
            bool isAlive)
        {
            Id = id;
            Name = name;
            IsAlive = isAlive;
        }

        public static Player Create(
            int id, 
            string name,
            bool isAlive) =>
            new Player(id, name, isAlive);
	}
}
