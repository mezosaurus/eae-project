using UnityEngine;
using System.Collections;

public class HealthController : MonoBehaviour
{
	public int playerHealth, maxHealth;
	private Rect healthBarRect;
	
	// Use this for initialization
	void Start ()
	{
		int width = 200;
		int height = 20;

		healthBarRect = new Rect (new Rect ((Globals.originalWidth / 2 - width / 2), 130, width, height));
	}
	
	// Update is called once per frame
	void Update ()
	{
	}
	
	void OnGUI ()
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
	}

	public void AddHealth( int health )
	{
		playerHealth += health;
	}
}
