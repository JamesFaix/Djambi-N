using System.Collections.Generic;
using System.Collections.Immutable;

namespace Djambi.Model
{
    public class LegalMoves
    {
        public ImmutableList<Location> Destinations { get; }

        public ImmutableList<int> TargetPieceIds { get; }

        private LegalMoves(
            IEnumerable<Location> destinations,
            IEnumerable<int> targetPieceIds)
        {
            Destinations = destinations.ToImmutableList();
            TargetPieceIds = targetPieceIds.ToImmutableList();
        }

        public static LegalMoves Create(
            IEnumerable<Location> destinations,
            IEnumerable<int> targetPieceIds) =>
            new LegalMoves(destinations, targetPieceIds);
    }
}
