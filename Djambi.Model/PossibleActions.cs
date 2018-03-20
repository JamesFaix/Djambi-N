using System.Collections.Generic;
using System.Collections.Immutable;

namespace Djambi.Model
{
    public class PossibleActions
    {
        public ImmutableList<int> SubjectIds { get; }

        public ImmutableList<Location> Destinations { get; }

        public ImmutableList<int> TargetPieceIds { get; }

        public bool ConfirmationRequired { get; }

        public bool NoActions { get; }

        private PossibleActions(
            IEnumerable<int> subjectIds,
            IEnumerable<Location> destinations,
            IEnumerable<int> targetPieceIds,
            bool confirmationRequired,
            bool noActions)
        {
            SubjectIds = subjectIds.ToImmutableList();
            Destinations = destinations.ToImmutableList();
            TargetPieceIds = targetPieceIds.ToImmutableList();
            ConfirmationRequired = confirmationRequired;
            NoActions = noActions;
        }

        public static PossibleActions Create(
            IEnumerable<int> subjectIds,
            IEnumerable<Location> destinations,
            IEnumerable<int> targetPieceIds,
            bool confirmationRequired,
            bool noActions) =>
            new PossibleActions(subjectIds, destinations, targetPieceIds, confirmationRequired, noActions);
    }
}
