using System;
using UnityEngine;

public class PossessorDestroyedMessage : Message
{
	public readonly GameObject Possessor;

	public PossessorDestroyedMessage (GameObject Possessor) : base(MessageType.PossessorDestroyed)
	{
		this.Possessor = Possessor;
	}
}

