/**
 * This message can be used to tell when an NPC has been eaten
 **/
using UnityEngine;
public class NPCEatenMessage : Message
{
	public readonly GameObject NPC;
	
	public NPCEatenMessage (GameObject NPC):base(MessageType.NPCEaten)
	{
		this.NPC = NPC;
	}
}
