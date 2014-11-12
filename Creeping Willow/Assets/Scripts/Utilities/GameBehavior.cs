using UnityEngine;
using System.Collections;

public class GameBehavior : MonoBehaviour
{
	protected bool paused = false;
	bool skipFrame = false;

	void Awake()
	{
		MessageCenter.Instance.RegisterListener (MessageType.PauseChanged, HandlePauseChanged);
	}

    private void Update()
    {
        // Check to see if paused
		if( !paused )
		{
			// This is used to invalidate any getButtonDown inputs from exiting the pause menu
			if( skipFrame ) 
				skipFrame = false;
			else
        		GameUpdate();
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
