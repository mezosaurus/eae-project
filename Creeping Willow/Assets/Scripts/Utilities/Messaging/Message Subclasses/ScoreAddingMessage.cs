using UnityEngine;
using System.Collections;

public class ScoreAddingMessage : Message
{
	public readonly bool adding;
	
	public ScoreAddingMessage(bool added) : base(MessageType.ScoreAdding)
	{
		adding = added;
	}
}
