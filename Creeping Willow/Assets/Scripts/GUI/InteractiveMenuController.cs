﻿using UnityEngine;
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
	public GameObject gateLeft;
	public GameObject gateRight;
	public Text levelText;
	public Image descriptionImage;
	public Text descriptionText;
	public Text descriptionScores;
	public Text descriptionScoresLabel;

	private bool axisBusy;
	private AudioSource clickAudio;
	private Vector3 cameraPosition;
	private Quaternion gateRotationLeft;
	private Quaternion gateRotationRight;
	private float totalZoomTime;
	private float elapsedZoomTime;
	private MenuPosition currentPosition;
	private LevelLoader levelLoader;
	private string levelName;
	private GameObject camera;
	private GameObject descriptionBox;

	// menu sounds
	public AudioClip gateSound;
	public AudioClip levelSound;
	public AudioClip menuMusic;

	private SoundManager soundManager;

	void Awake()
	{
		levelLoader = GameObject.FindObjectOfType<LevelLoader>();
		soundManager = GameObject.FindObjectOfType<SoundManager>();
		soundManager.ChangeMusic( menuMusic );

		MessageCenter.Instance.Broadcast( new PauseChangedMessage( false ) );
		Time.timeScale = 1;

		axisBusy = false;

		if( mainButtons.Length <= 0 )
			throw new UnassignedReferenceException( "No button was given to the menu" );

		GoToMainMenu();

		totalZoomTime = 25.0f;

		Screen.showCursor = false;
		Screen.lockCursor = true;

		camera = GameObject.Find( "Main Camera" );

		descriptionBox = GameObject.Find( "DescriptionBox" );
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
		// Play the gate sound if it is closed
		if( currentPosition == MenuPosition.MainGate && soundManager ) 
		{
			soundManager.PlaySoundEffect( gateSound );
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

		descriptionBox.transform.position = new Vector3( 0, -20, 700 );
	}

	public void GoToModeSelect()
	{
		// Play the audio if from the correct screen
		if( currentPosition == MenuPosition.LevelSelect && soundManager ) 
		{
			soundManager.PlaySoundEffect( levelSound );
		}

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

		descriptionBox.transform.position = new Vector3( 1500, -20, 700 );
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
		// TODO don't call this every update
		GameObject currentSelection = EventSystem.current.currentSelectedGameObject;
		if( currentSelection )
		{
			Transform imageObject = currentSelection.transform.parent.Find( "LevelImageBox/LevelImage" );
			if( imageObject )
			{
				descriptionImage.enabled = true;
				descriptionImage.sprite = imageObject.GetComponent<Image>().sprite;
			}

			switch( currentSelection.name )
			{
			case "Level0Button":
				descriptionText.text = "learn the art of the kill\n\nget your roots dirty";
				descriptionScores.text = "";
				descriptionScoresLabel.enabled = false;
				break;

			case "Level1Button":
				descriptionText.text = "on a beautiful day they come to play\nbut you will take their souls away";
				descriptionScores.text = "";
				descriptionScoresLabel.enabled = false;
				break;

			case "Level2Button":
				descriptionText.text = "there are days at the lake that can be calming and cool\n\nthis is not one of those days";
				descriptionScores.text = "";
				descriptionScoresLabel.enabled = false;
				break;

			case "Level3Button":
				descriptionText.text = "they stay safe on the other side\nbut fear will drive them into your arms";
				descriptionScores.text = "";
				descriptionScoresLabel.enabled = false;
				break;

			case "Level4Button":
				descriptionText.text = "those that enter the maze may never escape\n\nif they do they will not be the same";
				descriptionScores.text = "";
				descriptionScoresLabel.enabled = false;
				break;

			case "ModeSurvivalButton":
				descriptionText.text = "go on an endless killing spree to collect as much soul matter as possible\n\nyour reign of terror will end when all hollow trees have been chopped down";

				switch( levelName )
				{
				case "a stalk\nin the park":
					DisplayDescriptionScores( GetSavedHighScoresScript.LoadStalkSurvivorNames(), GetSavedHighScoresScript.LoadStalkSurvivorScores() );
					break;
					
				case "lakeside\nlullaby":
					DisplayDescriptionScores( GetSavedHighScoresScript.LoadQuadrantsSurvivorNames(), GetSavedHighScoresScript.LoadQuadrantsSurvivorScores() );
					break;
					
				case "over troubled\nwaters":
					DisplayDescriptionScores( GetSavedHighScoresScript.LoadBridgeSurvivorNames(), GetSavedHighScoresScript.LoadBridgeSurvivorScores() );
					break;
					
				case "hallowed\nlabyrinth":
					DisplayDescriptionScores( GetSavedHighScoresScript.LoadMazeSurvivorNames(), GetSavedHighScoresScript.LoadMazeSurvivorScores() );
					break;
					
				default:
					descriptionScores.text = "";
					descriptionScoresLabel.enabled = false;
					break;
				}
				break;

			case "ModeFeastButton":
				descriptionText.text = "get the highest score you can with using scares and lures to get bigger combos\n\nthe game ends when you eat 10 people";

				switch( levelName )
				{
				case "a stalk\nin the park":
					DisplayDescriptionScores( GetSavedHighScoresScript.LoadStalkFeastNames(), GetSavedHighScoresScript.LoadStalkFeastScores() );
					break;

				case "lakeside\nlullaby":
					DisplayDescriptionScores( GetSavedHighScoresScript.LoadQuadrantsFeastNames(), GetSavedHighScoresScript.LoadQuadrantsFeastScores() );
					break;

				case "over troubled\nwaters":
					DisplayDescriptionScores( GetSavedHighScoresScript.LoadBridgeFeastNames(), GetSavedHighScoresScript.LoadBridgeFeastScores() );
					break;

				case "hallowed\nlabyrinth":
					DisplayDescriptionScores( GetSavedHighScoresScript.LoadMazeFeastNames(), GetSavedHighScoresScript.LoadMazeFeastScores() );
					break;

				default:
					descriptionScores.text = "";
					descriptionScoresLabel.enabled = false;
					break;
				}
				break;

			case "ModeMarkedButton":
				descriptionText.text = "kill the five marked for harvest\nyour multiplier will drop continually and cannot be replenished\n\nthe faster they die the better";

				switch( levelName )
				{
				case "a stalk\nin the park":
					DisplayDescriptionScores( GetSavedHighScoresScript.LoadStalkMarkedNames(), GetSavedHighScoresScript.LoadStalkMarkedScores() );
					break;
					
				case "lakeside\nlullaby":
					DisplayDescriptionScores( GetSavedHighScoresScript.LoadQuadrantsMarkedNames(), GetSavedHighScoresScript.LoadQuadrantsMarkedScores() );
					break;
					
				case "over troubled\nwaters":
					DisplayDescriptionScores( GetSavedHighScoresScript.LoadBridgeMarkedNames(), GetSavedHighScoresScript.LoadBridgeMarkedScores() );
					break;
					
				case "hallowed\nlabyrinth":
					DisplayDescriptionScores( GetSavedHighScoresScript.LoadMazeMarkedNames(), GetSavedHighScoresScript.LoadMazeMarkedScores() );
					break;
					
				default:
					descriptionScores.text = "";
					descriptionScoresLabel.enabled = false;
					break;
				}
				break;

			case "ModeTimedButton":
				descriptionText.text = "you have five minutes to consume as many of the living as possible\nbe creative with your kills and you will be rewarded";
				break;

			default:
				descriptionImage.enabled = false;
				descriptionText.text = "";
				descriptionScores.text = "";
				descriptionScoresLabel.enabled = false;
				break;
			}
		}

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
			{
				soundManager.PlayClickSound();
				Application.Quit();
			}
			else if( currentPosition == MenuPosition.LevelSelect || currentPosition == MenuPosition.Scores || currentPosition == MenuPosition.Options )
			{
				soundManager.PlayClickSound();
				GoToMainMenu();
			}
			else if( currentPosition == MenuPosition.ModeSelect )
			{
				soundManager.PlayClickSound();
				GoToLevelSelect();
			}

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

		if( i_levelName == "Tutorial_Level" )
			levelName = "bloody\nbeginnings";
		else if( i_levelName == "Evan_Level1" )
			levelName = "a stalk\nin the park";
		else if( i_levelName == "Quadrants" )
			levelName = "lakeside\nlullaby";
		else if( i_levelName == "Bridge_Level" )
			levelName = "over troubled\nwaters";
		else if( i_levelName == "Maze_Level" )
			levelName = "hallowed\nlabyrinth";
		else
			levelName = "error";
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

	private void DisplayDescriptionScores( string[] names, int[] scores )
	{
		descriptionScores.text = "";
		descriptionScoresLabel.enabled = true;

		int i;
		for( i = 0; i < 5; i++ )
		{
			string name = names[ i ].ToLower();
			int score = scores[ i ];
			if( name != null && score > 0 )
				descriptionScores.text += name + "  " + score + "\n";
			else
				break;
		}
	}
}
