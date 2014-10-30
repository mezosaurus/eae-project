using UnityEngine;
using System.Collections.Generic;

public class PlayerReleasedNPCsMessage : Message
{
    public IList<GameObject> NPCs;

    public PlayerReleasedNPCsMessage(IList<GameObject> npcs) : base(MessageType.PlayerReleasedNPCs)
    {
        NPCs = npcs;
    }
}
