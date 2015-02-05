﻿using UnityEngine;
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
		nextPath = new GameObject();
		pathPosition = getRandomWanderPoint();
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
		float step = speed * Time.deltaTime;
		
		Vector3 movement = Vector3.MoveTowards (positionNPC, pathPosition, step);
		Vector3 direction = Vector3.Normalize(movement - transform.position);
		Vector3 biasPosition = new Vector3 (transform.position.x - movement.x, transform.position.y - movement.y);
		
		if (biasPosition.x == 0 && biasPosition.y == 0)
		{
			setAnimatorInteger(walkingKey, (int)WalkingDirection.STILL);
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
			
			int max = 5;
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

	protected override void alert()
	{
		base.alert ();
		setAnimatorInteger (walkingKey, (int)WalkingDirection.STILL);
	}

	override protected void panic()
	{
		base.panic ();
		setAnimatorInteger (walkingKey, (int)WalkingDirection.MOVING_DOWN);
	}
}

