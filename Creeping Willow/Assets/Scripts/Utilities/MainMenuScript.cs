using UnityEngine;
using System.Collections;

public class MainMenuScript : MonoBehaviour {

	public enum MenuState
	{
		PLAY,
		QUIT
	}

	int state;
	bool stateChanged;
	bool controllerInUse;

	TextMesh playText;
	TextMesh quitText;

	// Use this for initialization
	void Start () {
		state = (int)MenuState.PLAY;
		stateChanged = true;
		controllerInUse = false;

		playText = GameObject.Find ("PlayText").GetComponent<TextMesh>();
		quitText = GameObject.Find ("QuitText").GetComponent<TextMesh>();
	}
	
	// Update is called once per frame
	void Update () {

		// wait for controller to rest
		if( controllerInUse && Input.GetAxis("LeftStickY") == 0)
			controllerInUse = false;

		if( controllerInUse && (Input.GetAxis("LeftStickY") < 0 || Input.GetAxis("LeftStickY") > 0) )
			return;

		// check for change
		if( Input.GetAxis("LeftStickY") < 0 || Input.GetKeyDown(KeyCode.DownArrow) )
		{
			if( state == (int)MenuState.PLAY )
				state = (int)MenuState.QUIT;

			stateChanged = true;
			controllerInUse = true;
		}

		if( Input.GetAxis("LeftStickY") > 0 || Input.GetKeyDown(KeyCode.UpArrow) )
		{
			if( state == (int)MenuState.QUIT )
				state = (int)MenuState.PLAY;

			stateChanged = true;
			controllerInUse = true;
		}

		// check for input
		if( Input.GetKeyDown(KeyCode.Return) || Input.GetButtonDown("A") )
		{
			if( state == (int)MenuState.PLAY )
				Application.LoadLevel(Application.loadedLevel+1);
			else if( state == (int)MenuState.QUIT )
				Application.Quit();
		}


		if( stateChanged )
			updateText();
	}

	void updateText()
	{
		if( state == (int)MenuState.PLAY )
		{
			playText.color = Color.red;
			quitText.color = Color.green;
		}
		else if( state == (int)MenuState.QUIT )
		{
			playText.color = Color.green;
			quitText.color = Color.red;
		}
		stateChanged = false;
	}
}
