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
	public bool multiplayer = false;

	void OnGUI ()
	{
		// scale the GUI to the current screen size
		GUI.matrix = Globals.PrepareMatrix ();

		// set the GUI images and font
		GUI.skin.button.normal.background = (Texture2D)defaultImage;
		GUI.skin.button.hover.background = (Texture2D)hoverImage;
		GUI.skin.button.active.background = (Texture2D)defaultImage;
		GUI.skin.font = font;
		GUI.skin.GetStyle ("Button").fontSize = Mathf.FloorToInt (0.6f * height);
		GUI.depth = 0;

		// draw the button
		if (GUI.Button (new Rect (x - width / 2, y - height / 2, width, height), text))
		{
			changeScenes ();
		}

		// reset the resolution
		GUI.matrix = Matrix4x4.identity;
	}

	public void changeScenes ()
	{
		audio.Play ();
		Invoke ("switchScenes", 0.5f);
	}

	void switchScenes ()
	{
		if (!Globals.CurrentScene.ToString ().Equals ("MainMenu") && !text.Equals ("Back"))
			Application.LoadLevel (scene.ToString () + "MP");
		else
			Application.LoadLevel (scene.ToString ());
		Globals.CurrentScene = scene;
	}
}