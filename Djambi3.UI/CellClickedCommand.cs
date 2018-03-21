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
            var state = StateManager.Turn.State;
            switch (state)
            {
                case TurnState.AwaitingSelection:
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

                case TurnState.AwaitingConfirmation:
                case TurnState.Paused:
                case TurnState.Error:
                    break; //Do nothing; you can't click cells or pieces in these states

                default:
                    throw new Exception($"Invalid {nameof(TurnState)} ({state})");
            }
        }
    }
}
