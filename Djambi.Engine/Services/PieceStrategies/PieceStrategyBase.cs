using System.Collections.Generic;
using System.Collections.Immutable;
using System.Linq;
using Djambi.Engine.Extensions;
using Djambi.Model;

namespace Djambi.Engine.Services.PieceStrategies
{
    abstract class PieceStrategyBase : IPieceStrategy
    {
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

        protected LocationWithPiece GetLocationWithPiece(Location location, ImmutableDictionary<Location, Piece> index) =>
            new LocationWithPiece(location, index.TryGetValue(location, out var p) ? p : null);

        protected Selection CreateSelection(LocationWithPiece lwp)
        {
            if (lwp.Piece == null)
            {
                return Selection.Create(
                    lwp.Location,
                    SelectionType.MoveDestination,
                    $"Move to {lwp.Location}.");
            }
            else
            {
                return Selection.Create(
                    lwp.Location,
                    SelectionType.MoveDestinationWithTarget,
                    $"Move to {lwp.Location} and target {lwp.Piece.Type}.");
            }
        }

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
