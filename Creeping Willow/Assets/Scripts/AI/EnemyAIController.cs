using UnityEngine;	
using System.Collections;

public class EnemyAIController : AIController 
{
	public float sittingTime;
	protected Vector3 panickedNPCPosition;
	private bool sitting = false;
	private float leaveTime;
	private bool calledToPoint = false;

	private static string axeManWalkingKey = "direction";
	
	private enum AxeManWalkingDirection
	{
		STILL = 0,
		LEFT = 1,
		RIGHT = 2
	}
	
	protected override void GameUpdate () 
	{
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
			//setAnimatorInteger(axeManWalkingKey, (int)AxeManWalkingDirection.STILL);
		}
		else
			//setAnimatorInteger(axeManWalkingKey, (int)AxeManWalkingDirection.LEFT);
		
		transform.position = movement;

		if (movement == pathPosition && (nextPath.transform.position.Equals(panickedNPCPosition) || killSelf))
		{
			if (killSelf && !nextPath.transform.position.Equals(panickedNPCPosition))
				Destroy(gameObject);
			
			if (!sitting)
			{
				sitting = true;
				leaveTime = Time.time + sittingTime;
			}
			
			if (leaveTime <= Time.time)
			{
				sitting = false;
				killSelf = true;
				if (calledToPoint)
				{
					calledToPoint = false;
					Destroy(nextPath);
				}
				nextPath = getLeavingPath();
				this.GetComponent<BoxCollider2D>().isTrigger = false;
			}
		}
	}

	public void setStationaryPoint(GameObject panickedNPC)
	{
		panickedNPCPosition = panickedNPC.transform.position;
		calledToPoint = true;
		nextPath = new GameObject ();
		nextPath.transform.position = panickedNPCPosition;
	}

	protected override void alert()
	{
		base.alert ();
		//setAnimatorInteger (axeManWalkingKey, (int)AxeManWalkingDirection.STILL);
	}
	
	new protected Vector3 getNextPath()
	{

		return panickedNPCPosition;
	}

	override protected void panic()
	{
		base.panic ();
		this.GetComponent<BoxCollider2D>().isTrigger = false;
	}

	void OnCollisionEnter2D(Collision2D collision)
	{
		/*
		if (!killSelf && !panicked && collision.collider.Equals (panickedNPCPosition.collider2D)) {
			this.GetComponent<BoxCollider2D> ().isTrigger = true;
		}
		*/
	}

	override protected void OnTriggerEnter2D(Collider2D other)
	{
		base.OnTriggerEnter2D (other);
	}

	override protected void OnTriggerExit2D(Collider2D other)
	{
		base.OnTriggerEnter2D (other);
		
	}

	override protected void OnTriggerStay2D(Collider2D other)
	{
		base.OnTriggerStay2D (other);
	}
}
