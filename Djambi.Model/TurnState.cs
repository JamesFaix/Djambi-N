using System.Collections.Generic;
using System.Collections.Immutable;

namespace Djambi.Model
{
    public class TurnState
    {
        public TurnStatus Status { get; }

        public ImmutableList<Location> Selections { get; }

        public bool SelectionRequired { get; }

        private TurnState(
            TurnStatus status,
            IEnumerable<Location> selections,
            bool selectionRequired)
        {
            Status = status;
            Selections = selections.ToImmutableList();
            SelectionRequired = selectionRequired;
        }

        public static TurnState Create(
            TurnStatus status,
            IEnumerable<Location> selections,
            bool selectionRequired) => 
            new TurnState(status, selections, selectionRequired);
    }
}
