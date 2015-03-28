using UnityEngine;
using System.Collections;
using UnityEngine.UI;
using UnityEngine.EventSystems;

public class IntroScript : MonoBehaviour
{
	public AudioClip levelMusic;
	public Button[] pauseButtons;

	private Canvas pauseCanvas;
	private bool isPaused;
	private bool axisBusy;

	private SoundManager soundManager;

	void Start()
	{
		// Pause the game
		isPaused = true;
		MessageCenter.Instance.Broadcast( new PauseChangedMessage( true ) );

		axisBusy = false;

		// Hide the cursor
		Screen.showCursor = false;
		Screen.lockCursor = true;

		// Get the canvas and buttons ready
		pauseCanvas = GameObject.Find( "IntroCanvas" ).GetComponent<Canvas>();
		pauseCanvas.enabled = true;

		EnableAllButtons();
		EventSystem.current.SetSelectedGameObject( pauseButtons[ 0 ].gameObject );

		// Disable the other scripts until we are out of the menu
		PauseScript pauseScript = GameObject.Find( "PauseCanvas" ).GetComponent<PauseScript>();
		pauseScript.enabled = false;

		OutroScript outroScript = GameObject.Find( "OutroCanvas" ).GetComponent<OutroScript>();
		outroScript.enabled = false;

		// Switch the music
		soundManager = GameObject.FindObjectOfType<SoundManager>();
		soundManager.ChangeMusic( levelMusic );
	}

	void Update()
	{
		if( isPaused )
		{
			// Check for mouse
			if( Input.GetAxis( "Mouse X" ) != 0 || Input.GetAxis( "Mouse Y" ) != 0 )
			{
				Screen.showCursor = true;
				Screen.lockCursor = false;
				axisBusy = true;
				EventSystem.current.SetSelectedGameObject( null );
			}
			// Check for controller movement
			else if( !axisBusy && ( Input.GetAxis( "LSX" ) != 0 || Input.GetAxis( "MenuX" ) != 0 ) )
			{
				Screen.showCursor = false;
				Screen.lockCursor = true;
				axisBusy = true;
				
				if( EventSystem.current.currentSelectedGameObject == null )
				{
					EventSystem.current.SetSelectedGameObject( pauseButtons[ 0 ].gameObject );
				}
			}
			// Exit pause screen
			else if(  !axisBusy && ( Input.GetButtonDown( "Start" ) || Input.GetButtonDown( "Back" ) || Input.GetButtonDown( "B" ) ) )
			{
				StartBattle();
				
				axisBusy = true;
			}
			else
			{
				axisBusy = false;
			}
		}
		else
			axisBusy = false;
	}

	public void Menu()
	{
		// Load the main menu
		Application.LoadLevel( "InteractiveMenu" );
	}

	public void StartBattle()
	{
		isPaused = false;
		pauseCanvas.enabled = false;
		
		MessageCenter.Instance.Broadcast( new LevelStartMessage( LevelStartType.Start ) );
		MessageCenter.Instance.Broadcast( new PauseChangedMessage( false ) );

		DisableAllButtons();

		// Get the other scripts ready to go
		PauseScript pauseScript = GameObject.Find( "PauseCanvas" ).GetComponent<PauseScript>();
		pauseScript.enabled = true;

		OutroScript outroScript = GameObject.Find( "OutroCanvas" ).GetComponent<OutroScript>();
		outroScript.enabled = true;

		this.enabled = false;
	}

	private void EnableAllButtons()
	{
		foreach( Button button in pauseButtons )
			button.interactable = true;
	}
	
	private void DisableAllButtons()
	{
		foreach( Button button in pauseButtons ) 
			button.interactable = false;
	}
}