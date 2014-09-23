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
	private bool nearWall;
	private Vector3 moveDir;
	private float nextMoveTime;

	void Start()
	{
		float time = Time.time;
		nextMoveTime = time + waitTime;
	}

	void FixedUpdate () 
	{
		if (grabbed)
			return;
		if (alerted)
		{
			if (!audio.isPlaying)
			{
				audio.Play();
			}
			//Debug.Log ("Move: " + moveDir.x + ", " + moveDir.y);
			//if (nearWall)
			//{
				//nearWall = false;
				//moveDir.Scale(new Vector3(-1, -1, 0));
			//}
			rigidbody2D.velocity = moveDir.normalized * runSpeed;
			return;
		}
		// Five options for moving:
		//    Up, Down, Left, Right, Still

		float moveHorizontal = 0, moveVertical = 0, rand;

		// Get random direction or continue on same path
		rand = getRandomNumber ();

		float time = Time.time;
		if (Time.time >= nextMoveTime)
		{
			nextMoveTime = Time.time + waitTime;
			Vector2 randy = Random.insideUnitCircle * 2;
			Vector3 direction = new Vector3(randy.x, randy.y, 0.0f);
			rigidbody2D.velocity = direction * normalSpeed;
		}
		return;

		// Determine direction to move
		if (rand < 1) 
		{
			if (transform.position.x + normalSpeed >= boundary.xMax)
			{
				previousMovement = 1.5f;
				moveHorizontal = -1;
			}
			else
			{
				moveHorizontal = 1;
			}
		} 
		else if (rand < 2) 
		{
			if (transform.position.x - normalSpeed <= boundary.xMin)
			{
				previousMovement = 0.5f;
				moveHorizontal = 1;
			}
			else
			{
				moveHorizontal = -1;
			}
		}
		else if (rand < 3) 
		{
			if (transform.position.z + normalSpeed >= boundary.zMax)
			{
				previousMovement = 3.5f;
				moveVertical = -1;
			}
			else
			{
				moveVertical = 1;
			}
		}
		else if (rand < 4) 
		{
			if (transform.position.z - normalSpeed <= boundary.zMin)
			{
				previousMovement = 2.5f;
				moveVertical = 1;
			}
			else
			{
				moveVertical = -1;
			}
		}

		// Apply random movement to AI
		Vector3 movement = new Vector3 (moveHorizontal, 0.0f, moveVertical);
		rigidbody2D.velocity = movement * normalSpeed;
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

	void OnTriggerEnter2D(Collider2D other)
	{
		if (other.tag == "Player")
		{
			moveDir = transform.position - player.transform.position;
			alerted = true;
		}
		else if (other.tag == "Wall")
		{
			nearWall = true;
		}
	}

	void OnTriggerExit2D(Collider2D other)
	{
		if (other.tag == "Player")
		{
			alerted = false;
		}
		else if (other.tag == "Wall")
		{
			nearWall = false;
		}
	}
}
	