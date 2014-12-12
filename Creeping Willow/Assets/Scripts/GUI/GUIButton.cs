using UnityEngine;
using System.Collections;

/// <summary>
/// GUIButton should be attached to an empty object.
/// All location and scale variables are based on 
/// the original 1920x1080 sceme. The GUI is then 
/// scaled to match the current screen resolution.
/// </summary>
public class GUIButton : MonoBehaviour
{
	public float x, y, width, height;
	public Texture2D defaultImage, hoverImage, downClickImage;
	public string text;
	public Font font;
	public SceneEnum scene;
	public AudioClip sound;
	public bool changeScene = true;

	private AudioSource audio;

	void Awake()
	{
		audio = gameObject.AddComponent<AudioSource> ();
		audio.clip = sound;
	}

	void OnGUI()
	{
		GUI.matrix = GlobalGameStateManager.PrepareMatrix();

		// set the GUI images and font
		GUI.skin.button.normal.background = (Texture2D)defaultImage;
		GUI.skin.button.hover.background = (Texture2D)hoverImage;
		GUI.skin.button.active.background = (Texture2D)defaultImage;
		GUI.skin.font = font;
		GUI.skin.GetStyle( "Button" ).fontSize = Mathf.FloorToInt( 0.6f * height );
		GUI.skin.GetStyle( "Button" ).normal.textColor = Color.black;
		GUI.depth = 0;

		// draw the button
		if( GUI.Button( new Rect( x - width / 2, y - height / 2, width, height ), text ) )
				changeScenes();

		GUI.matrix = Matrix4x4.identity;
	}

	public void changeScenes()
	{
		if( changeScene )
		{
			audio.Play();
			Invoke( "switchScenes", 0.5f );
		}
	}

	void switchScenes ()
	{
		GlobalGameStateManager.CurrentScene = scene;
		Application.LoadLevel (scene.ToString ());
	}
}
