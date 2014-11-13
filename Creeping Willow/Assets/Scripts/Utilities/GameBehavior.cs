using UnityEngine;
using System.Collections;

public class GameBehavior : MonoBehaviour
{
	protected bool paused = false;
	bool skipFrame = false;

	/// <summary>
	/// The system time at the beginning of the update call.  Only use in Gameupdate!  Do not modify!
	/// </summary>
	protected float g_currentTime = 0.0f;

	/// <summary>
	/// The system time at the end of the previous update call.  Only use in Gameupdate!  Do not modify!
	/// </summary>
	protected float g_previousTime = 0.0f;

	void Awake()
	{
		MessageCenter.Instance.RegisterListener (MessageType.PauseChanged, HandlePauseChanged);
	}

	private void Update()
	{
		g_currentTime = Time.time;

		// Check to see if paused
		if( !paused )
		{
			// This is used to invalidate any getButtonDown inputs from exiting the pause menu
			if( skipFrame ) 
				skipFrame = false;
			else
				GameUpdate();
		}

		g_previousTime = g_currentTime;
		}
	}

	protected virtual void GameUpdate() { }

	private void OnDestroy()
	{
		MessageCenter.Instance.UnregisterListener(MessageType.AbilityStatusChanged, HandlePauseChanged);
	}

	protected void HandlePauseChanged(Message message)
	{
		PauseChangedMessage mess = message as PauseChangedMessage;

		paused = mess.isPaused;

		if( paused == false )
			skipFrame = true;
	}
}
