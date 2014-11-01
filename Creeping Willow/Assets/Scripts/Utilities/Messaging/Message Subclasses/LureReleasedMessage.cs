using UnityEngine;

public class LureReleasedMessage : Message
{
	public Lure Lure;
	public GameObject NPC;
	public LureReleasedMessage (Lure lure, GameObject npc):base(MessageType.LureReleased)
	{
		Lure = lure;
		NPC = npc;
	}
}
