using UnityEngine;

class AxeManMinigameTreeChangePhaseMessage : Message
{
    public GameObject Tree;
    public string Phase;


    public AxeManMinigameTreeChangePhaseMessage(GameObject tree, string phase) : base(MessageType.AxeManMinigameTreeChangePhase)
    {
        Tree = tree;
        Phase = phase;
    }
}
