using UnityEngine;

public class TrapEnteredMessage : Message {
	public readonly Trap Trap;
	public readonly GameObject NPC;

	public TrapEnteredMessage(Trap trap, GameObject npc):base(MessageType.TrapEntered){
		Trap = trap;
		NPC = npc;
	}
}
