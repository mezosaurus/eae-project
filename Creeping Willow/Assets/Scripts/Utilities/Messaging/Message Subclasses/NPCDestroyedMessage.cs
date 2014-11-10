/**
 * This message can be used to tell when an NPC has been destroyed
 **/
using UnityEngine;
public class NPCDestroyedMessage : Message
{
	public readonly GameObject NPC;

	public NPCDestroyedMessage (GameObject NPC):base(MessageType.NPCDestroyed)
	{
		this.NPC = NPC;
	}
}