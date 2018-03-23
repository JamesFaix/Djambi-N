using System.Collections.Generic;
using System.Collections.Immutable;
using System.Linq;
using Djambi.Engine.Extensions;
using Djambi.Engine.Services;
using Djambi.Model;

namespace Djambi.Engine.PieceStrategies
{
    abstract class PieceStrategyBase : IPieceStrategy
    {
        protected readonly GameUpdateService _gameUpdateService;

        public PieceStrategyBase(GameUpdateService gameUpdateService)
        {
            _gameUpdateService = gameUpdateService;
        }

        public abstract Result<IEnumerable<Selection>> GetAdditionalSelections(GameState game, Piece piece, TurnState turn);

        public abstract Result<IEnumerable<Selection>> GetMoveDestinations(GameState game, Piece piece);

        public abstract TurnState GetNextTurnState(GameState game, TurnState turn, Selection newSelection);

        protected IEnumerable<Location> GetColinearNonBlockedLocations(Piece piece, GameState game)
        {
            foreach (var dir in EnumUtility.GetValues<Directions>())
            {
                var loc = piece.Location.Offset(dir, 1);

                //TODO: Refactor to not use GOTO
                next:
                if (!loc.IsValid())
                {
                    continue;
                }
                if (game.Pieces.Any(p => p.Location == loc))
                {
                    yield return loc;
                    continue;
                }
                else
                {
                    yield return loc;
                    loc = loc.Offset(dir, 1);
                    goto next;
                }
            }
        }

        protected IEnumerable<Location> GetEmptyLocations(GameState game)
        {
            return Enumerable.Range(1, Constants.BoardSize)
                .SelectMany(x => Enumerable.Range(1, Constants.BoardSize)
                    .Select(y => Location.Create(x, y)))
                .Except(game.PiecesIndexedByLocation.Keys);
        }

        protected LocationWithPiece GetLocationWithPiece(Location location, ImmutableDictionary<Location, Piece> index) =>
            new LocationWithPiece(location, index.TryGetValue(location, out var p) ? p : null);

        protected Selection CreateSelection(LocationWithPiece lwp) =>
            lwp.Piece == null
                ? Selection.Move(lwp.Location)
                : Selection.MoveWithTarget(lwp.Location, lwp.Piece);

        protected class LocationWithPiece
        {
            public Location Location { get; }

            public Piece Piece { get; }

            public LocationWithPiece(Location location, Piece piece)
            {
                Location = location;
                Piece = piece;
            }
        }
    }
}
