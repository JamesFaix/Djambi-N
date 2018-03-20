namespace Djambi.Model
{
    public enum InteractionState
    {
        AwaitingSubjectSelection = 1,
        AwaitingDestinationSelection,
        AwaitingTargetSelection,
        AwaitingConfirmation,
        Paused,
        Error
    }
}
