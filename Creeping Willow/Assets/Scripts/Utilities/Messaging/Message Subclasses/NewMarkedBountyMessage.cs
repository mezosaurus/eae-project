/**
 * This message can be used to tell when a marked bounty npc appears
 **/
using UnityEngine;
public class NewMarkedBountyMessage : Message
{
	public readonly GameObject NPC;
	
	public NewMarkedBountyMessage (GameObject NPC):base(MessageType.NewMarkedBounty)
	{
		this.NPC = NPC;
	}
}
