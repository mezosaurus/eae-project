using UnityEngine;
using System.Collections;

public class ScoreChangedMessage : Message
{
	public readonly int addedScore;
	public readonly int multiplier;
	
	public ScoreChangedMessage(int score, int multi) : base(MessageType.ScoreChanged)
	{
		addedScore = score;
		multiplier = multi;
	}
}
