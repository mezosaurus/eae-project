using UnityEngine;
using System.Collections;

public class stillAIController : aiController 
{
	void Update()
	{
		if (panicked)
		{
			timePanicked -= Time.deltaTime;
			if (timePanicked <= 0) {
				panicked = false;
				GetComponent<SpriteRenderer>().sprite = normalTexture;
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
				moveDir = Quaternion.AngleAxis(90, transform.forward) * -moveDir;
			}
			
			rigidbody2D.velocity = moveDir.normalized * runSpeed;
			return;
		}
		/*if (grabbed || alerted)
			return;

		Vector3 movement;
		/*if (!panicked) 
		{
			movement = new Vector3 (0.0f, 0.0f, 0.0f);
			rigidbody2D.velocity = movement;

			if (audio.isPlaying)
			{
				//audio.Pause();
			}

			return;
		}
		if (panicked) {
			panicCooldown -= Time.deltaTime;
			if (panicCooldown == 0) {
				panicked = false;
				movement = new Vector3 (0.0f, 0.0f, 0.0f);
				rigidbody2D.velocity = movement;
				if (audio.isPlaying)
				{
					//audio.Pause();
				}
				GetComponent<SpriteRenderer>().sprite = normalTexture;
				panicCooldown = 20;
			}
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
		
		rigidbody2D.velocity = moveDir.normalized * runSpeed;*/
	}
}




//