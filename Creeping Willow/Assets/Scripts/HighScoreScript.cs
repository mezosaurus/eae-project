using UnityEngine;
using System.Collections;

public class HighScoreScript : MonoBehaviour
{
	public Font guiFont;
	public Texture2D boxImage;
	private Rect boxRect;
	private GUIStyle boxStyle;
	private GUIContent boxContent;

	void Start()
	{
		boxRect = new Rect( 650, 100, GlobalGameStateManager.originalWidth - 1300, GlobalGameStateManager.originalHeight - 350 );
		boxStyle = new GUIStyle();
		boxContent = new GUIContent();

		//Testing
		/*for( int i = 0; i < GlobalGameStateManager.highscores.Length; i++ )
		{
			GlobalGameStateManager.playerNames[ i ] = "abc";
		}*/
	}

	void OnGUI()
	{
		GUI.matrix = GlobalGameStateManager.PrepareMatrix();

		Rect labelRect = new Rect( boxRect );
		labelRect.width = boxRect.width;
		labelRect.height = ( boxRect.height - 70 ) / 11;
		labelRect.x = boxRect.x + 70;

		// prepare the style for the boxes
		boxStyle.font = guiFont;
		boxStyle.fontSize = 45;
		boxStyle.wordWrap = true;
		boxStyle.alignment = TextAnchor.MiddleLeft;
		GUI.DrawTexture( boxRect, boxImage, ScaleMode.StretchToFill );
		labelRect.y = boxRect.y + 10;
		GUI.Label( labelRect, "#  Name        Score", boxStyle );

		for( int i = 0; i < GlobalGameStateManager.highscores.Length; i++ )
		{
			if( GlobalGameStateManager.playerNames[ i ] != null )
			{
				labelRect.y = boxRect.y + 10 + labelRect.height * ( i + 1 );
				GUI.Label( labelRect, (i + 1).ToString() + ": " + GlobalGameStateManager.playerNames[ i ] + " ... " + GlobalGameStateManager.highscores[ i ].ToString(), boxStyle );
			}
		}
		
		GUI.matrix = Matrix4x4.identity;
	}
}
