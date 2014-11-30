/**
 * This message can be used to tell when an NPC has been destroyed
 **/
using UnityEngine;
public class NPCDestroyedMessage : Message
{
	public readonly GameObject NPC;
	public readonly bool eaten = false;

	public NPCDestroyedMessage (GameObject NPC, bool wasEaten = true):base(MessageType.NPCDestroyed)
	{
		this.NPC = NPC;
		eaten = wasEaten;
	}
}