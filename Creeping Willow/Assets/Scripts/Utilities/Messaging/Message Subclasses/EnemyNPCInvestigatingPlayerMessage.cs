using System;
using UnityEngine;

public class EnemyNPCInvestigatingPlayerMessage : Message
{
	public readonly GameObject NPC;

	public EnemyNPCInvestigatingPlayerMessage (GameObject NPC) : base(MessageType.EnemyNPCInvestigatingPlayer)
	{
		this.NPC = NPC;
	}
}

