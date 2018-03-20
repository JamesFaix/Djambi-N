using System.Collections.Generic;
using System.Linq;
using Djambi.Model;

namespace Djambi.Engine
{
    public class ModelFactory
    {
        private readonly Validator _validator = new Validator();

        private int _lastPieceId = 0;
        private int NextPieceId => ++_lastPieceId;
        
        public Result<GameState> InitializeGame(IEnumerable<string> playerNames)
        {
            var nameList = playerNames.ToList();

            return _validator.ValidatePlayerNames(nameList)
                .Map(_ => CreateInitialGameState(nameList))
                .Bind(state => _validator.ValidateGameState(state)
                    .Map(_ => state));
        }
        
        private GameState CreateInitialGameState(List<string> playerNames)
        {
            var factions = Enumerable.Range(1, Constants.FactionCount)
                .Select(n => Faction.Create(n))
                .ToList();

            var players = playerNames
                .Select((name, n) => Player.Create(n + 1, name, true, Enumerable.Empty<int>()))
                .ToList();

            var pieces = factions
                .SelectMany(f => GetFactionPieces(f.Id))
                .ToList();

            //Assign a faction to each player randomly
            var shuffledFactions = factions.Shuffle().ToList();
            for (var i = 0; i < players.Count; i++)
            {
                players[i] = players[i].AddFaction(shuffledFactions[i].Id);
            }

            //Create random turn cycle
            var turnCycle = players
                .Select(p => p.Id)
                .Shuffle();

            return GameState.Create(players, factions, pieces, turnCycle);
        }

        private IEnumerable<Piece> GetFactionPieces(int factionId)
        {
            IEnumerable<Piece> pieces = new[] {
                Piece.Create(NextPieceId, PieceType.Chief, factionId, true, Location.Create(-1,-1)),
                Piece.Create(NextPieceId, PieceType.Assassin, factionId, true, Location.Create(0,-1)),
                Piece.Create(NextPieceId, PieceType.Diplomat, factionId, true, Location.Create(0,0)),
                Piece.Create(NextPieceId, PieceType.Necromobile, factionId, true, Location.Create(1,1)),
                Piece.Create(NextPieceId, PieceType.Reporter, factionId, true, Location.Create(-1,0)),
                Piece.Create(NextPieceId, PieceType.Militant, factionId, true, Location.Create(1,-1)),
                Piece.Create(NextPieceId, PieceType.Militant, factionId, true, Location.Create(1,0)),
                Piece.Create(NextPieceId, PieceType.Militant, factionId, true, Location.Create(-1,1)),
                Piece.Create(NextPieceId, PieceType.Militant, factionId, true, Location.Create(0,1)),
            };

            Piece MovePiece(Piece piece, Location location) =>
                Piece.Create(piece.Id, piece.Type, piece.Faction, piece.IsAlive, location);

            IEnumerable<Piece> InvertX(IEnumerable<Piece> seq) =>
                seq.Select(p => MovePiece(p, Location.Create(-p.Location.X, p.Location.Y)));

            IEnumerable<Piece> InvertY(IEnumerable<Piece> seq) =>
                seq.Select(p => MovePiece(p, Location.Create(p.Location.X, -p.Location.Y)));

            IEnumerable<Piece> Offset(IEnumerable<Piece> seq, int x, int y) =>
                seq.Select(p => MovePiece(p, p.Location.Offset(x, y)));

            switch (factionId)
            {
                case 1:
                    pieces = Offset(pieces, 2, 2);
                    break;

                case 2:
                    pieces = Offset(InvertX(pieces), 8, 2);
                    break;

                case 3:
                    pieces = Offset(InvertY(pieces), 2, 8);
                    break;

                case 4:
                    pieces = Offset(InvertY(InvertX(pieces)), 8, 8);
                    break;
            }

            return pieces;
        }        
    }
}
