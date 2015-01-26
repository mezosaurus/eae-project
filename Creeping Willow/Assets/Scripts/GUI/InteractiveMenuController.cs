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
	public AudioClip clickSound;
	public Texture2D cursorImage;
	public GameObject camera;

	private bool axisBusy;
	private int selected;
	private bool usesSound;
	private AudioSource clickAudio;
	private Vector3 cameraPosition;
	private float totalZoomTime;
	private float elapsedZoomTime;
	private MenuPosition currentPosition;

	void Awake()
	{
		selected = 0;
		axisBusy = false;

		if( clickSound )
		{
			usesSound = true;
			clickAudio = gameObject.AddComponent<AudioSource>();
			clickAudio.clip = clickSound;
		}

		selected = -1;

		if( mainButtons.Length <= 0 )
			throw new UnassignedReferenceException( "No button was given to the menu" );

		GoToMainMenu();

		if( cursorImage )
			Cursor.SetCursor( cursorImage, new Vector2( cursorImage.width / 2, cursorImage.height / 2 ), CursorMode.ForceSoftware );

		totalZoomTime = 3.0f;
		cameraPosition = new Vector3( 0, 0, -10 );

		currentPosition = MenuPosition.MainGate;
	}

	void OnDestroy() {}

	public void GoToMainMenu()
	{
		// Show all the front buttons
		foreach( Button button in mainButtons )
		{
			button.enabled = true;
			button.image.enabled = true;
		}
		
		// Set the next camera position
		cameraPosition = new Vector3( 0, 0, -10 );
		currentPosition = MenuPosition.MainGate;
		
		elapsedZoomTime = 0.0f;
	}

	public void GoToLevelSelect()
	{
		// Get rid of all the front buttons
		foreach( Button button in mainButtons )
		{
			button.enabled = false;
			button.image.enabled = false;
		}

		// Set the next camera position
		cameraPosition = new Vector3( 0, 0, 0 );
		currentPosition = MenuPosition.LevelSelect;

		elapsedZoomTime = 0.0f;
	}

	public void GoToHighScores()
	{
		// Get rid of all the front buttons
		foreach( Button button in mainButtons )
		{
			button.enabled = false;
			button.image.enabled = false;
		}
		
		// Set the next camera position
		cameraPosition = new Vector3( 8, -2, -5 );
		currentPosition = MenuPosition.Scores;

		elapsedZoomTime = 0.0f;
	}

	public void GoToOptions()
	{
		// Get rid of all the front buttons
		foreach( Button button in mainButtons )
		{
			button.enabled = false;
			button.image.enabled = false;
		}
		
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
			selected = -1;
			UnselectAll();
		}

		// Check for controller movement
		else if( Input.GetAxis( "LSX" ) != 0 || Input.GetAxis( "DX" ) != 0 || Input.GetAxis( "RSX" ) != 0 )
		{
			if( !axisBusy )
			{
				if( Input.GetAxisRaw( "LSX" ) < 0 || Input.GetAxisRaw( "DX" ) < 0 || Input.GetAxisRaw( "RSX" ) < 0 )
					selected--;
				else if( Input.GetAxisRaw( "LSX" ) > 0 || Input.GetAxisRaw( "DX" ) > 0 || Input.GetAxisRaw( "RSX" ) > 0 )
					selected++;

				if( selected < 0 )
					selected = mainButtons.Length - 1;
				else if( selected > mainButtons.Length - 1 )
					selected = 0;

				Screen.showCursor = false;
				Screen.lockCursor = true;
				axisBusy = true;
				Select( mainButtons[ selected ] );
			}
		}
		else
			axisBusy = false;

		// The user wants to go forward
		if( Input.GetAxisRaw( "Start" ) != 0 || Input.GetAxisRaw( "A" ) != 0 )
		{
			if( selected >= 0 )
				mainButtons[ selected ].OnPointerClick( new PointerEventData( EventSystem.current ) );
		}
		// The user wants to go back
		else if( Input.GetAxisRaw( "Back" ) != 0 || Input.GetAxisRaw( "B" ) != 0 )
		{
			//TODO exit the program or ask to quit or something
			if( currentPosition == MenuPosition.MainGate )
				Application.Quit();
			else if( currentPosition == MenuPosition.LevelSelect || currentPosition == MenuPosition.Scores || currentPosition == MenuPosition.Options )
				GoToMainMenu();
		}

		// Move the camera to the correct position
		if( camera.transform.position != cameraPosition )
		{
			elapsedZoomTime += Time.deltaTime;
			camera.transform.position = Vector3.Lerp( camera.transform.position, cameraPosition, elapsedZoomTime / totalZoomTime );
		}
		else
			elapsedZoomTime = 0.0f;
	}

	private void Select( Button button )
	{
		UnselectAll();

		button.OnSelect( new BaseEventData( EventSystem.current ) );
		if( usesSound )
			clickAudio.PlayOneShot( clickSound );
	}

	private void UnselectAll()
	{
		foreach( Button button in mainButtons )
			button.OnDeselect( new BaseEventData( EventSystem.current ) );
	}
}
