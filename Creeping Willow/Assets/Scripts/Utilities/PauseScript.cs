using UnityEngine;
using System.Collections;

public class PauseScript : MonoBehaviour
{
	public Canvas PauseCanvas;
	private bool isPaused;

	void Start()
	{
		isPaused = false;

		PauseCanvas = GameObject.Find( "PauseCanvas" ).GetComponent<Canvas>();
	}

	void Update()
	{
		if( isPaused )
		{
			// Exit pause screen
			if( Input.GetButtonDown( "Start" ) || Input.GetButtonDown( "B" ) )
			{
				isPaused = false;
				PauseCanvas.enabled = false;
				MessageCenter.Instance.Broadcast( new PauseChangedMessage( false ) );
			}
		}
		else
		{
			// Pause game
			if( Input.GetButtonDown( "Start" ) )
			{
				isPaused = true;
				PauseCanvas.enabled = true;
				MessageCenter.Instance.Broadcast( new PauseChangedMessage( true ) );
			}
		}
	}
}
