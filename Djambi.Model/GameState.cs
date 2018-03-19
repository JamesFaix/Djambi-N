using System.Collections.Generic;
using System.Collections.Immutable;

namespace Djambi.Model
{
    class GameState
    {
        public ImmutableList<Player> Players { get; }

        public ImmutableList<Piece> Pieces { get; }

        private GameState(IEnumerable<Player> players, IEnumerable<Piece> pieces)
        {
            Players = players.ToImmutableList();
            Pieces = pieces.ToImmutableList();
        }

        public static GameState Create(IEnumerable<Player> players, IEnumerable<Piece> pieces) =>
            new GameState(players, pieces);
    }
}
