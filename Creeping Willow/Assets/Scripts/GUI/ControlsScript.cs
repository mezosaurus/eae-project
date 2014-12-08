using UnityEngine;
using System.Collections;

public class ControlsScript : MonoBehaviour {

	public GUITexture controlsMap;
	public bool nextBtnTapped;

	// Use this for initialization
	void Start () {
		nextBtnTapped = false;
		controlsMap.enabled = true;
	}
	
	// Update is called once per frame
	void Update () {
		if (Input.GetButtonDown("A"))
		{
			controlsMap.enabled = false;
			nextBtnTapped = true;
		}
	}
}
