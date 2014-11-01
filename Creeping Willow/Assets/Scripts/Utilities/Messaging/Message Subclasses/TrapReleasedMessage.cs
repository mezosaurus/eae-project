using UnityEngine;

public class TrapReleasedMessage : Message
{
	public Trap Trap;
	public GameObject NPC;

	public TrapReleasedMessage (Trap trap, GameObject npc):base(MessageType.TrapReleased)
	{
		Trap = trap;
		NPC = npc;
	}
}
