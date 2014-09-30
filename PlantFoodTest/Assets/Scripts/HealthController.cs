using UnityEngine;
using System.Collections;
using System;

public class HealthController : MonoBehaviour
{
	public int playerHealth, maxHealth;
	private Rect healthBarRect;
	private String healthPopup;
	private float popupAlpha;
	
	// Use this for initialization
	void Start ()
	{
		int width = 200;
		int height = 20;

        //healthBarRect = new Rect (new Rect ((Globals.originalWidth / 2 - width / 2), 130, width, height));
        healthBarRect = new Rect(new Rect(96f, 150f, width, height));
		Globals.healthController = this;

		healthPopup = "+0";
		popupAlpha = 0.0f;
	}
	
	// Update is called once per frame
	void Update ()
	{
		popupAlpha = Math.Max (popupAlpha - 0.01f, 0.0f);
	}
	
	void OnGUI ()
	{
        if (Globals.GameState == GameState.INLEVEL_DEFAULT || Globals.GameState == GameState.INLEVEL_CHASE || Globals.GameState == GameState.INLEVEL_EATING || Globals.GameState == GameState.INLEVEL_EATING_CINEMATIC )
		{
			GUI.matrix = Globals.PrepareMatrix ();

			GUIStyle g2 = new GUIStyle ();
			Texture2D t2 = new Texture2D (200, 20);
			
			g2.normal.background = t2;

			GUI.BeginGroup (healthBarRect);
			GUI.Box (new Rect(0, 0, healthBarRect.width, healthBarRect.height), "" );

			GUI.BeginGroup (new Rect (0, 0, healthBarRect.width * playerHealth / maxHealth, healthBarRect.height));
			GUI.Box (new Rect(0, 0, healthBarRect.width, healthBarRect.height), GUIContent.none, g2);
			GUI.EndGroup ();
			GUI.EndGroup ();

			GUI.skin.GetStyle ("Label").fontSize = 40;
			GUI.color = new Color (1, 1, 1, popupAlpha);
			GUI.Label (new Rect (96 + 100, 150, 200, 50), healthPopup);
		}
	}

	public void AddHealth( int health )
	{
		playerHealth += health;
		healthPopup = "+" + health.ToString ();
		popupAlpha = 1.0f;
	}
}
