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

        public ImmutableList<string> Log { get; }

        public ImmutableDictionary<Location, Piece> PiecesIndexedByLocation { get; }

        private GameState(
            IEnumerable<Player> players, 
            IEnumerable<Piece> pieces,
            IEnumerable<int> turnCycle,
            IEnumerable<string> log)
        {
            Players = players.ToImmutableList();
            Pieces = pieces.ToImmutableList();
            TurnCycle = turnCycle.ToImmutableList();
            Log = log.ToImmutableList();
            PiecesIndexedByLocation = Pieces.ToImmutableDictionary(p => p.Location, p => p);
        }

        public static GameState Create(
            IEnumerable<Player> players, 
            IEnumerable<Piece> pieces, 
            IEnumerable<int> turnCycle,
            IEnumerable<string> log) =>
            new GameState(players, pieces, turnCycle, log);

        public static GameState Empty { get; } = 
            new GameState(
                Enumerable.Empty<Player>(), 
                Enumerable.Empty<Piece>(), 
                Enumerable.Empty<int>(),
                Enumerable.Empty<string>());
    }
}
