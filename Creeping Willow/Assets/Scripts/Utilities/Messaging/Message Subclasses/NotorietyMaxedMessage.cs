using UnityEngine;

public class NotorietyMaxedMessage : Message
{
	public GameObject NPC;
	
	public NotorietyMaxedMessage( GameObject npc ) : base( MessageType.NotorietyMaxed )
	{
		NPC = npc;
	}
}
