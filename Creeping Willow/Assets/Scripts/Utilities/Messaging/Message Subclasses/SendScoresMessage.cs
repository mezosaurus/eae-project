using UnityEngine;
using System.Collections;

public class SendScoresMessage : Message
{
	public readonly int score;
	public readonly string levelName;
	public readonly string initials;
	
	public SendScoresMessage(int newScore, string name, string level) : base(MessageType.SendScore)
	{
		score = newScore;
		levelName = level;
		initials = name;
	}
}
