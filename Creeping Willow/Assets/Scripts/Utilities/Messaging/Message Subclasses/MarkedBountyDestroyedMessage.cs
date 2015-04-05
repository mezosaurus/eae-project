/**
 * This message can be used to tell when a marked bounty npc is destroyed by being eaten or escaping
 **/
using UnityEngine;
public class MarkedBountyDestroyedMessage : Message
{
	public MarkedBountyDestroyedMessage ():base(MessageType.MarkedBountyDestroyed)
	{

	}
}
