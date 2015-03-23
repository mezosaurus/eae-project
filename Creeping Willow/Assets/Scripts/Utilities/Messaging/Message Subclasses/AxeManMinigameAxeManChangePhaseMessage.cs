class AxeManMinigameAxeManChangePhaseMessage : Message
{
    public uint Phase;


    public AxeManMinigameAxeManChangePhaseMessage(uint phase) : base(MessageType.AxeManMinigameAxeManChangePhase)
    {
        Phase = phase;
    }
}
