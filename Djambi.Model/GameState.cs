using System.Collections.Generic;
using System.Collections.Immutable;
using System.Linq;

namespace Djambi.Model
{
    public class GameState
    {
        public ImmutableList<Player> Players { get; }

        public ImmutableList<Piece> Pieces { get; }
        
        public ImmutableList<int> TurnCycle { get; }

        private GameState(
            IEnumerable<Player> players, 
            IEnumerable<Piece> pieces,
            IEnumerable<int> turnCycle)
        {
            Players = players.ToImmutableList();
            Pieces = pieces.ToImmutableList();
            TurnCycle = turnCycle.ToImmutableList();
        }

        public static GameState Create(
            IEnumerable<Player> players, 
            IEnumerable<Piece> pieces, 
            IEnumerable<int> turnCycle) =>
            new GameState(players, pieces, turnCycle);

        public static GameState Empty { get; } = 
            new GameState(
                Enumerable.Empty<Player>(), 
                Enumerable.Empty<Piece>(), 
                Enumerable.Empty<int>());
    }
}
