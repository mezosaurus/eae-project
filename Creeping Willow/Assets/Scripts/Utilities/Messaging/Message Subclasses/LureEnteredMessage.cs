using UnityEngine;

public class LureEnteredMessage : Message {
	public readonly Lure Lure;
	public readonly GameObject NPC;

	public LureEnteredMessage(Lure lure, GameObject npc):base(MessageType.LureRadiusEntered){
		Lure = lure;
		NPC = npc;
	}
}
