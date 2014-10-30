using System.Collections.Generic;
using UnityEngine;

public class PlayerGrabbedNPCsMessage : Message
{
    public IList<GameObject> NPCs;

    public PlayerGrabbedNPCsMessage(IList<GameObject> npcs) : base(MessageType.PlayerGrabbedNPCs)
    {
        NPCs = npcs;
    }
}
