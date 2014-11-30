using UnityEngine;	
using System.Collections;

public class PathAIController : AIController 
{
	// OLD Variables
	//public Transform[] moveWayPoints;
	
	//private int wayPointIndex;
	//private bool reverseDirection;

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
		/*
		if (grabbed || alerted)
			return;

        if (panicked)
        {
            timePanicked -= Time.deltaTime;
            if (timePanicked <= 0)
            {
                panicked = false;
                speed = 1;
                alertLevel = alertThreshold - 0.1f;
				NPCAlertLevelMessage message = new NPCAlertLevelMessage (gameObject, AlertLevelType.Normal);
				MessageCenter.Instance.Broadcast (message);
                //GetComponent<SpriteRenderer>().sprite = normalTexture;
                return;
            }
            if (nearWall)
            {
                nearWall = false;
                moveDir = Quaternion.AngleAxis(90, transform.forward) * -moveDir;
            }
            else
                rigidbody2D.velocity = moveDir.normalized * speed;

            /*if (!audio.isPlaying)
            {
                //audio.Play();
            }//*//*

            return;
        }

		if (playerInRange)
		{
			Vector2 playerSpeed = player.rigidbody2D.velocity;
			if (playerSpeed == Vector2.zero && alertLevel > 0)
			{
				// decrement alert level
				alertLevel -= (panicThreshold * 0.05f);
			}
		}
		else if (alertLevel > 0)
		{
			alertLevel -= (panicThreshold * 0.05f);
		}
		// Make sure alert level does not go below 0
		if (alertLevel < 0)
			alertLevel = 0;
		*/
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
				destroyNPC();
			
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
	//	void OnTriggerEnter2D(Collider2D other)
	//	{
	//		if (other.tag == "Player")
	//		{
	//			alerted = true;
	//			
	//			moveDir = transform.position - player.transform.position;
	//		}
	//	}
	//
	//	void OnTriggerExit2D(Collider2D other)
	//	{
	//		if (other.tag == "Player")
	//			alerted = false;
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

	/*
	 * Old Update
	void Update ()
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
			if (nearWall)
			{
				nearWall = false;
				moveDir = Quaternion.AngleAxis(90, transform.forward) * -moveDir;
			}
			else
				rigidbody2D.velocity = moveDir.normalized * runSpeed;
			
			if (!audio.isPlaying)
			{
				//audio.Play();
			}
			
			return;
		}
		if(transform.position == moveWayPoints[wayPointIndex].position)
		{
			if (wayPointIndex == moveWayPoints.Length - 1)
				reverseDirection = true;
			else if (wayPointIndex == 0)
				reverseDirection = false;
			
			if (reverseDirection)
				wayPointIndex--;
			else wayPointIndex++;
		}
		
		//Debug.Log ("Way point: " + wayPointIndex);
		Vector3 position = transform.position;
		Vector3 goal = moveWayPoints [wayPointIndex].position;
		float step = normalSpeed * Time.deltaTime;
		Vector3 movement = Vector3.MoveTowards (position, goal, step);
		//if (movement > goal)
		//{
		//Debug.Log ("Setting to goal position");
		//transform.position = goal;
		//}
		transform.position = movement;
	}
	*/
}
