using UnityEngine;
using System.Collections;
using UnityEngine.UI;
using UnityEngine.EventSystems;

public class PauseScript : MonoBehaviour
{
	private Canvas pauseCanvas;
	private bool isPaused;
	private bool axisBusy;

	public Button[] pauseButtons;

	void Start()
	{
		isPaused = false;

		Screen.showCursor = false;
		Screen.lockCursor = true;

		axisBusy = false;

		pauseCanvas = GameObject.Find( "PauseCanvas" ).GetComponent<Canvas>();
	}

	void Update()
	{
		if( !axisBusy && Input.GetButtonDown( "Cancel" ) )
		{
			Application.Quit();
			
			axisBusy = true;
		}

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
				Resume();

				axisBusy = true;
			}
			else
			{
				axisBusy = false;
			}
		}
		else
		{
			axisBusy = false;

			// Pause game
			if( Input.GetButtonDown( "Start" ) || Input.GetButtonDown( "Back" ) )
			{
				Pause();
			}
		}
	}

	public void Pause()
	{
		isPaused = true;
		pauseCanvas.enabled = true;
		MessageCenter.Instance.Broadcast( new PauseChangedMessage( true ) );
		
		EventSystem.current.SetSelectedGameObject( pauseButtons[ 0 ].gameObject );
		
		axisBusy = true;
		
		foreach( Button button in pauseButtons )
			button.interactable = true;
	}

	public void Resume()
	{
		isPaused = false;
		pauseCanvas.enabled = false;
		MessageCenter.Instance.Broadcast( new PauseChangedMessage( false ) );

		DisableAllButtons();
	}

	public void Menu()
	{
		// Load the main menu
		Application.LoadLevel( "InteractiveMenu" );
	}

	private void DisableAllButtons()
	{
		foreach( Button button in pauseButtons )
			button.interactable = false;
	}
}
