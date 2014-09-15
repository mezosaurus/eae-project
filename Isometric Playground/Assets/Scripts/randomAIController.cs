using UnityEngine;
using System.Collections;

[System.Serializable]
public class Boundary
{
	public float xMin, xMax, zMin, zMax;
	
}

public class randomAIController : aiController 
{
	public int minMovementRepeat, maxMovementRepeat;
	public Boundary boundary;

	private float previousMovement;
	private int moveCounter;

	void FixedUpdate () 
	{
		// Five options for moving:
		//    Up, Down, Left, Right, Still

		float moveHorizontal = 0, moveVertical = 0, rand;

		// Get random direction or continue on same path
		rand = getRandomNumber ();

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
		rigidbody.velocity = movement * normalSpeed;
	}

	float getRandomNumber()
	{
		float rand;
		if (moveCounter == 0) 
		{
			moveCounter = Mathf.FloorToInt(Random.Range(minMovementRepeat,maxMovementRepeat));
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
}
