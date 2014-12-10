using UnityEngine;
using System.Collections;

public class ScoreChangedMessage : Message
{
	public readonly int[] highScores;
	public readonly string[] playerNames;
	
	public ScoreChangedMessage(int[] scores, string[] names) : base(MessageType.ScoreChanged)
	{
		highScores = scores;
		playerNames = names;
	}
}
