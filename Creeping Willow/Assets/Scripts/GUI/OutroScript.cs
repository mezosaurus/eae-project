using UnityEngine;
using System.Collections;

public class OutroScript : MonoBehaviour
{
	public Font guiFont;
	public Texture2D boxImage;
	public GameObject menu;
	private Rect boxRect;
	private GUIStyle boxStyle;
	private GUIContent boxContent;
	private int currentIntro;
	private AudioSource mapAudio;
	private string message;

	void Start ()
	{
		boxRect = new Rect( 485, 100, GlobalGameStateManager.originalWidth - 970, GlobalGameStateManager.originalHeight - 350 );
		boxStyle = new GUIStyle();
		boxContent = new GUIContent();
		MessageCenter.Instance.Broadcast( new PauseChangedMessage( true ) );
		this.enabled = false;
		RegisterListeners();
	}
	
	void OnDestroy()
	{
		UnregisterListeners();
	}

	protected void RegisterListeners()
	{
		MessageCenter.Instance.RegisterListener( MessageType.LevelFinished, HandleLevelFinishedMessage );
	}
	
	protected void UnregisterListeners()
	{
		MessageCenter.Instance.UnregisterListener( MessageType.LevelFinished, HandleLevelFinishedMessage );
	}

	protected void HandleLevelFinishedMessage( Message message )
	{
		menu.GetComponent<MenuController>().enabled = true;
		for( int i = 0; i < menu.GetComponent<MenuController>().buttons.Length; i++ )
		{
			menu.GetComponent<MenuController>().buttons[ i ].GetComponent<GUIButton>().enabled = true;
		}

		LevelFinishedMessage mess = message as LevelFinishedMessage;

		if( mess.Type == LevelFinishedType.Loss )
		{
			switch( mess.Reason )
			{
			case LevelFinishedReason.MaxNPCsPanicked:
				this.enabled = true;
				this.message = "You have failed\n\nThe NPC noticed you and got scared";
				MessageCenter.Instance.Broadcast( new PauseChangedMessage( true ) );
				break;

			case LevelFinishedReason.PlayerDied:
				this.enabled = true;
				this.message = "You have failed\n\nYour tree was chopped down and made into evil little toothpicks";
				MessageCenter.Instance.Broadcast( new PauseChangedMessage( true ) );
				break;

			case LevelFinishedReason.TimerOut:
				this.enabled = true;
				this.message = "You have failed\n\nYou ran out of time";
				MessageCenter.Instance.Broadcast( new PauseChangedMessage( true ) );
				break;

			default:
				break;
			}
		}
		else if( mess.Type == LevelFinishedType.Win )
		{
			switch( mess.Reason )
			{
			case LevelFinishedReason.TargetNPCEaten:
				this.enabled = true;
				this.message = "You have won\n\nYour target has been consumed";
				MessageCenter.Instance.Broadcast( new PauseChangedMessage( true ) );
				break;
				
			case LevelFinishedReason.NumNPCsEaten:
				this.enabled = true;
				this.message = "You have won\n\nYour tree has feasted upon many souls";
				MessageCenter.Instance.Broadcast( new PauseChangedMessage( true ) );
				break;
				
			case LevelFinishedReason.TimerOut:
				this.enabled = true;
				this.message = "You have won\n\nYou survived the day";
				MessageCenter.Instance.Broadcast( new PauseChangedMessage( true ) );
				break;
				
			default:
				break;
			}
		}


	}

	void OnGUI ()
	{
		GUI.matrix = GlobalGameStateManager.PrepareMatrix();

		// prepare the style for the boxes
		boxStyle.font = guiFont;
		boxStyle.fontSize = 45;
		boxStyle.wordWrap = true;
		boxStyle.alignment = TextAnchor.UpperCenter;

		drawIntroBox ();

		GUI.matrix = Matrix4x4.identity;
	}

	private void drawIntroBox ()
	{
		drawBox( message, "Try Again", 85 );
	}

	private void restartBattle()
	{
		Application.LoadLevel( Application.loadedLevel );
	}

	private void drawBox( string boxText, string buttonText, int fontSize )
	{	
		// draw the box and text
		boxContent.text = boxText;
		boxStyle.fontSize = fontSize;
		GUI.DrawTexture( boxRect, boxImage, ScaleMode.StretchToFill );
		GUI.Label( new Rect ( boxRect.x + 100, boxRect.y + 100, boxRect.width - 200, boxRect.height - 200 ), boxContent, boxStyle );
	}
}