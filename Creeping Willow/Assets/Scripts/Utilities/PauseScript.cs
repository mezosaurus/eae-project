using UnityEngine;
using System.Collections;

public class PauseScript : MonoBehaviour {

	public enum PauseState
	{
		RESUME,
		SETTINGS,
		QUIT,
		SETTINGS_IN
	}

	bool isPaused;
	int state;

	public GUITexture pauseScreenResume;
	public GUITexture pauseScreenQuit;
	public GUITexture pauseScreenSettings;
	public GUITexture pauseScreenSettingsIn;

	bool controllerInUse;

	// Use this for initialization
	void Start () {
		isPaused = false;
		state = -1;
		controllerInUse = false;

		pauseScreenResume.enabled = false;
		pauseScreenQuit.enabled = false;
		pauseScreenSettings.enabled = false;
		pauseScreenSettingsIn.enabled = false;
	}
	
	// Update is called once per frame
	void Update () {

		// wait for controller to be at rest
		if( controllerInUse && Input.GetAxis("LeftStickY") == 0)
			controllerInUse = false;
		
		if( controllerInUse && (Input.GetAxis("LeftStickY") < 0 || Input.GetAxis("LeftStickY") > 0) )
			return;

		if( isPaused )
		{
			// if in second layer of pause menu
			if( state == (int)PauseState.SETTINGS_IN )
			{
				if( Input.GetButtonDown("B") )
				{
					state = (int)PauseState.SETTINGS;
					pauseScreenSettingsIn.enabled = false;
					pauseScreenSettings.enabled = true;
				}
			}
			else
			{
				// if resume state
				if( state == (int)PauseState.RESUME )
				{
					if( Input.GetAxis("LeftStickY") < 0 || Input.GetKeyDown(KeyCode.DownArrow) )
					{
						state = (int)PauseState.SETTINGS;
						pauseScreenSettings.enabled = true;
						pauseScreenResume.enabled = false;
						controllerInUse = true;
					}
					else if( Input.GetButtonDown("A") )
					{
						isPaused = false;
						pauseScreenResume.enabled = false;

						MessageCenter.Instance.Broadcast(new PauseChangedMessage(false));
						state = -1;
					}
				}
				// settings state
				else if( state == (int)PauseState.SETTINGS )
				{
					if( Input.GetAxis("LeftStickY") < 0 || Input.GetKeyDown(KeyCode.DownArrow) )
					{
						state = (int)PauseState.QUIT;
						pauseScreenSettings.enabled = false;
						pauseScreenQuit.enabled = true;
						controllerInUse = true;
					}
					else if( Input.GetAxis("LeftStickY") > 0 || Input.GetKeyDown(KeyCode.UpArrow) )
					{
						state = (int)PauseState.RESUME;
						pauseScreenSettings.enabled = false;
						pauseScreenResume.enabled = true;
						controllerInUse = true;
					}
					else if( Input.GetButtonDown("A") )
					{
						state = (int)PauseState.SETTINGS_IN;
						pauseScreenSettings.enabled = false;
						pauseScreenSettingsIn.enabled = true;
					}
				}
				// quit state
				else if( state == (int)PauseState.QUIT )
				{
					if( Input.GetAxis("LeftStickY") > 0 || Input.GetKeyDown(KeyCode.UpArrow) )
					{
						state = (int)PauseState.SETTINGS;
						pauseScreenSettings.enabled = true;
						pauseScreenQuit.enabled = false;
						controllerInUse = true;
					}
					else if( Input.GetButtonDown("A") )
					{
						Application.LoadLevel(0);
					}
				}

				// check for exit of pause screen
				if( Input.GetButtonDown("Start") || Input.GetButtonDown("B") || Input.GetKeyDown(KeyCode.P))
				{
					isPaused = false;
					pauseScreenResume.enabled = false;
					pauseScreenQuit.enabled = false;
					pauseScreenSettings.enabled = false;
					
					MessageCenter.Instance.Broadcast(new PauseChangedMessage(false));
					state = -1;
				}
			}
		}
		else
		{
			// pause game
			if( Input.GetButtonDown("Start") || Input.GetKeyDown(KeyCode.P) )
			{
				isPaused = true;
				state = (int)PauseState.RESUME;
				pauseScreenResume.enabled = true;
				MessageCenter.Instance.Broadcast(new PauseChangedMessage(true));
			}
		}

	}
}
