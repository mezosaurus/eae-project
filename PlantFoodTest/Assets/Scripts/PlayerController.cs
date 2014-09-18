using UnityEngine;
using System.Collections;

[System.Serializable]

public class PlayerController : MonoBehaviour 
{
	public float speed;
	public Boundary boundary;

	void FixedUpdate()
	{
		float moveHorizontal = Input.GetAxis ("Horizontal");
		float moveVertical = Input.GetAxis ("Vertical");

		Vector3 movement = new Vector3 (moveHorizontal, moveVertical, 0.0f);
		rigidbody.velocity = movement * speed;
	}
}
