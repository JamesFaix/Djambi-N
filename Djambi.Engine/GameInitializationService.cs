using System;
using System.Collections.Generic;
using System.Linq;
using Djambi.Model;

namespace Djambi.Engine
{
    public class GameInitializationService
    {
        private readonly ValidationService _validator = new ValidationService();

        private readonly string[] _virtualPlayerNames = new[]
        {
            "Yoshi",
            "Wario",
            "Peach",
            "Bowser",
            "Toad",
            "Kirby",
            "Donkey Kong"
        };

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
            var players = CreatePlayers(playerNames);

            //Shuffle playerIds 
            players = players.Shuffle().ToList();
            players = players.Select((p, id) => p.SetId(id + 1)).ToList();
            
            //Create pieces
            var pieces = players
                .SelectMany(p => GetPlayerPieces(p.Id))
                .ToList();
            
            //Create random turn cycle
            var turnCycle = players
                .Select(p => p.Id)
                .Shuffle();

            return GameState.Create(players, pieces, turnCycle);
        }

        private List<Player> CreatePlayers(List<string> nonVirtualPlayerNames)
        {
            var namesWithSimplifiedNames = nonVirtualPlayerNames
                .Select(name => new
                {
                    Name = name,
                    Simplified = _validator.GetSimplifiedPlayerName(name)
                })
                .ToList();

            var virtualPlayernamesWithSimplifiedNames = _virtualPlayerNames
                .Select(name => new
                {
                    Name = name,
                    Simplified = _validator.GetSimplifiedPlayerName(name)
                })
                .ToList();

            var usableVirtualPlayerNames = virtualPlayernamesWithSimplifiedNames
                .Where(virtualNameWithSimplified => !namesWithSimplifiedNames
                    .Any(nameWithSimplified => nameWithSimplified.Simplified == virtualNameWithSimplified.Simplified))
                .Select(virtualNameWithSimplified => virtualNameWithSimplified.Name)
                .Shuffle()
                .ToList();

            var colors = EnumUtility.GetValues<PlayerColor>()
                .Except(new[] { PlayerColor.Dead })
                .Shuffle()
                .ToList();

            var playerIds = Enumerable.Range(1, Constants.MaxPlayerCount)
                .Shuffle()
                .ToList();

            var players = new List<Player>();

            while(nonVirtualPlayerNames.Count > 0)
            {
                players.Add(Player.Create(
                    playerIds[0],
                    nonVirtualPlayerNames[0],
                    isAlive: true,
                    isVirtual: false,
                    color: colors[0],
                    conqueredPlayerIds: Enumerable.Empty<int>()));

                playerIds.RemoveAt(0);
                nonVirtualPlayerNames.RemoveAt(0);
                colors.RemoveAt(0);
            }
            
            while (players.Count < Constants.MaxPlayerCount)
            {
                players.Add(Player.Create(
                    playerIds[0],
                    usableVirtualPlayerNames[0], 
                    isAlive: false, 
                    isVirtual: true,
                    color: colors[0],
                    conqueredPlayerIds: Enumerable.Empty<int>()));

                playerIds.RemoveAt(0);
                usableVirtualPlayerNames.RemoveAt(0);
                colors.RemoveAt(0);
            }

            return players;
        }

        private IEnumerable<Piece> GetPlayerPieces(int playerId)
        {
            /*
             * Start each player's pieces at locations from -1 to 1 in X and Y dimensions.
             * This makes reflecting the configuration on the X and Y axis as simple as flipping +/-.
             * After reflecting, offset to the right locations.
             */

            IEnumerable<Piece> pieces = new[] {
                Piece.Create(NextPieceId, PieceType.Chief,       playerId, playerId, true, Location.Create(-1,-1)),
                Piece.Create(NextPieceId, PieceType.Assassin,    playerId, playerId, true, Location.Create( 0,-1)),
                Piece.Create(NextPieceId, PieceType.Diplomat,    playerId, playerId, true, Location.Create( 0, 0)),
                Piece.Create(NextPieceId, PieceType.Necromobile, playerId, playerId, true, Location.Create( 1, 1)),
                Piece.Create(NextPieceId, PieceType.Reporter,    playerId, playerId, true, Location.Create(-1, 0)),
                Piece.Create(NextPieceId, PieceType.Militant,    playerId, playerId, true, Location.Create( 1,-1)),
                Piece.Create(NextPieceId, PieceType.Militant,    playerId, playerId, true, Location.Create( 1, 0)),
                Piece.Create(NextPieceId, PieceType.Militant,    playerId, playerId, true, Location.Create(-1, 1)),
                Piece.Create(NextPieceId, PieceType.Militant,    playerId, playerId, true, Location.Create( 0, 1)),
            };

            Piece MovePiece(Piece piece, Location location) =>
                Piece.Create(piece.Id, piece.Type, piece.PlayerId, piece.OriginalPlayerId, piece.IsAlive, location);

            IEnumerable<Piece> InvertX(IEnumerable<Piece> seq) =>
                seq.Select(p => MovePiece(p, Location.Create(-p.Location.X, p.Location.Y)));

            IEnumerable<Piece> InvertY(IEnumerable<Piece> seq) =>
                seq.Select(p => MovePiece(p, Location.Create(p.Location.X, -p.Location.Y)));

            IEnumerable<Piece> Offset(IEnumerable<Piece> seq, int x, int y) =>
                seq.Select(p => MovePiece(p, p.Location.Offset(x, y)));

            switch (playerId)
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

                default:
                    throw new ArgumentOutOfRangeException($"PlayerId must be between 1 and {Constants.MaxPlayerCount}.", nameof(playerId));
            }

            return pieces;
        }        
    }
}
