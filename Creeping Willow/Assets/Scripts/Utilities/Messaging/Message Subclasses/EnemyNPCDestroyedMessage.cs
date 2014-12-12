/**
 * This message can be used to tell when an axeman has killed the player
 **/
using UnityEngine;
public class EnemyNPCDestroyedMessage : Message
{
	public readonly GameObject NPC;
	
	public EnemyNPCDestroyedMessage (GameObject NPC):base(MessageType.EnemyNPCDestroyed)
	{
		this.NPC = NPC;
	}
}