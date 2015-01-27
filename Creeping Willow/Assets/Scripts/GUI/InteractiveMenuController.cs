using UnityEngine;
using System.Collections;
using UnityEngine.UI;
using UnityEngine.EventSystems;

public class InteractiveMenuController : MonoBehaviour
{
	private enum MenuPosition
	{
		MainGate,
		Options,
		Scores,
		LevelSelect,
	}

	public Button[] mainButtons;
	public Button[] levelButtons;
	public AudioClip clickSound;
	public Texture2D cursorImage;
	public GameObject camera;

	private bool axisBusy;
	private bool usesSound;
	private AudioSource clickAudio;
	private Vector3 cameraPosition;
	private float totalZoomTime;
	private float elapsedZoomTime;
	private MenuPosition currentPosition;

	void Awake()
	{
		axisBusy = false;

		if( clickSound )
		{
			usesSound = true;
			clickAudio = gameObject.AddComponent<AudioSource>();
			clickAudio.clip = clickSound;
		}

		if( mainButtons.Length <= 0 )
			throw new UnassignedReferenceException( "No button was given to the menu" );

		GoToMainMenu();

		if( cursorImage )
			Cursor.SetCursor( cursorImage, new Vector2( cursorImage.width / 2, cursorImage.height / 2 ), CursorMode.ForceSoftware );

		totalZoomTime = 4.0f;
	}

	void OnDestroy() {}

	public void GoToMainMenu()
	{
		// Get rid of all other buttons
		HideLevelMenu();

		// Show all the buttons
		foreach( Button button in mainButtons )
			button.interactable = true;

		EventSystem.current.SetSelectedGameObject( mainButtons[ 0 ].gameObject );

		// Set the next camera position
		cameraPosition = new Vector3( 0, 0, -10 );
		currentPosition = MenuPosition.MainGate;
		
		elapsedZoomTime = 0.0f;
	}

	private void HideMainMenu()
	{
		foreach( Button button in mainButtons )
			button.interactable = false;
	}

	public void GoToLevelSelect()
	{
		// Get rid of all other buttons
		HideMainMenu();

		// Show all the buttons
		foreach( Button button in levelButtons )
			button.interactable = true;

		EventSystem.current.SetSelectedGameObject( levelButtons[ 0 ].gameObject );

		// Set the next camera position
		cameraPosition = new Vector3( 0, 0, 10 );
		currentPosition = MenuPosition.LevelSelect;

		elapsedZoomTime = 0.0f;
	}

	private void HideLevelMenu()
	{
		foreach( Button button in levelButtons )
			button.interactable = false;
	}

	public void GoToHighScores()
	{
		// Get rid of all other buttons
		HideMainMenu();
		
		// Set the next camera position
		cameraPosition = new Vector3( 8, -2, -5 );
		currentPosition = MenuPosition.Scores;

		elapsedZoomTime = 0.0f;
	}

	public void GoToOptions()
	{
		// Get rid of all other buttons
		HideMainMenu();
		
		// Set the next camera position
		cameraPosition = new Vector3( -8, -2, -5 );
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
		else if( !axisBusy && Input.GetAxis( "LSX" ) != 0 )
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

				default:
					break;
				}
			}
		}
		// The user wants to go back
		else if( !axisBusy && ( Input.GetAxisRaw( "Back" ) != 0 || Input.GetAxisRaw( "B" ) != 0 ) )
		{
			if( currentPosition == MenuPosition.MainGate )
				Application.Quit();
			else if( currentPosition == MenuPosition.LevelSelect || currentPosition == MenuPosition.Scores || currentPosition == MenuPosition.Options )
				GoToMainMenu();

			axisBusy = true;
		}
		else
			axisBusy = false;

		// Move the camera to the correct position
		if( camera.transform.position != cameraPosition )
		{
			elapsedZoomTime += Time.deltaTime;
			camera.transform.position = Vector3.Lerp( camera.transform.position, cameraPosition, elapsedZoomTime / totalZoomTime );
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
	}

	public void LoadLevel( string i_levelName )
	{
		Application.LoadLevel( i_levelName );
	}
}
