using System;
using System.Windows.Input;
using Djambi.Engine;
using Djambi.Engine.Extensions;
using Djambi.Model;

namespace Djambi.UI
{
    internal class CellClickedCommand : ICommand
    {
        public GamePage Page { get; }

        public Location Location { get; }

        public CellClickedCommand(GamePage page, Location location)
        {
            Page = page;
            Location = location;
        }

        public event EventHandler CanExecuteChanged;

        public bool CanExecute(object parameter) => true;

        public void Execute(object parameter)
        {
            var state = Controller.TurnState.Status;
            switch (state)
            {
                case TurnStatus.AwaitingSelection:
                    Controller.MakeSelection(Location)
                        .OnValue(_ => Page.OnSelectionMade())
                        .OnError(error =>
                        {
                            //TODO: Display error somehow
                            throw new Exception("Whoops");
                        });
                    break;

                case TurnStatus.AwaitingConfirmation:
                case TurnStatus.Paused:
                case TurnStatus.Error:
                    break; //Do nothing; you can't click cells or pieces in these states

                default:
                    throw new Exception($"Invalid {nameof(TurnStatus)} ({state})");
            }
        }
    }
}
