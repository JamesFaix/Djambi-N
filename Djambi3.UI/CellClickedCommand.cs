using System;
using System.Windows.Input;
using Djambi.Engine;
using Djambi.Model;

namespace Djambi.UI
{
    internal class CellClickedCommand : ICommand
    {
        public Location Location { get; }

        public CellClickedCommand(Location location)
        {
            Location = location;
        }

        public event EventHandler CanExecuteChanged;

        public bool CanExecute(object parameter) => true;

        public void Execute(object parameter)
        {
            var state = StateManager.TurnState.Status;
            switch (state)
            {
                case TurnStatus.AwaitingSelection:
                    StateManager.MakeSelection(Location)
                        .OnValue(_ =>
                        {
                            //Draw selection
                            //Query current state
                        })
                        .OnError(error => 
                        { 
                            //Inform the user of an invalid request or something
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
