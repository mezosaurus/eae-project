class AxeManMinigameTreeChangePhaseMessage : Message
{
    public string Phase;


    public AxeManMinigameTreeChangePhaseMessage(string phase) : base(MessageType.AxeManMinigameTreeChangePhase)
    {
        Phase = phase;
    }
}
