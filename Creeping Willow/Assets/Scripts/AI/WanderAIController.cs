using UnityEngine;
using System.Collections;

public class WanderAIController : AIController {

	public float waitTime;
	public string wanderTag = "Wander";

	private float previousMovement;
	private int moveCounter;
	private float nextMoveTime;
    private Vector2 pathPosition;
	private bool isOnPath;
	private GameObject[] wanderPoints;
		
	new void Start()
	{
		base.Start ();
		float time = Time.time;
		nextMoveTime = time + waitTime;
		wanderPoints = GameObject.FindGameObjectsWithTag(wanderTag);

		isOnPath = true;
		pathPosition = getRandomWanderPoint();

		nextPath = createNextPath (pathPosition);
	}

	override protected void NPCOnDestroy()
	{
		if (nextPath != null && nextPath.tag.Equals(wanderTag))
		{
			Destroy (nextPath);
		}
	}

	protected override void GameUpdate () 
	{
		if (updateNPC())
		{
			return;
		}

		if (nextPath == null)
		{
			return;
		}
		if (isOnPath)
		{
			move();
		}
		else if (Time.time >= nextMoveTime)
		{
			nextMoveTime = Time.time + waitTime * 15;
			move ();
		}
	}

	protected void move()
	{
		if (!isOnPath)
		{
			isOnPath = true;
			pathPosition = pathPosition + Random.insideUnitCircle * 2;
			nextPath.transform.position = pathPosition;
		}

		Vector3 positionNPC = transform.position;
		Vector3 positionNextPath = nextPath.transform.position;
		float step = speed * Time.deltaTime;
		
		Vector3 movement = Vector3.MoveTowards (positionNPC, positionNextPath, step);
		Vector3 direction = Vector3.Normalize(movement - transform.position);
		Vector3 biasPosition = new Vector3 (transform.position.x - movement.x, transform.position.y - movement.y);
		
		if (biasPosition.x == 0 && biasPosition.y == 0)
		{
			setAnimatorInteger(walkingKey, (int)WalkingDirection.STILL_DOWN_LEFT);
		}
		else if (biasPosition.y >= 0)
		{
			setAnimatorInteger(walkingKey, (int)WalkingDirection.MOVING_DOWN_LEFT);
		}
		else
		{
			setAnimatorInteger(walkingKey, (int)WalkingDirection.MOVING_UP_LEFT);
		}

		Vector3 changeMovement = lured ? Vector3.zero : avoid (direction);
		if( changeMovement != Vector3.zero )
		{	
			Vector3 newPos = Vector3.MoveTowards(transform.position,changeMovement,step);
			determineDirectionChange(transform.position, newPos);
			transform.position = newPos;
		}
		else
		{
			determineDirectionChange(transform.position, movement);
			transform.position = movement;
		}

		if (movement == new Vector3(positionNextPath.x, positionNextPath.y) && !lured)
		{
			if (killSelf)
				destroyNPC();
			
			int max = 5;
			int rand = Random.Range (0, max);
			if (rand < max - 1 || marked)
			{
				nextMoveTime = Time.time + waitTime;
				isOnPath = false;
				setAnimatorInteger(walkingKey, (int)WalkingDirection.STILL_DOWN_LEFT);
			}
			else
			{
				killSelf = true;
				pathPosition = getLeavingPath().transform.position;
				nextPath.transform.position = pathPosition;
			}
		}
	}
	
	float getRandomNumber()
	{
		float rand;
		if (moveCounter == 0) 
		{
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

	Vector2 getRandomWanderPoint()
	{
		int rand = Random.Range (0, wanderPoints.Length);
		return wanderPoints [rand].transform.position;
	}

	override protected GameObject getNextPath()
	{
		nextPath = createNextPath (pathPosition);
		return nextPath;
	}

	protected override void alert()
	{
		base.alert ();
		setAnimatorInteger (walkingKey, (int)WalkingDirection.STILL_DOWN_LEFT);
	}

	override protected void panic()
	{
		base.panic ();
		setAnimatorInteger (walkingKey, (int)WalkingDirection.MOVING_DOWN_LEFT);
	}

	override protected void lure(Vector3 lurePosition)
	{
		if (nextPath.tag.Equals(wanderTag))
			Destroy (nextPath);
		base.lure (lurePosition);
	}

	private GameObject createNextPath(Vector3 position)
	{
		GameObject path = new GameObject();
		path.tag = wanderTag;
		path.transform.position = position;
		return path;
	}
}

