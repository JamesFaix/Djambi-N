using System.Collections.Generic;
using System.Collections.Immutable;

namespace Djambi.Model
{
    public class Turn
    {
        public TurnState State { get; }

        public ImmutableList<Location> Selections { get; }

        public bool SelectionRequired { get; }

        private Turn(
            TurnState state,
            IEnumerable<Location> selections,
            bool selectionRequired)
        {
            State = state;
            Selections = selections.ToImmutableList();
            SelectionRequired = selectionRequired;
        }

        public static Turn Create(
            TurnState state,
            IEnumerable<Location> selections,
            bool selectionRequired) => 
            new Turn(state, selections, selectionRequired);
    }
}
