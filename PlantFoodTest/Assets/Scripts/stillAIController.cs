using UnityEngine;
using System.Collections;

public class stillAIController : aiController 
{
	private bool nearWall;
	private Vector3 moveDir; 

	void OnTriggerEnter(Collider other)
	{
		//Debug.Log ("Enter: " + other.tag);
		if (other.tag == "Player")
		{
			alerted = true;

			moveDir = transform.position - player.transform.position;
		}
		else if (other.tag == "Wall")
		{
			nearWall = true;
		}
	}

	void OnTriggerExit(Collider other)
	{
		if (other.tag == "Player")
			alerted = false;
		else if (other.tag == "Wall")
		{
			nearWall = false;
		}
	}

	void Update()
	{
		Vector3 movement;
		if (!alerted) 
		{
			movement = new Vector3 (0.0f, 0.0f, 0.0f);
			rigidbody.velocity = movement;
			return;
		}

		//Debug.Log ("Move: " + moveDir.x + ", " + moveDir.y);
		rigidbody.velocity = moveDir.normalized * runSpeed;
		Debug.Log ("Velocity: " + rigidbody.velocity.magnitude);
	}
}