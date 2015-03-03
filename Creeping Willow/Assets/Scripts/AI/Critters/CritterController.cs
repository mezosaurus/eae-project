using UnityEngine;
using System.Collections;

public enum CritterType
{
	fast,
	poisonous,
	stealthy,
}

public class CritterController : AIController {
	public float waitTime;
	public string wanderTag = "Wander";
	public CritterType critterUpgradeType;

	private float previousMovement;
	private int moveCounter;
	private float nextMoveTime;
	private Vector2 pathPosition;
	private Vector2 spawnPosition;
	private bool isOnPath;

	// critter sounds
	public AudioClip idleSound;
	public AudioClip walkSound;

	new void Start()
	{
		//Debug.Log ("start");
		base.Start ();
		float time = Time.time;
		nextMoveTime = time + waitTime;

		nextPath = new GameObject();
		pathPosition = spawnPosition;

		isCritterType = true;

        if (Random.Range(0f, 1f) > 0.5f) critterUpgradeType = CritterType.fast;
        else critterUpgradeType = CritterType.poisonous;

        SkinType = NPCSkinType.Critter;
	}
	
	protected override void GameUpdate () 
	{
		if (updateNPC())
		{
			return;
		}
		
		if (isOnPath)
		{
			move();
		}
		else if (!killSelf && Time.time >= nextMoveTime)
		{
			//Debug.Log ("new path");
			nextMoveTime = Time.time + waitTime * 15;
			move ();
		}
	}
	
	protected void move()
	{
		if (!isOnPath || (!killSelf && Time.time >= nextMoveTime))
		{
			nextMoveTime = Time.time + waitTime;

			isOnPath = true;
			pathPosition = spawnPosition + Random.insideUnitCircle * 2;
			nextPath.transform.position = pathPosition;
		}
		
		Vector3 positionNPC = transform.position;
		float step = speed * Time.deltaTime;

		Vector3 movement = Vector3.MoveTowards (positionNPC, nextPath.transform.position, step);
		Vector3 direction = Vector3.Normalize(movement - transform.position);
		Vector3 biasPosition = new Vector3 (transform.position.x - movement.x, transform.position.y - movement.y);

		if (biasPosition.x == 0 && biasPosition.y == 0)
		{
			setAnimatorInteger(walkingKey, (int)WalkingDirection.STILL);
			audio.PlayOneShot (idleSound, 0.7f);
		}
		else if (biasPosition.y >= 0)
		{
			setAnimatorInteger(walkingKey, (int)WalkingDirection.MOVING_DOWN);
		}
		else
		{
			setAnimatorInteger(walkingKey, (int)WalkingDirection.MOVING_UP);
		}
		Vector3 changeMovement = avoid (direction);
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
		
		if (movement == new Vector3(pathPosition.x, pathPosition.y) && !lured)
		{
			if (killSelf)
				destroyNPC();
			
			int max = 4;
			int rand = Random.Range (0, max);
			if (rand < max - 1)
			{
				nextMoveTime = Time.time + waitTime;
				isOnPath = false;
				setAnimatorInteger(walkingKey, (int)WalkingDirection.STILL);
			}
			else
			{
				killSelf = true;
				isOnPath = true;
				pathPosition = spawnPosition;
				nextPath.transform.position = pathPosition;
			}
		}
	}

	protected override void alert()
	{
		return;
		//base.alert ();
		//setAnimatorInteger (walkingKey, (int)WalkingDirection.STILL);
	}
	
	override protected void panic()
	{
		return;
		//base.panic ();
		//setAnimatorInteger (walkingKey, (int)WalkingDirection.MOVING);
	}

	public void setSpawnPosition(Vector2 spawn)
	{
		spawnPosition = spawn;
	}
}
