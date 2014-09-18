using UnityEngine;
using System.Collections;
public class GUIController : MonoBehaviour
{
	public Font guiFont;
	public Texture2D boxImage;
	public Texture2D buttonImage;
	public SceneEnum levelCode;
	public AudioClip sound;
	private Rect boxRect;
	private Rect buttonRect;
	private GUIStyle boxStyle;
	private GUIStyle buttonStyle;
	private GUIContent boxContent;
	private GUIContent buttonContent;
	private bool displayIntroBox;
	private bool displayOutroBox;
	private int currentIntro;
	private AudioSource mapAudio;

	void Start ()
	{
		boxRect = new Rect (100, 100, Globals.originalWidth - 200, Globals.originalHeight - 200);
		buttonRect = new Rect (Globals.originalWidth / 2 - 200, 700, 400, 200);
		boxStyle = new GUIStyle ();
		buttonStyle = new GUIStyle ();
		boxContent = new GUIContent ();
		buttonContent = new GUIContent ();
		displayIntroBox = true;
		displayOutroBox = false;
		Globals.CurrentScene = levelCode;
		Globals.GameState = GameState.BEGINLEVEL;
		mapAudio = gameObject.AddComponent<AudioSource> ();
		mapAudio.clip = sound;
	}

	void OnGUI ()
	{
		// scale the gui to match the current screen size
		GUI.matrix = Globals.PrepareMatrix ();

		// prepare the style for the boxes
		boxStyle.font = guiFont;
		boxStyle.fontSize = 45;
		boxStyle.wordWrap = true;
		boxStyle.alignment = TextAnchor.UpperCenter;

		// prepare the style for buttons
		GUI.skin.button.normal.background = buttonImage;
		GUI.skin.button.hover.background = buttonImage;
		GUI.skin.button.active.background = buttonImage;
		buttonStyle = new GUIStyle (GUI.skin.button);
		buttonStyle.font = guiFont;
		buttonStyle.fontSize = 85;
		buttonStyle.alignment = TextAnchor.MiddleCenter;

		// display the display boxes if neccessary
		switch (Globals.GameState)
		{
			case GameState.BEGINLEVEL:
				drawIntroBox ();
				break;
			case GameState.ENDOFLEVEL_VICTORY:
			case GameState.ENDOFLEVEL_DEFEAT:
				drawOutroBox ();
				break;
			case GameState.PAUSED:
				drawPauseBox ();
			break;
		}

		// reset the resolution
		GUI.matrix = Matrix4x4.identity;
	}

	private void drawIntroBox ()
	{
		drawBox ("This is an intro", "Continue", 85);

		// create the start button
		buttonStyle.fontSize = 85;
		if (GUI.Button (buttonRect, buttonContent, buttonStyle))
		{
			mapAudio.PlayOneShot (sound);
			startBattle ();
		}
	}

	private void drawPauseBox ()
	{
		opaqueBox ("Paused\n\nPress Start to resume or Back to quit", boxContent.text, 85);	
	}

	private void startBattle ()
	{
		displayIntroBox = false;
		Globals.GameState = GameState.INLEVEL_DEFAULT;
		GameTimer.StartTimer ();
	}

	void Update ()
	{
		if (Input.GetKeyDown (KeyCode.Return) || Input.GetKeyDown (KeyCode.Joystick1Button7) || Input.GetKeyDown (KeyCode.Joystick1Button9))
		{
			mapAudio.PlayOneShot (sound);
			if (displayIntroBox)
			{
				startBattle ();
			}
			else if (displayOutroBox)
			{
				endBattle ();
			}
		}
		else if (Globals.GameState == GameState.INLEVEL_DEFAULT && (Input.GetKeyDown (KeyCode.Backspace) || Input.GetKeyDown (KeyCode.Escape) || Input.GetKeyDown (KeyCode.Joystick1Button6) || Input.GetKeyDown (KeyCode.Joystick1Button8)))
		{
			mapAudio.PlayOneShot (sound);
			displayOutroBox = true;
			Globals.GameState = GameState.ENDOFLEVEL_DEFEAT;
		}
		else if (displayOutroBox && (Input.GetKeyDown (KeyCode.Backspace) || Input.GetKeyDown (KeyCode.Escape) || Input.GetKeyDown (KeyCode.Joystick1Button6) || Input.GetKeyDown (KeyCode.Joystick1Button8)))
		{
			mapAudio.PlayOneShot (sound);
			endBattle();
		}
	}

	private void drawOutroBox ()
	{
		if (Globals.GameState == GameState.ENDOFLEVEL_DEFEAT)
		{
			boxContent.text = "You Lost!";
			drawBox ("You Lost!", boxContent.text, 60);
			displayOutroBox = true;
		}
		else if (Globals.GameState == GameState.ENDOFLEVEL_VICTORY)
		{
			boxContent.text = "You Won!";
			drawBox ("You won!", boxContent.text, 60);
			displayOutroBox = true;
		}
		else
		{
			return;
		}

		Globals.CurrentScene = levelCode;

		// create end battle button
		buttonStyle.fontSize = 85;
		if (GUI.Button (buttonRect, buttonContent, buttonStyle))
		{
			endBattle ();
		}	
	}

	private void endBattle ()
	{
		Application.LoadLevel ("MAINMENU");
	}

	private void opaqueBox (string boxText, string buttonText, int fontSize)
	{
		GUI.color = new Color (1.0f, 1.0f, 1.0f, 0.75f);
		drawBox (boxText, buttonText, fontSize, new Rect (610, 300, 700, 600));
		GUI.color = new Color (1f, 1f, 1f, 1f);
	}

	private void drawBox (string boxText, string buttonText, int fontSize, Rect boxRect)
	{	
		// draw the box and text
		boxContent.text = boxText;
		boxStyle.fontSize = fontSize;
		GUI.DrawTexture (boxRect, boxImage, ScaleMode.StretchToFill);
		GUI.Label (new Rect (boxRect.x + 100, boxRect.y + 100, boxRect.width - 200, boxRect.height - 200), boxContent, boxStyle);
		// change the button text
		buttonContent.text = buttonText;
	}

	private void drawBox (string boxText, string buttonText, int fontSize)
	{	
		// draw the box and text
		boxContent.text = boxText;
		boxStyle.fontSize = fontSize;
		GUI.DrawTexture (boxRect, boxImage, ScaleMode.StretchToFill);
		GUI.Label (new Rect (boxRect.x + 100, boxRect.y + 100, boxRect.width - 200, boxRect.height - 200), boxContent, boxStyle);
		// change the button text
		buttonContent.text = buttonText;
	}
}