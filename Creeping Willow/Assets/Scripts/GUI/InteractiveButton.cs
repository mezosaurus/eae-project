using UnityEngine;
using System.Collections;

public class InteractiveButton : MonoBehaviour
{
	public delegate void ButtonClickFunction();

	private ButtonClickFunction clickFunction;
	public Texture2D defaultImage, hoverImage, downClickImage;
	public string text;
	public Font font;
	public AudioClip sound;
	public bool selected = false;
	public float fadeTime;

	private AudioSource audio;
	private Texture2D currentImage;

	void Awake()
	{
		audio = gameObject.AddComponent<AudioSource> ();
		audio.clip = sound;

		//clickFunction = changeScenes;
	}

	void OnGUI()
	{
		if( enabled )
		{
			Vector3 point = Camera.main.WorldToScreenPoint( this.transform.position );
			Vector3 scale = this.transform.localScale;

			// set the GUI images and font
			GUI.skin.button.normal.background = ( Texture2D )defaultImage;
			GUI.skin.button.hover.background = ( Texture2D )hoverImage;
			GUI.skin.button.active.background = ( Texture2D )defaultImage;
			GUI.skin.font = font;
			GUI.skin.GetStyle( "Button" ).fontSize = Mathf.FloorToInt( 0.6f * scale.y );
			GUI.skin.GetStyle( "Button" ).normal.textColor = Color.black;
			GUI.depth = 0;

			// draw the button
			if( GUI.Button( new Rect( point.x - scale.x / 2, point.y - scale.y / 2, scale.x, scale.y ), text ) )
					ClickButton();
		}
	}

	public void SetClickAction( ButtonClickFunction i_function )
	{
		this.clickFunction = i_function;
	}

	public void ClickButton()
	{
		clickFunction();
	}

	public void Fade()
	{
		//TODO play some animation
		enabled = false;
	}

	public void Reveal()
	{
		enabled = true;
	}

	public void Hover()
	{
		//TODO use particles
	}
}
