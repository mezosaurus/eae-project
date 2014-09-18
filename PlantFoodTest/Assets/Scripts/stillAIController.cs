using UnityEngine;
using System.Collections;

public class stillAIController : aiController 
{
	private bool nearWall;
	private Vector3 moveDir; 

	void Start()
	{
		audio.clip = chaseMusic;
	}

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

			//transform.rotation = Quaternion.AngleAxis(180, new Vector3(90, 0, 0)) * transform.rotation;
		}
	}

	void Update()
	{
		if (grabbed)
			return;

		Vector3 movement;
		if (!alerted) 
		{
			movement = new Vector3 (0.0f, 0.0f, 0.0f);
			rigidbody.velocity = movement;

			if (audio.isPlaying)
			{
				//audio.Pause();
			}

			return;
		}

		if (!audio.isPlaying)
		{
			audio.Play();
		}
		//Debug.Log ("Move: " + moveDir.x + ", " + moveDir.y);
		if (nearWall)
		{
			nearWall = false;
			moveDir.Scale(new Vector3(-1, -1, 0));
		}
		rigidbody.velocity = moveDir.normalized * runSpeed;
	}
}




//