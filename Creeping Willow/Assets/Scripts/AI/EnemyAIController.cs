using UnityEngine;	
using System.Collections;

public class EnemyAIController : AIController 
{
	public float sittingTime;
	protected Vector3 panickedNPCPosition;
	private bool sitting = false;
	private bool angry = false;
	private float leaveTime;
	private bool calledToPoint = false;

	private float nextInvestigateTime = 0;

	private static string axeManWalkingKey = "direction";

	private enum AxeManWalkingDirection
	{
		STILL = 0,
		WALK = 1,
	}

	new public void Start()
	{
		base.Start ();
		xScale = -xScale;
	}

	protected override void GameUpdate () 
	{
		if (grabbed)
			return;

		if (angry)
		{
			chasePlayer();
			return;
		}

		if (updateNPC ())
			return;	

		// if lure is deleted
		if( nextPath == null ) return;

		if (nextPath.Equals(Vector3.zero))
			Debug.Log ("har");

		if (sitting)
		{
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
			}
			
			investigate();
		}

		Vector3 pathPosition = nextPath.transform.position;
		Vector3 positionNPC = transform.position;
		float step = speed * Time.deltaTime;

		Vector3 movement = Vector3.MoveTowards (positionNPC, pathPosition, step);

		animateCharacter(movement, pathPosition);
		
		transform.position = movement;

		if (movement == pathPosition && (nextPath.transform.position.Equals(panickedNPCPosition) || killSelf))
		{
			if (killSelf && !nextPath.transform.position.Equals(panickedNPCPosition))
				destroyNPC();
			
			if (!sitting)
			{
				sitting = true;
				leaveTime = Time.time + sittingTime;
			}			
		}
	}
	
	private void animateCharacter(Vector3 movement, Vector3 moveTo)
	{
		determineDirectionChange (transform.position, movement);
		Vector3 biasPosition = new Vector3 (transform.position.x - movement.x, transform.position.y - movement.y);
		//if (biasPosition.x == 0 && biasPosition.y == 0)
		if (movement == moveTo && biasPosition.x == 0 && biasPosition.y == 0)
		{
			//no movement
			//flipXScale(!lastDirectionWasRight);
			setAnimatorInteger(axeManWalkingKey, (int)AxeManWalkingDirection.STILL);
		}
		else
			setAnimatorInteger(axeManWalkingKey, (int)AxeManWalkingDirection.WALK);
	}

	private void chasePlayer()
	{
		rigidbody2D.velocity = Vector2.zero;
		Vector3 pathPosition = player.transform.position;
		Vector3 positionNPC = transform.position;
		float step = speed * Time.deltaTime;
		
		Vector3 movement = Vector3.MoveTowards (positionNPC, pathPosition, step);
		
		animateCharacter(movement, pathPosition);
		
		transform.position = movement;
	}

	private void investigate()
	{
		if (nextInvestigateTime <= Time.time)
		{
			nextInvestigateTime = Time.time + sittingTime/4;

			if (Random.value > 0.5)
			{
				GameObject[] trees = GameObject.FindGameObjectsWithTag("Tree");
			}
			else
			{
				Vector2 position = Random.insideUnitCircle * 2;
				nextPath.transform.position = new Vector3(position.x, position.y, 0.0f) + panickedNPCPosition;
			}
		}
	}

	public void setStationaryPoint(GameObject panickedNPC)
	{
		panickedNPCPosition = panickedNPC.transform.position;
		panickedNPCPosition = new Vector3 (panickedNPCPosition.x, panickedNPCPosition.y);
		calledToPoint = true;
		nextPath = new GameObject ();
		nextPath.transform.position = panickedNPCPosition;
	}

	protected override void alert()
	{
		if (!angry)
		{
			base.alert ();
			setAnimatorInteger (axeManWalkingKey, (int)AxeManWalkingDirection.STILL);
		}
	}
	
	override protected GameObject getNextPath()
	{
		nextPath = new GameObject ();
		nextPath.transform.position = panickedNPCPosition;
		return nextPath;
	}

	override protected void panic()
	{
		if (!angry)
		{
			base.panic ();
			panicked = false;

			angry = true;
		}
	}

	override protected void decrementAlertLevel()
	{
		if (!angry)
			base.decrementAlertLevel();
	}

	void OnCollisionEnter2D(Collision2D collision)
	{
		if (angry && collision.gameObject.tag.Equals(player.tag))
		{
			// TODO: It's axe time
			PlayerKilledMessage message = new PlayerKilledMessage(gameObject);
			MessageCenter.Instance.Broadcast(message);
		}
	}
}
