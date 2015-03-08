using UnityEngine;
using System.Collections;

public class HintTextureScript : MonoBehaviour {

	public GameObject target;

	// Use this for initialization
	void Start () {
	
	}
	
	// Update is called once per frame
	void Update () {
		if (target != null)
		{
			PossessableItem item = target.GetComponent<PossessableItem>();
			Vector2 hintOffset = item.hintOffset;
			Vector2 pos = new Vector2(target.transform.position.x + hintOffset.x, (target.transform.position.y + hintOffset.y));
			transform.position = pos;
		}
	}
}
