using System.Collections.Generic;
using System.Collections.Immutable;

namespace Djambi.Model
{
    public class Player
	{
        public int Id { get; }

        public string Name { get; }

        public bool IsAlive { get; }

        public bool IsVirtual { get; }

        public PlayerColor Color { get; }

        public ImmutableList<int> ConqueredPlayerIds { get; }

        private Player(
            int id, 
            string name, 
            bool isAlive,
            bool isVirtual,
            PlayerColor color,
            IEnumerable<int> conqueredPlayerIds)
        {
            Id = id;
            Name = name;
            IsAlive = isAlive;
            IsVirtual = IsVirtual;
            Color = color;
            ConqueredPlayerIds = conqueredPlayerIds.ToImmutableList();
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
            PlayerColor color,
            IEnumerable<int> conqueredPlayerIds) =>
            new Player(id, name, isAlive, isVirtual, color, conqueredPlayerIds);
	}
}
