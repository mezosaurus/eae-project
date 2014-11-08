using UnityEngine;
using System.Collections;

public class PauseScript : MonoBehaviour {

	public enum PauseState
	{
		RESUME,
		QUIT
	}

	bool isPaused;
	public GUITexture pauseScreen;


	// Use this for initialization
	void Start () {
		isPaused = false;
		pauseScreen.enabled = false;
	}
	
	// Update is called once per frame
	void Update () {

		if( isPaused )
		{
			if( Input.GetButtonDown("Start") || Input.GetButtonDown("B") || Input.GetKeyDown(KeyCode.P))
			{
				isPaused = false;
				pauseScreen.enabled = false;
				MessageCenter.Instance.Broadcast(new PauseChangedMessage(false));
			}
		}
		else
		{
			if( Input.GetButtonDown("Start") || Input.GetKeyDown(KeyCode.P) )
			{
				isPaused = true;
				pauseScreen.enabled = true;
				MessageCenter.Instance.Broadcast(new PauseChangedMessage(true));
			}
		}

	}
}
