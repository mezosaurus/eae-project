using System;
using UnityEngine;

public class PossessorSpawnedMessage : Message
{
	public readonly GameObject Possessor;

	public PossessorSpawnedMessage (GameObject Possessor) : base(MessageType.PossessorSpawned)
	{
		this.Possessor = Possessor;
	}
}

