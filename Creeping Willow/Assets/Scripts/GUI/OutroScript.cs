using UnityEngine;
using System.Collections;
using UnityEngine.UI;
using UnityEngine.EventSystems;

public class OutroScript : MonoBehaviour
{
	public Button[] pauseButtons;
	public Text outroMessage;

	private Canvas pauseCanvas;
	private bool isPaused;
	private bool axisBusy;

	void Start()
	{
		isPaused = false;
		axisBusy = false;

		// Get the canvas and buttons ready
		pauseCanvas = GameObject.Find( "OutroCanvas" ).GetComponent<Canvas>();
		pauseCanvas.enabled = false;

        //outroMessage = new Text();

		RegisterListeners();
	}

	protected void RegisterListeners()
	{
		MessageCenter.Instance.RegisterListener( MessageType.LevelFinished, HandleLevelFinishedMessage );
		MessageCenter.Instance.RegisterListener( MessageType.ScoreAdding, HandleScoreAddingMessage );
	}

	protected void UnregisterListeners()
	{
		MessageCenter.Instance.UnregisterListener( MessageType.LevelFinished, HandleLevelFinishedMessage );
		MessageCenter.Instance.UnregisterListener( MessageType.ScoreAdding, HandleScoreAddingMessage );
	}

	protected void HandleLevelFinishedMessage( Message message )
	{
		LevelEnded();

		LevelFinishedMessage mess = message as LevelFinishedMessage;
		/*switch( mess.Type )
		{
		case LevelFinishedType.Loss:
			switch( mess.Reason )
			{
			case LevelFinishedReason.MaxNPCsPanicked:
				outroMessage.text = "You have failed\n\nThe NPC noticed you and got scared";
				break;
				
			case LevelFinishedReason.PlayerDied:
				outroMessage.text = "You have failed\n\nYour tree was chopped down and made into evil little toothpicks";
				break;
				
			case LevelFinishedReason.TimerOut:
				outroMessage.text = "You have failed\n\nYou ran out of time";
				break;
				
			default:
				break;
			}
			break;

		case LevelFinishedType.Win:
			switch( mess.Reason )
			{
			case LevelFinishedReason.TargetNPCEaten:
				outroMessage.text = "You have won\n\nYour target has been consumed";
				break;
				
			case LevelFinishedReason.NumNPCsEaten:
				outroMessage.text = "You have won\n\nYour tree has feasted upon many souls";
				break;

			case LevelFinishedReason.TimerOut:
				outroMessage.text = "You have won\n\nYou survived the day";
				break;

			default:
				break;
			}
			break;

		case LevelFinishedType.Tie:
			outroMessage.text = "You have tied";
			break;
		}*/
	}

	protected void HandleScoreAddingMessage( Message message )
	{
		ScoreAddingMessage mess = message as ScoreAddingMessage;
		if( mess.adding == false )
			ReadyButtons();
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
				Menu();
				
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

	private void LevelEnded()
	{
		isPaused = true;
		pauseCanvas.enabled = true;
		MessageCenter.Instance.Broadcast( new PauseChangedMessage( true ) );

		// Disable the other scripts until we are out of the menu
		IntroScript introScript = GameObject.Find( "IntroCanvas" ).GetComponent<IntroScript>();
		introScript.enabled = false;

		PauseScript pauseScript = GameObject.Find( "PauseCanvas" ).GetComponent<PauseScript>();
		pauseScript.enabled = false;

		// Hide the cursor
		Screen.showCursor = false;
		Screen.lockCursor = true;

		axisBusy = true;
	}

	private void ReadyButtons()
	{
		EnableAllButtons();
		EventSystem.current.SetSelectedGameObject( pauseButtons[ 0 ].gameObject );
	}

	public void Menu()
	{
		// Hide the cursor
		Screen.showCursor = false;
		Screen.lockCursor = true;

		// Load the main menu
		Application.LoadLevel( "InteractiveMenu" );
	}

	public void Retry()
	{
		// Hide the cursor
		Screen.showCursor = false;
		Screen.lockCursor = true;

		// Restart the level
		Application.LoadLevel( Application.loadedLevel );
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