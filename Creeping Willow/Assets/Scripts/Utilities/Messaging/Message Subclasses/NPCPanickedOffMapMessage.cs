/**
 * This message can be used to tell when an axeman has killed the player
 **/
using UnityEngine;
public class NPCPanickedOffMapMessage : Message
{
	public readonly Vector3 PanickedPosition;
	
	public NPCPanickedOffMapMessage (Vector3 panickedPosition):base(MessageType.NPCPanickedOffMap)
	{
		PanickedPosition = panickedPosition;
	}
}