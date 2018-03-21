using System.Collections.Generic;
using System.Collections.Immutable;

namespace Djambi.Model
{
    public class TurnState
    {
        public TurnStatus Status { get; }

        public ImmutableList<Selection> Selections { get; }

        public bool SelectionRequired { get; }

        private TurnState(
            TurnStatus status,
            IEnumerable<Selection> selections,
            bool selectionRequired)
        {
            Status = status;
            Selections = selections.ToImmutableList();
            SelectionRequired = selectionRequired;
        }

        public static TurnState Create(
            TurnStatus status,
            IEnumerable<Selection> selections,
            bool selectionRequired) => 
            new TurnState(status, selections, selectionRequired);
    }
}
