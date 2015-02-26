using UnityEngine;
using System.Collections;

public class StationaryAIController : AIController 
{
	public float sittingTime;
	protected GameObject bench;
	private bool sitting = false;
	private float leaveTime;

	//private static string oldManWalkingKey = "direction";

	//private enum OldManWalkingDirection
	//{
		//STILL = 0,
		//LEFT = 1,
	//}

	protected override void GameUpdate () 
	{
		if (updateNPC ())
			return;

		// if lure is deleted
		if( nextPath == null ) return;

		Vector3 pathPosition = nextPath == bench ? getBenchOffsetVector() : nextPath.transform.position;
		Vector3 positionNPC = transform.position;
		float step = speed * Time.deltaTime;
		
		Vector3 movement = Vector3.MoveTowards (positionNPC, pathPosition, step);
		Vector3 direction = Vector3.Normalize(movement - transform.position);
		Vector3 biasPosition = new Vector3 (transform.position.x - movement.x, transform.position.y - movement.y);

		if (biasPosition.x == 0)
		{
			//To the right
			flipXScale(false);
			setAnimatorInteger(walkingKey, (int)WalkingDirection.STILL);
		}
		else
			setAnimatorInteger(walkingKey, (int)WalkingDirection.MOVING_DOWN);

		Vector3 changeMovement = lured ? Vector3.zero : avoid (direction);

		if( changeMovement != Vector3.zero )
		{
			Vector3 newPos = Vector3.MoveTowards(positionNPC,changeMovement,step);
			transform.position = newPos;
			determineDirectionChange (transform.position, newPos);
		}
		else
		{
			determineDirectionChange (transform.position, movement);
			transform.position = movement;
		}

		if (movement == pathPosition && (nextPath == bench || nextPath.tag.Equals (spawnTag)))
		{
			if (killSelf && nextPath != bench)
				destroyNPC();

			if (!sitting && nextPath == bench)
			{
				sitting = true;
				leaveTime = Time.time + sittingTime;
			}

			if (leaveTime <= Time.time && !nextPath.tag.Equals(spawnTag))
			{
				sitting = false;
				killSelf = true;
				nextPath = getLeavingPath();
			}
		}
		//objectAvoidance ();
	}

	public void setStationaryPoint(GameObject point)
	{
		bench = point;
//		bench.transform.position = new Vector3(point.transform.position.x, point.transform.position.y - gameObject.transform.renderer.bounds.size.y/5);
		nextPath = bench;
	}

	private Vector3 getBenchOffsetVector()
	{
		return new Vector3(bench.transform.position.x, bench.transform.position.y - gameObject.transform.renderer.bounds.size.y/5);

	}
	protected override void alert()
	{
		base.alert ();
		if (playGasp)
		{
			int rand = Random.Range (0, gasps.Count);
			AudioClip gasp = (AudioClip)gasps[rand];
			audio.PlayOneShot (gasp, 0.8f);
			playGasp = false;
		}
		setAnimatorInteger (walkingKey, (int)WalkingDirection.STILL);
	}

	override protected GameObject getNextPath()
	{
		//Debug.Log ("Child");
		return bench;
	}

	override protected void panic()
	{
		base.panic ();
		sitting = false;
		this.GetComponent<BoxCollider2D>().isTrigger = false;
		setAnimatorInteger (walkingKey, (int)WalkingDirection.MOVING_DOWN);
	}

	protected override void OnCollisionEnter2D(Collision2D collision)
	{
		base.OnCollisionEnter2D (collision);

		if (!killSelf && !panicked && collision.collider.Equals (bench.collider2D)) {
			this.GetComponent<BoxCollider2D> ().isTrigger = true;
		}
	}

	protected override void OnTriggerExit2D (Collider2D other)
	{
		if (other.Equals(bench.collider2D))
			this.GetComponent<BoxCollider2D>().isTrigger = false;
		base.OnTriggerExit2D (other);
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
