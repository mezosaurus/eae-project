using UnityEngine;
using System.Collections;

public class IntroScript : MonoBehaviour
{
	public Font guiFont;
	public Texture2D boxImage;
	public Texture2D buttonImage;
	public AudioClip sound;
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
		boxRect = new Rect( 100, 100, GlobalGameStateManager.originalWidth - 200, GlobalGameStateManager.originalHeight - 200 );
		buttonRect = new Rect( GlobalGameStateManager.originalWidth / 2 - 200, 700, 400, 200 );
		boxStyle = new GUIStyle();
		buttonStyle = new GUIStyle();
		boxContent = new GUIContent();
		buttonContent = new GUIContent();
		GlobalGameStateManager.LevelState = LevelState.BEGINLEVEL;
		MessageCenter.Instance.Broadcast( new PauseChangedMessage( true ) );
		mapAudio = gameObject.AddComponent<AudioSource>();
		mapAudio.clip = sound;
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

		drawIntroBox ();

		GUI.matrix = Matrix4x4.identity;
	}

	private void drawIntroBox ()
	{
		drawBox( "Feast on the Souls of the living\n\nwithout alerting the AxeMan", "Begin", 85 );

		// create the start button
		buttonStyle.fontSize = 85;
		if( GUI.Button( buttonRect, buttonContent, buttonStyle ) )
		{
			mapAudio.PlayOneShot( sound );
			startBattle();
		}
	}

	private void startBattle()
	{
		GlobalGameStateManager.LevelState = LevelState.INLEVEL;
		this.enabled = false;
		MessageCenter.Instance.Broadcast( new PauseChangedMessage( false ) );
	}

	void Update ()
	{
		if( Input.GetButtonDown( "Start" ) || Input.GetButtonDown( "A" ) )
		{
			mapAudio.PlayOneShot( sound );
			startBattle();
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