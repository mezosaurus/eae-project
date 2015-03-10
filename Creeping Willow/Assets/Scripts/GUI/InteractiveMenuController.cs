using UnityEngine;
using System.Collections;
using UnityEngine.UI;
using UnityEngine.EventSystems;

public class InteractiveMenuController : MonoBehaviour
{
	private enum MenuPosition
	{
		MainGate,
		LevelSelect,
		ModeSelect,
		Options,
		Scores,
	}

	public Button[] mainButtons;
	public Button[] levelButtons;
	public Button[] modeButtons;
	public Button[] scoresButtons;
	public Button[] optionsButtons;
	public GameObject camera;
	public GameObject gateLeft;
	public GameObject gateRight;
	public Text levelText;

	private bool axisBusy;
	private bool usesSound;
	private AudioSource clickAudio;
	private Vector3 cameraPosition;
	private Quaternion gateRotationLeft;
	private Quaternion gateRotationRight;
	private float totalZoomTime;
	private float elapsedZoomTime;
	private MenuPosition currentPosition;
	private LevelLoader levelLoader;
	private string levelName;

	// menu sounds
	public AudioClip clickSound;
	public AudioClip gateSound;
	public AudioClip levelSound;
	private bool playGateSound = true;

	void Awake()
	{
		axisBusy = false;

		if( clickSound )
		{
			usesSound = true;
			//clickAudio = gameObject.AddComponent<AudioSource>();
			//clickAudio.clip = clickSound;
		}

		if( mainButtons.Length <= 0 )
			throw new UnassignedReferenceException( "No button was given to the menu" );

		GoToMainMenu();

		totalZoomTime = 25.0f;

		levelLoader = GameObject.FindObjectOfType<LevelLoader>();

		Screen.showCursor = true;
		Screen.lockCursor = false;
	}

	void OnDestroy() {}

	private void DisableAllButtons()
	{
		foreach( Button button in mainButtons )
			button.interactable = false;
		
		foreach( Button button in levelButtons )
			button.interactable = false;

		foreach( Button button in modeButtons )
			button.interactable = false;
		
		foreach( Button button in scoresButtons )
			button.interactable = false;

		foreach( Button button in optionsButtons )
			button.interactable = false;
	}

	public void GoToMainMenu()
	{
		playGateSound = true;
		// Get rid of all other buttons
		DisableAllButtons();

		// Show all the buttons
		foreach( Button button in mainButtons )
			button.interactable = true;

		EventSystem.current.SetSelectedGameObject( mainButtons[ 0 ].gameObject );

		// Set the next camera position
		cameraPosition = new Vector3( 0, 55, -305 );

		// set the gate rotation
		gateRotationLeft = Quaternion.Euler( 0, 0, 0 );
		gateRotationRight = Quaternion.Euler( 0, 0, 0 );

		currentPosition = MenuPosition.MainGate;
		
		elapsedZoomTime = 0.0f;
	}

	public void GoToLevelSelect()
	{
		if (playGateSound) 
		{
			camera.audio.PlayOneShot (gateSound, 0.8f);
			playGateSound = false;
		}
		// Get rid of all other buttons
		DisableAllButtons();

		// Show all the buttons
		foreach( Button button in levelButtons )
			button.interactable = true;

		EventSystem.current.SetSelectedGameObject( levelButtons[ 0 ].gameObject );

		// Set the next camera position
		cameraPosition = new Vector3( 0, 0, 400 );

		// set the gate rotation
		gateRotationLeft = Quaternion.Euler( 0, -45, 0 );
		gateRotationRight = Quaternion.Euler( 0, 45, 0 );

		currentPosition = MenuPosition.LevelSelect;

		elapsedZoomTime = 0.0f;
	}

	public void GoToModeSelect()
	{
		camera.audio.PlayOneShot (levelSound, 0.8f);
		// Get rid of all other buttons
		DisableAllButtons();
		
		// Show all the buttons
		foreach( Button button in modeButtons )
			button.interactable = true;
		
		EventSystem.current.SetSelectedGameObject( modeButtons[ 0 ].gameObject );

		levelText.text = levelName;
		
		// Set the next camera position
		cameraPosition = new Vector3( 1500, 0, 400 );
		
		currentPosition = MenuPosition.ModeSelect;
		
		elapsedZoomTime = 0.0f;
	}

	public void GoToHighScores()
	{
		// Get rid of all other buttons
		DisableAllButtons();

		// Show all the buttons
		foreach( Button button in scoresButtons )
			button.interactable = true;
		
		EventSystem.current.SetSelectedGameObject( scoresButtons[ 0 ].gameObject );
		
		// Set the next camera position
		cameraPosition = new Vector3( 545, 10, -123 );
		currentPosition = MenuPosition.Scores;

		elapsedZoomTime = 0.0f;
	}

