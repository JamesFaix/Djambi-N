using System.Collections.Generic;
using System.Collections.Immutable;

namespace Djambi.Model
{
    public class Player
	{
        public int Id { get; }

        public string Name { get; }

        public bool IsAlive { get; }

        public ImmutableList<int> Factions { get; }

        private Player(
            int id, 
            string name, 
            bool isAlive,
            IEnumerable<int> factions)
        {
            Id = id;
            Name = name;
            IsAlive = isAlive;
            Factions = factions.ToImmutableList();
        }

        public static Player Create(
            int id, 
            string name,
            bool isAlive,
            IEnumerable<int> factions) =>
            new Player(id, name, isAlive, factions);
	}
}
