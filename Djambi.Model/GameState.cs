using System.Collections.Generic;
using System.Collections.Immutable;
using System.Linq;

namespace Djambi.Model
{
    public class GameState
    {
        public ImmutableList<Player> Players { get; }

        public ImmutableList<Faction> Factions { get; }

        public ImmutableList<Piece> Pieces { get; }
        
        public ImmutableList<int> TurnCycle { get; }

        private GameState(
            IEnumerable<Player> players, 
            IEnumerable<Faction> factions,
            IEnumerable<Piece> pieces,
            IEnumerable<int> turnCycle)
        {
            Players = players.ToImmutableList();
            Factions = factions.ToImmutableList();
            Pieces = pieces.ToImmutableList();
            TurnCycle = turnCycle.ToImmutableList();
        }

        public static GameState Create(
            IEnumerable<Player> players, 
            IEnumerable<Faction> factions,
            IEnumerable<Piece> pieces, 
            IEnumerable<int> turnCycle) =>
            new GameState(players, factions, pieces, turnCycle);

        public static GameState Empty { get; } = 
            new GameState(
                Enumerable.Empty<Player>(), 
                Enumerable.Empty<Faction>(), 
                Enumerable.Empty<Piece>(), 
                Enumerable.Empty<int>());
    }
}
