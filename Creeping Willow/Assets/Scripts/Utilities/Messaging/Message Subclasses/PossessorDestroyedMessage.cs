using System;
using UnityEngine;

public class PossessorDestroyedMessage : Message
{
	public readonly Possessor Possessor;

	public PossessorDestroyedMessage (Possessor Possessor) : base(MessageType.PossessorDestroyed)
	{
		this.Possessor = Possessor;
	}
}

