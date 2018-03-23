namespace Djambi.Model
{
    public class Player
	{
        public int Id { get; }

        public string Name { get; }

        public bool IsAlive { get; }

        public bool IsVirtual { get; }

        public PlayerColor Color { get; }

        private Player(
            int id, 
            string name, 
            bool isAlive,
            bool isVirtual,
            PlayerColor color)
        {
            Id = id;
            Name = name;
            IsAlive = isAlive;
            IsVirtual = isVirtual;
            Color = color;
        }

        public override string ToString() =>
            $"Id: {Id}, " + 
            $"Name: {Name}, " + 
            $"IsAlive: {IsAlive}, " + 
            $"IsVirtual: {IsVirtual}";

        public static Player Create(
            int id, 
            string name,
            bool isAlive,
            bool isVirtual,
            PlayerColor color) =>
            new Player(id, name, isAlive, isVirtual, color);
	}
}
