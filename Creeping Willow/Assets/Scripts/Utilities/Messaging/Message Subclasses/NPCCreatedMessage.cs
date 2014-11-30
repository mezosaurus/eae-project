/**
 * This message can be used to tell when an NPC has been created
 **/
using UnityEngine;
public class NPCCreatedMessage : Message
{
	public readonly GameObject NPC;
	
	public NPCCreatedMessage (GameObject NPC):base(MessageType.NPCCreated)
	{
		this.NPC = NPC;
	}
}
