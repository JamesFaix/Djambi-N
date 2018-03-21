using System.Collections.Generic;
using System.Collections.Immutable;
using System.Linq;

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

        public static TurnState Empty { get; } =
            new TurnState(
                TurnStatus.AwaitingSelection,
                Enumerable.Empty<Selection>(),
                true);

        public static TurnState MainMenu { get; } =
            new TurnState(
                TurnStatus.Paused,
                Enumerable.Empty<Selection>(),
                false);
    }
}
