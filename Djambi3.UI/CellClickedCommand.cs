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
            var state = StateManager.Interaction.State;
            switch (state)
            {
                case InteractionState.AwaitingSubjectSelection:
                    StateManager.SelectSubject(Location)
                        .OnValue(legalMoves =>
                        {
                            //Draw possible actions
                        })
                        .OnError(error => 
                        { 
                            //Inform the user of an invalid request or something
                        });
                    break;

                case InteractionState.AwaitingDestinationSelection:
                    StateManager.SelectDestination(Location)
                        .OnValue(legalMoves =>
                        {
                            //Draw possible actions
                        })
                        .OnError(error =>
                        {
                            //Inform the user of an invalid request or something
                        });
                    break;

                case InteractionState.AwaitingTargetSelection:
                    StateManager.SelectTarget(Location)
                        .OnValue(legalMoves =>
                        {
                            //Draw possible actions
                        })
                        .OnError(error =>
                        {
                            //Inform the user of an invalid request or something
                        });
                    break;

                case InteractionState.AwaitingConfirmation:
                case InteractionState.Paused:
                case InteractionState.Error:
                    break; //Do nothing; you can't click cells or pieces in these states

                default:
                    throw new Exception($"Invalid {nameof(InteractionState)} ({state})");
            }
        }
    }
}
