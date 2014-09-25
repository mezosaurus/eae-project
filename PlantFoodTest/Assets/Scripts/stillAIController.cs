using UnityEngine;
using System.Collections;

public class stillAIController : aiController 
{
	private bool nearWall;
	private Vector2 moveDir; 

	void Start()
	{
		audio.clip = chaseMusic;
	}
	
	void OnTriggerExit2D(Collider2D other)
	{
		if (other.tag == "Player")
			alerted = false;
	}

	void OnTriggerStay2D(Collider2D other)
	{
		if (other.tag == "Wall")
		{
			RaycastHit2D raycast = Physics2D.Raycast(transform.position, moveDir);
 			if (raycast.collider != null && raycast.collider.tag == "Wall")
			{
				float distance = Vector2.Distance(transform.position, raycast.point);
				if (distance < 1.5f)
					nearWall = true;
				else
					nearWall = false;
			}
		}
		else if (other.tag == "Player")
		{
			PlayerController controller = player.GetComponent<PlayerController>();
			if (controller.CurrentSpeed / controller.MaxSpeed >= 0.5)
			{
				alerted = true;
			
				moveDir = transform.position - player.transform.position;
			}
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
			rigidbody2D.velocity = movement;

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

		if (nearWall)
		{
			//return;
			nearWall = false;
			moveDir = Quaternion.AngleAxis(90, transform.forward) * -moveDir;
		}
		
		rigidbody2D.velocity = moveDir.normalized * runSpeed;
	}
}




//