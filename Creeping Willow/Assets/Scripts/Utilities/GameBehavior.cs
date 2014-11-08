using UnityEngine;
using System.Collections;

public class GameBehavior : MonoBehaviour
{
	protected bool paused = false;

	void Awake()
	{
		MessageCenter.Instance.RegisterListener (MessageType.PauseChanged, HandlePauseChanged);
	}

    private void Update()
    {
        // Check to see if paused
		if( !paused )
        	GameUpdate();
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
	}
}
