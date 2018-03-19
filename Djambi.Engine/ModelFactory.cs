using System;
using System.Collections.Generic;
using System.Linq;
using System.Text.RegularExpressions;
using Djambi.Model;

namespace Djambi.Engine
{
    public class ModelFactory
    {
        private int _lastPieceId = 0;
        private int NextPieceId => ++_lastPieceId;

        private static readonly Regex _validPlayerNameRegex = new Regex(@"^[A-Za-z0-9_\- ]$");
        
        public Result<GameState> InitializeGame(IEnumerable<string> playerNames)
        {
            return ValidatePlayerNames(playerNames)
                .Map(CreateInitialGameState)
                .Bind(ValidateGameState);
        }
        
        private Result<List<string>> ValidatePlayerNames(IEnumerable<string> playerNames)
        {
            if (playerNames == null)
            {
                return new ArgumentNullException(nameof(playerNames))
                    .ToErrorResult<List<string>>();
            }

            var nameList = playerNames.ToList();

            if (nameList.Count < Constants.MinPlayerCount || nameList.Count > Constants.MaxPlayerCount)
            {
                return new ArgumentException($"There must be between {Constants.MinPlayerCount} and {Constants.MaxPlayerCount} players.", nameof(playerNames))
                    .ToErrorResult<List<string>>();
            }

            var invalidNames = nameList.Where(name => !_validPlayerNameRegex.IsMatch(name)).ToList();

            if (invalidNames.Any())
            {
                return new AggregateException(
                    $"Player names must have only letters, numbers, - (dash), _ (underscore), and (space).", 
                    invalidNames.Select(name => new ArgumentException(name)))
                   .ToErrorResult<List<string>>();
            }
            
            var namesGroupedBySimilarity = nameList
                .Select(name => new
                {
                    Input = name,
                    Simplified = name.Replace(" ", "").ToUpperInvariant()
                })
                .GroupBy(name => name.Simplified);

            if (namesGroupedBySimilarity.Any(group => group.Count() > 1))
            {
                var invalidInputNames = namesGroupedBySimilarity
                        .Where(group => group.Count() > 1)
                        .SelectMany(group => group.Select(name => name.Input));

                return new AggregateException(
                    $"Player names cannot differ only by capitalization and whitespace.",
                    invalidInputNames.Select(name => new ArgumentException(name)))
                    .ToErrorResult<List<string>>();
            }

            return nameList.ToResult();
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

        private Result<GameState> ValidateGameState(GameState state)
        {
            var piecesOutOfBounds = state.Pieces
                .Where(p => !p.Location.IsValid())
                .ToList();

            if (piecesOutOfBounds.Any()){
                return new Exception($"The following pieces are in invalid locations.\n" + 
                    string.Join("\n", piecesOutOfBounds.Select(p => $"{p.Type} {p.Location}")))
                    .ToErrorResult<GameState>();
            }

            return state.ToResult();
        }
    }
}
