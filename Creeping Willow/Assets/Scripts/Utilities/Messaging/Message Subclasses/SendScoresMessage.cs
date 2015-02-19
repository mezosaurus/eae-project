using UnityEngine;
using System.Collections;

public class SendScoresMessage : Message
{
	public readonly int score;
	public readonly string levelName;
	
	public SendScoresMessage(int newScore, string level) : base(MessageType.SendScore)
	{
		score = newScore;
		levelName = level;
	}
}
