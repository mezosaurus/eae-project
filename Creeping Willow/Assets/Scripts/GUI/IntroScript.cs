using UnityEngine;
using System.Collections;

public class IntroScript : MonoBehaviour
{
	public Font guiFont;
	public Texture2D boxImage;
	public Texture2D buttonImage;
	public AudioClip sound;
	public Texture2D[] TutorialImages;
	private Rect boxRect;
	private Rect buttonRect;
	private GUIStyle boxStyle;
	private GUIStyle buttonStyle;
	private GUIContent boxContent;
	private GUIContent buttonContent;
	private int currentIntro;
	private AudioSource mapAudio;

	void Start ()
	{
		boxRect = new Rect( 485, 100, GlobalGameStateManager.originalWidth - 970, GlobalGameStateManager.originalHeight - 350 );
		buttonRect = new Rect( GlobalGameStateManager.originalWidth / 2 - 150, 850, 300, 150 );
		boxStyle = new GUIStyle();
		buttonStyle = new GUIStyle();
		boxContent = new GUIContent();
		buttonContent = new GUIContent();
		GlobalGameStateManager.LevelState = LevelState.BEGINLEVEL;
		MessageCenter.Instance.Broadcast( new PauseChangedMessage( true ) );
		mapAudio = gameObject.AddComponent<AudioSource>();
		mapAudio.clip = sound;
		currentIntro = 0;
	}

	void OnGUI ()
	{
		GUI.matrix = GlobalGameStateManager.PrepareMatrix();

		// prepare the style for the boxes
		boxStyle.font = guiFont;
		boxStyle.fontSize = 45;
		boxStyle.wordWrap = true;
		boxStyle.alignment = TextAnchor.UpperCenter;

		// prepare the style for buttons
		GUI.skin.button.normal.background = buttonImage;
		GUI.skin.button.hover.background = buttonImage;
		GUI.skin.button.active.background = buttonImage;
		buttonStyle = new GUIStyle( GUI.skin.button );
		buttonStyle.font = guiFont;
		buttonStyle.fontSize = 85;
		buttonStyle.alignment = TextAnchor.MiddleCenter;

		drawIntroBox();

		GUI.matrix = Matrix4x4.identity;
	}

	private void drawIntroBox ()
	{
		switch( currentIntro )
		{
		case 0:
			drawBox( "Feast on the Souls of the living\n\nwithout alerting the AxeMan", "Next", 85 );
			break;
			
		case 1:
			drawBox( "", "Next", 85 );
			GUI.DrawTexture( boxRect, TutorialImages[ 0 ], ScaleMode.StretchToFill );
			break;

		case 2:
			drawBox( "", "Next", 85 );
			GUI.DrawTexture( boxRect, TutorialImages[ 1 ], ScaleMode.StretchToFill );
			break;

		case 3:
			drawBox( "", "Next", 85 );
			GUI.DrawTexture( boxRect, TutorialImages[ 2 ], ScaleMode.StretchToFill );
			break;

		case 4:
			drawBox( "", "Next", 85 );
			GUI.DrawTexture( boxRect, TutorialImages[ 3 ], ScaleMode.StretchToFill );
			break;
			
		default:
			break;
		}

		// create the start button
		buttonStyle.fontSize = 85;
		if( GUI.Button( buttonRect, buttonContent, buttonStyle ) )
		{
			mapAudio.PlayOneShot( sound );
			currentIntro++;
		}

		if( currentIntro >= 5 ) 
		{
			startBattle();
		}
	}

	private void startBattle()
	{
		GlobalGameStateManager.LevelState = LevelState.INLEVEL;
		this.enabled = false;
		MessageCenter.Instance.Broadcast (new LevelStartMessage (LevelStartType.Start));
		MessageCenter.Instance.Broadcast( new PauseChangedMessage( false ) );
		//MessageCenter.Instance.Broadcast( new LevelFinishedMessage( LevelFinishedType.Win, LevelFinishedReason.NumNPCsEaten ) );
	}

	void Update ()
	{
		if( Input.GetButtonDown( "Start" ) || Input.GetButtonDown( "A" ) )
		{
			mapAudio.PlayOneShot( sound );
			currentIntro++;
		}
	}

	private void drawBox( string boxText, string buttonText, int fontSize )
	{	
		// draw the box and text
		boxContent.text = boxText;
		boxStyle.fontSize = fontSize;
		GUI.DrawTexture( boxRect, boxImage, ScaleMode.StretchToFill );
		GUI.Label( new Rect ( boxRect.x + 100, boxRect.y + 100, boxRect.width - 200, boxRect.height - 200 ), boxContent, boxStyle );

		// change the button text
		buttonContent.text = buttonText;
	}
}