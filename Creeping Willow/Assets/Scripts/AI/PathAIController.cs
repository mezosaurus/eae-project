using UnityEngine;	
using System.Collections;

public class PathAIController : AIController 
{
	protected bool inMaze = false;

	new void Start()
	{
		base.Start ();

		// Get path for AI
		nextPath = movePath.getNextPath (inMaze ? gameObject : null, gameObject);
	}

	public void setMovingPath(SubpathScript movePath)
	{
		this.movePath = movePath;
	}

	override protected GameObject getNextPath()
	{
		return movePath.getNextPath(null, gameObject);
	}
	
	// Update is called once per frame
	protected override void GameUpdate () 
	{
		if (updateNPC())
			return;

		// if lure is deleted
		if( nextPath == null ) return;

		Vector3 pathPosition = nextPath.transform.position;
		Vector3 position = transform.position;
		float step = speed * Time.deltaTime;
		Vector3 movement = Vector3.MoveTowards (position, pathPosition, step);
		Vector3 direction = Vector3.Normalize(movement - transform.position);

		Vector3 biasPosition = new Vector3 (transform.position.x - movement.x, transform.position.y - movement.y);
		bool flipX = false;

		if (biasPosition.x == 0)
		{
			setAnimatorInteger(walkingKey, (int)WalkingDirection.STILL);
		}
		else if (biasPosition.y >= 0)
		{
			setAnimatorInteger(walkingKey, (int)WalkingDirection.MOVING_DOWN);
		}
		else
		{
			flipX = true;
			setAnimatorInteger(walkingKey, (int)WalkingDirection.MOVING_UP);
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

		if (flipX)
		{
			flipXScale(biasPosition.x > 0);
		}
		if (movement == pathPosition && !lured)
		{
			if (killSelf)
			{
				string tag = nextPath.gameObject.tag;
				if (tag.Equals("MazePathEnter"))
				{
					nextPath = getLeavingPath();
				}
				else if (tag.Equals("Respawn"))
				{
					destroyNPC();
				}
				return;
			}

			int max = 10;
			int rand = Random.Range (0, max);
			if (rand == max - 1)
			{
				if (inMaze && nextPath.tag.Equals("MazePath1Away"))
				{
					nextPath = GameObject.FindGameObjectWithTag("MazePathEnter");
					killSelf = true;
					return;
				}

				if (!inMaze || nextPath.gameObject.tag.Equals("MazePathEnter"))
				{
					killSelf = true;
					nextPath = getLeavingPath();
					return;
				}
			}
			nextPath = movePath.getNextPath(nextPath, gameObject);
		}
	}

	public void setInMaze(bool maze)
	{
		inMaze = maze;
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

