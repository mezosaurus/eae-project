using UnityEngine;
using System.Collections;

public class AbilityBar : MonoBehaviour
{
	public Texture2D ability1;
	public Texture2D ability2;
	public Texture2D ability3;
	public Texture2D ability4;
	public Texture2D cover;

	public Texture2D button1;
	public Texture2D button2;
	public Texture2D button3;
	public Texture2D button4;

	public int left = 10;
	public int top = 500;
	public int width = 200;
	public int height = 50;

	// Use this for initialization
	void Start ()
	{
	}
	
	// Update is called once per frame
	void Update ()
	{
	}

	void OnGUI()
	{
		// draw the abilities
		if (ability1 != null)
			GUI.DrawTexture (new Rect (left, top, width * 0.25f, height), ability1);

		if (ability2 != null)
			GUI.DrawTexture (new Rect (left + width * 0.25f, top, width * 0.25f, height), ability2);

		if (ability3 != null)
			GUI.DrawTexture (new Rect (left + width * 0.5f, top, width * 0.25f, height), ability3);

		if (ability4 != null)
			GUI.DrawTexture (new Rect (left + width * 0.75f, top, width * 0.25f, height), ability4);

		// draw the cover
		GUI.DrawTexture (new Rect (left, top, width * 1.05f, height), cover);

		// draw the buttons
		GUI.DrawTexture (new Rect (left + width * 0.075f, top + height * 0.75f, width * 0.1f, height * 0.4f), button1);
		GUI.DrawTexture (new Rect (left + width * 0.325f, top + height * 0.75f, width * 0.1f, height * 0.4f), button2);
		GUI.DrawTexture (new Rect (left + width * 0.575f, top + height * 0.75f, width * 0.1f, height * 0.4f), button3);
		GUI.DrawTexture (new Rect (left + width * 0.825f, top + height * 0.75f, width * 0.1f, height * 0.4f), button4);
	}
}
