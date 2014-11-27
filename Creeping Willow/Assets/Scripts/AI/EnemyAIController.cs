using UnityEngine;	
using System.Collections;

public class EnemyAIController : AIController 
{
	// OLD Variables
	//public Transform[] moveWayPoints;
	
	//private int wayPointIndex;
	private bool reverseDirection;
	
	new void Start()
	{
		base.Start ();
		
		// Get path for AI
		nextPath = movePath.getNextPath (null);
		
		//reverseDirection = false;	// OLD
	}
	
	public void setMovingPath(SubpathScript movePath)
	{
		this.movePath = movePath;
	}
	
	override protected GameObject getNextPath()
	{
		return movePath.getNextPath(null);
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
		//Vector3 goal = GameObject.Find ("SpawnMoves/SpawnMove1").transform.position;
		float step = speed * Time.deltaTime;
		Vector3 movement = Vector3.MoveTowards (position, pathPosition, step);
		//Vector3 movement = Vector3.MoveTowards (position, spawnMove, step);
		determineDirectionChange(transform.position, movement);
		transform.position = movement;
		
		if (movement == pathPosition && !lured)
		{
			if (killSelf && nextPath.gameObject.tag.Equals("Respawn"))
				Destroy(gameObject);
			
			int max = 10;
			int rand = Random.Range (0, max);
			if (rand < max - 1)
				nextPath = movePath.getNextPath(nextPath);
			else
			{
				killSelf = true;
				nextPath = getLeavingPath();
			}
		}
	}	
}
