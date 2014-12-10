using System;
using UnityEngine;

public class PossessorSpawnedMessage : Message
{
	public readonly Possessor Possessor;

	public PossessorSpawnedMessage (Possessor Possessor) : base(MessageType.PossessorSpawned)
	{
		this.Possessor = Possessor;
	}
}

