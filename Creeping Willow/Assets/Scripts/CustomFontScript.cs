using UnityEngine;
using System.Collections;

public class CustomFontScript : MonoBehaviour {

	public Font myFont;
	GUIStyle myStyle = new GUIStyle ();

	// Use this for initialization
	void Start () {
		myStyle.font = myFont;
	}
	
	// Update is called once per frame
	void Update () {
	
	}

	void OnGUI()
	{
		GUI.Label (new Rect (300, 200, 400, 50), "asdfghjklqwertyuiopzxcvbnm", myStyle);
	}
}