	public void GoToOptions()
	{
		// Get rid of all other buttons
		DisableAllButtons();

		// Show all the buttons
		foreach( Button button in optionsButtons )
			button.interactable = true;
		
		EventSystem.current.SetSelectedGameObject( optionsButtons[ 0 ].gameObject );
		
		// Set the next camera position
		cameraPosition = new Vector3( -320, 0, -120 );
		currentPosition = MenuPosition.Options;

		elapsedZoomTime = 0.0f;
	}

	// Update is called once per frame
	void Update()
	{
		// Check for mouse
		if( Input.GetAxis( "Mouse X" ) != 0 || Input.GetAxis( "Mouse Y" ) != 0 )
		{
			Screen.showCursor = true;
			Screen.lockCursor = false;
			axisBusy = true;
			UnselectAll();
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
				switch( currentPosition )
				{
				case MenuPosition.MainGate:
					EventSystem.current.SetSelectedGameObject( mainButtons[ 0 ].gameObject );
					break;

				case MenuPosition.LevelSelect:
					EventSystem.current.SetSelectedGameObject( levelButtons[ 0 ].gameObject );
					break;

				case MenuPosition.ModeSelect:
					EventSystem.current.SetSelectedGameObject( modeButtons[ 0 ].gameObject );
					break;

				case MenuPosition.Scores:
					EventSystem.current.SetSelectedGameObject( scoresButtons[ 0 ].gameObject );
					break;

				case MenuPosition.Options:
					EventSystem.current.SetSelectedGameObject( optionsButtons[ 0 ].gameObject );
					break;

				default:
					break;
				}
			}
		}
		// The user wants to go back
		else if( !axisBusy && ( Input.GetButtonDown( "Back" ) || Input.GetButtonDown( "B" ) ) )
		{
			if( currentPosition == MenuPosition.MainGate && Vector3.Distance( camera.transform.position, cameraPosition ) < 5.0f )
				Application.Quit();
			else if( currentPosition == MenuPosition.LevelSelect || currentPosition == MenuPosition.Scores || currentPosition == MenuPosition.Options )
				GoToMainMenu();
			else if( currentPosition == MenuPosition.ModeSelect )
				GoToLevelSelect();

			axisBusy = true;
		}
		// The user wants to quit
		else if( !axisBusy && Input.GetButtonDown( "Cancel" ) )
		{
			Application.Quit();
			
			axisBusy = true;
		}
		else
			axisBusy = false;

		// Move the camera to the correct position
		if( camera.transform.position != cameraPosition)// || gateLeft.transform.rotation != gateRotation )
		{
			elapsedZoomTime += Time.deltaTime;
			camera.transform.position = Vector3.Lerp( camera.transform.position, cameraPosition, elapsedZoomTime / totalZoomTime );
			gateLeft.transform.rotation = Quaternion.Lerp( gateLeft.transform.rotation, gateRotationLeft, elapsedZoomTime / totalZoomTime * 4 );
			gateRight.transform.rotation = Quaternion.Lerp( gateRight.transform.rotation, gateRotationRight, elapsedZoomTime / totalZoomTime * 4 );
		}
		else
			elapsedZoomTime = 0.0f;
	}

	private void UnselectAll()
	{
		foreach( Button button in mainButtons )
			button.OnDeselect( new BaseEventData( EventSystem.current ) );

		foreach( Button button in levelButtons )
			button.OnDeselect( new BaseEventData( EventSystem.current ) );

		foreach( Button button in modeButtons )
			button.OnDeselect( new BaseEventData( EventSystem.current ) );

		foreach( Button button in scoresButtons )
			button.OnDeselect( new BaseEventData( EventSystem.current ) );

		foreach( Button button in optionsButtons )
			button.OnDeselect( new BaseEventData( EventSystem.current ) );
	}

	public void SetLevel( string i_levelName )
	{
		// Persist the level loader
		levelLoader.levelName = i_levelName;

		if( i_levelName == "Evan_Level1" )
			levelName = "Bloody Beginnings";
		else if( i_levelName == "Quadrants" )
			levelName = "Lakeside Lullaby";
		else if( i_levelName == "Bridge_Level" )
			levelName = "Over Troubled Waters";
		else if( i_levelName == "Maze_Level" )
			levelName = "Hallowed Labyrinth";
		else
			levelName = "Error";
	}

	public void SetMode( string i_modeName )
	{
		// Persist the level loader
		levelLoader.modeName = i_modeName;
	}

	public void LoadLevel()
	{
		// Load the new level
		Application.LoadLevel( "LoadingScreen" );

		// Hide the mouse
		Screen.showCursor = false;
		Screen.lockCursor = true;
	}
}
