/**
 * This message can be used to tell when an axeman has killed the player
 **/
using UnityEngine;
public class PlayerKilledMessage : Message
{
	public readonly GameObject NPC;

	public PlayerKilledMessage (GameObject NPC):base(MessageType.PlayerKilled)
	{
		this.NPC = NPC;
	}
}