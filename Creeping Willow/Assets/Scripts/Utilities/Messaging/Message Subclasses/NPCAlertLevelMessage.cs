/**
 * This message can be used to tell when an NPC has been destroyed
 **/
using UnityEngine;
public enum AlertLevelType
{
	Normal,
	Alert,
	Panic
}
public class NPCAlertLevelMessage : Message
{
	public readonly GameObject NPC;
	public AlertLevelType alertLevelType;
	
	public NPCAlertLevelMessage (GameObject NPC, AlertLevelType alertLevelType):base(MessageType.NPCAlertLevel)
	{
		this.NPC = NPC;
		this.alertLevelType = alertLevelType;
	}
}