using System.Collections.Generic;
using System.Collections.Immutable;
using System.Linq;

namespace Djambi.Model
{
    public class TurnState
    {
        public TurnStatus Status { get; }

        public ImmutableList<Selection> Selections { get; }
        
        private TurnState(
            TurnStatus status,
            IEnumerable<Selection> selections)
        {
            Status = status;
            Selections = selections.ToImmutableList();
        }

        public static TurnState Create(
            TurnStatus status,
            IEnumerable<Selection> selections) => 
            new TurnState(status, selections);

        public static TurnState Empty { get; } =
            new TurnState(
                TurnStatus.AwaitingSelection,
                Enumerable.Empty<Selection>());

        public static TurnState MainMenu { get; } =
            new TurnState(
                TurnStatus.Paused,
                Enumerable.Empty<Selection>());
    }
}
