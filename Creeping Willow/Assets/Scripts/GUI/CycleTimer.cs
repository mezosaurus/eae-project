using UnityEngine;
using System.Collections;

public class CycleTimer : MonoBehaviour
{
	public Texture2D back;
	public Texture2D cover;

	public int left = 10;
	public int top = 10;
	public int width = 50;
	public int height = 50;

	private int angle = 0;

	// Use this for initialization
	void Start ()
	{
	}
	
	// Update is called once per frame
	void Update ()
	{
		angle++;
	}

	void OnGUI ()
	{
		// draw the day/night cycle
		Matrix4x4 oldMatrix = GUI.matrix;
		GUIUtility.RotateAroundPivot (angle, new Vector2 (left + width / 2, top + height / 2));
		GUI.DrawTexture (new Rect (left, top, width, height), back);
		GUI.matrix = oldMatrix;

		// draw the cover
		GUI.DrawTexture (new Rect (left, top, width, height), cover);
	}
}
