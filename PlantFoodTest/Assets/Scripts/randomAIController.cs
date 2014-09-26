using UnityEngine;
using System.Collections;

[System.Serializable]
public class Boundary
{
	public float xMin, xMax, zMin, zMax;	
}

public class randomAIController : aiController 
{
	public float waitTime;
	public Boundary boundary;

	private float previousMovement;
	private int moveCounter;
	private float nextMoveTime;

	new void Start()
	{
		base.Start ();
		float time = Time.time;
		nextMoveTime = time + waitTime;
	}

	void FixedUpdate () 
	{
		if (grabbed || alerted)
			return;
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

		if (Time.time >= nextMoveTime)
		{
			nextMoveTime = Time.time + waitTime;
			Vector2 randy = Random.insideUnitCircle * 2;
			Vector3 direction = new Vector3(randy.x, randy.y, 0.0f);
			rigidbody2D.velocity = direction * normalSpeed;
		}
	}

	float getRandomNumber()
	{
		float rand;
		if (moveCounter == 0) 
		{
			//moveCounter = Mathf.FloorToInt(Random.Range(minMovementRepeat,maxMovementRepeat));
			rand = Random.Range (0, 5);
			previousMovement = rand;
		} 
		else 
		{
			moveCounter--;
			rand = previousMovement;
		}

		return rand;
	}

//	void OnTriggerEnter2D(Collider2D other)
//	{
//		if (other.tag == "Player")
//		{
//			moveDir = transform.position - player.transform.position;
//			alerted = true;
//		}
//		else if (other.tag == "Wall")
//		{
//			nearWall = true;
//		}
//	}
//
//	void OnTriggerExit2D(Collider2D other)
//	{
//		if (other.tag == "Player")
//		{
//			alerted = false;
//		}
//		else if (other.tag == "Wall")
//		{
//			nearWall = false;
//		}
//	}
//
//	void OnTriggerStay2D(Collider2D other)
//	{
//		if (other.tag == "Wall")
//		{
//			RaycastHit2D raycast = Physics2D.Raycast(transform.position, moveDir);
//			if (raycast.collider != null && raycast.collider.tag == "Wall")
//			{
//				float distance = Vector2.Distance(transform.position, raycast.point);
//				if (distance < 1.5f)
//					nearWall = true;
//				else
//					nearWall = false;
//			}
//		}
//	}
}
	