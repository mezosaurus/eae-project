/**
 * This message can be used to tell when an axeman has killed the player
 **/
using UnityEngine;
public class PlayerKilledMessage : Message
{
    public readonly GameObject NPC, Tree;

	public PlayerKilledMessage (GameObject npc, GameObject tree):base(MessageType.PlayerKilled)
	{
        NPC = npc;
        Tree = tree;
	}
}