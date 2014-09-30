using UnityEngine;
using System.Collections;
using System;

public class GameTimer : MonoBehaviour
{
	private static float prev_time;
	public float time;
	public Font timerFont;
	public Texture2D boxImage;
	private GUIStyle boxStyle;
	private String timerPopup;
	private float popupAlpha;

	// Use this for initialization
	void Start ()
	{
		boxStyle = new GUIStyle ();
		boxStyle.fontSize = 85;
		boxStyle.alignment = TextAnchor.MiddleCenter;
		boxStyle.font = timerFont;
		boxStyle.wordWrap = true;

		timerPopup = "+0";
		popupAlpha = 0.0f;

		Globals.gameTimer = this;
	}

	public static void StartTimer ()
	{
		GameTimer.prev_time = Time.time;
	}

	// Update is called once per frame
	void Update ()
	{
		if (Globals.GameState == GameState.INLEVEL_DEFAULT || Globals.GameState == GameState.INLEVEL_CHASE || Globals.GameState == GameState.INLEVEL_EATING)
		{
			if (time > 0) {
				time -= Time.time - prev_time;
				GameTimer.prev_time = Time.time;
			}
			if (time <= 0) {
				time = 0;
				Globals.GameState = GameState.ENDOFLEVEL_DEFEAT;
			}
		}
		GameTimer.prev_time = Time.time;

		popupAlpha = Math.Max (popupAlpha - 0.01f, 0.0f);
	}

	void OnGUI ()
	{
		GUI.matrix = Globals.PrepareMatrix ();

		// prepare the label tag for the timer
		GUI.skin.GetStyle ("Label").fontSize = 85;
		GUI.skin.GetStyle ("Label").alignment = TextAnchor.MiddleCenter;
		GUI.skin.GetStyle ("Label").font = timerFont;

		float left = 96.0f;
		float top = 48.0f;

		int width = 100;
		int height = 100;
        
        GUI.Box(new Rect(left, top, width, height), "");
		GUI.Label (new Rect (left, top, width, height), Mathf.FloorToInt (time).ToString ());

		GUI.skin.GetStyle ("Label").fontSize = 40;
		GUI.color = new Color (1, 1, 1, popupAlpha);
		GUI.Label (new Rect (left + 2 * width / 3, top, width, height / 2), timerPopup);
	}

	public void AddTime( int time )
	{
		this.time += time;
		timerPopup = "+" + time.ToString ();
		popupAlpha = 1.0f;
	}
}