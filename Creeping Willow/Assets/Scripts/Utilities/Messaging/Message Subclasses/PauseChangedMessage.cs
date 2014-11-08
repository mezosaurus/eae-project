using UnityEngine;
using System.Collections;

public class PauseChangedMessage : Message {

	public readonly bool isPaused;
	
	public PauseChangedMessage(bool paused) : base(MessageType.PauseChanged)
	{
		isPaused = paused;
	}
}
