using UnityEngine;
using System.Collections;

public class StationaryAIController : AIController 
{
	public float sittingTime;
	protected GameObject bench;
	private bool sitting = false;
	private float leaveTime;

	private static string oldManWalkingKey = "direction";

	private enum OldManWalkingDirection
	{
		STILL = 0,
		LEFT = 1,
		RIGHT = 2
	}

	protected override void GameUpdate () 
	{
		/*
		if (grabbed)
			return;

        if (panicked)
        {
            timePanicked -= Time.deltaTime;
            if (timePanicked <= 0)
            {
                panicked = false;
				alertLevel = alertThreshold - 0.1f;
                speed = 1;
				NPCAlertLevelMessage message = new NPCAlertLevelMessage (gameObject, AlertLevelType.Normal);
				MessageCenter.Instance.Broadcast (message);
                //GetComponent<SpriteRenderer>().sprite = normalTexture;
                return;
            }
            /*if (!audio.isPlaying)
            {
                //audio.Play();
            }*//*
            //Debug.Log ("Move: " + moveDir.x + ", " + moveDir.y);
            if (nearWall)
            {
                nearWall = false;
                moveDir = Quaternion.AngleAxis(90, transform.forward) * -moveDir;
            }

            rigidbody2D.velocity = moveDir.normalized * speed;
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
		if (updateNPC ())
			return;
		

		// if lure is deleted
		if( nextPath == null ) return;

		Vector3 pathPosition = nextPath.transform.position;
		Vector3 positionNPC = transform.position;
		float step = speed * Time.deltaTime;

		Vector3 movement = Vector3.MoveTowards (positionNPC, pathPosition, step);
		determineDirectionChange (transform.position, movement);
		Vector3 biasPosition = new Vector3 (transform.position.x - movement.x, transform.position.y - movement.y);

		if (biasPosition.x == 0)
		{
			//To the right
			setAnimatorInteger(oldManWalkingKey, (int)OldManWalkingDirection.STILL);
		}
		else
			setAnimatorInteger(oldManWalkingKey, (int)OldManWalkingDirection.LEFT);

		transform.position = movement;

		if (movement == pathPosition && (nextPath == bench || nextPath.tag.Equals (spawnTag)))
		{
			if (killSelf && nextPath != bench)
				Destroy(gameObject);

			if (!sitting)
			{
				sitting = true;
				leaveTime = Time.time + sittingTime;
			}

			if (leaveTime <= Time.time)
			{
				killSelf = true;
				nextPath = getLeavingPath();
			}
		}
	}

	public void setStationaryPoint(GameObject point)
	{
		bench = new GameObject ();
		bench.transform.position = new Vector3(point.transform.position.x, point.transform.position.y - gameObject.transform.renderer.bounds.size.y/5);
		nextPath = bench;
	}

	protected override void alert()
	{
		base.alert ();
		setAnimatorInteger (oldManWalkingKey, (int)OldManWalkingDirection.STILL);
	}

	override protected GameObject getNextPath()
	{
		//Debug.Log ("Child");
		return bench;
	}
	/*
	 * Old Stuff
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
				//audio.Play();
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
		
		rigidbody2D.velocity = moveDir.normalized * runSpeed;
	}
	*/
}
