using UnityEngine;
using System.Collections;

public class TextureScript : MonoBehaviour {

	public GameObject target;

	// Use this for initialization
	void Start () {
	
	}
	
	// Update is called once per frame
	void Update () {
		if (target != null)
		{
			//renderer.enabled = true;
			//renderer.enabled = true;
			//Debug.Log (target.position);
			//var wantedPos = Camera.main.WorldToScreenPoint (target.position);
			Vector2 pos = new Vector2(target.transform.position.x, (target.transform.position.y + target.renderer.bounds.size.y));
			transform.position = pos;
		}
	}
}
