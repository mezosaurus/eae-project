using UnityEngine;
using System.Collections;

public class CustomFontScript : MonoBehaviour {

	public Font myFont;
	public Material myMaterial;
	GUIStyle myStyle = new GUIStyle ();

	Texture2D[] pics;
	int score = 0;

	// Use this for initialization
	void Start () {
		myStyle.font = myFont;
		myStyle.font.material = myMaterial;
	}
	
	// Update is called once per frame
	void Update () {
	
	}

	void OnGUI()
	{
		//GUI.Label (new Rect (200, 200, 400, 50), "asdfghjklqwertyuiopzxcvbnmQWERTYUIOPASDFGHJKLZXCVBNM1234567890", myStyle);


		FontConverter.instance.parseStringToTextures ( 200, 200, 40, 50, "score " + score );

		score++;
	}
}
