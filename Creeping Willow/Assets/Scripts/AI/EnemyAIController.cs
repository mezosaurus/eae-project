using UnityEngine;	
using System.Collections;

public class EnemyAIController : AIController 
{
	public float investigateTime = 60f;
	public float sittingTime = 2.0f;
	public float wanderRadius = 5f;
	protected Vector3 panickedNPCPosition;
	private bool sitting = false;
	private bool angry = false;
	private float leaveTime;
	private bool calledToPoint = false;
	private bool treePath = false;
	private bool checkingPlayer = false;
	private ArrayList treeList;

	private float nextInvestigateTime = 0;

	private static string axeManWalkingKey = "direction";

	private enum AxeManWalkingDirection
	{
		STILL = 0,
		WALK = 1,
		CHOPPING = 2,
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
		Vector3 direction = Vector3.Normalize(movement - transform.position);
		animateCharacter(movement, pathPosition);
		
		if( avoid (direction) != Vector3.zero )
			transform.position = Vector3.MoveTowards(positionNPC,avoid (direction),step);
		else
			transform.position = movement;

		if (treePath && movement == pathPosition) 
		{
			if (checkingPlayer)
			{
				MessageCenter.Instance.Broadcast(new EnemyNPCInvestigatingPlayerMessage(gameObject));
			}

			// TODO: animate axe
			setAnimatorInteger(axeManWalkingKey, (int)AxeManWalkingDirection.CHOPPING);
		}

		if (movement == pathPosition && (nextPath.transform.position.Equals(panickedNPCPosition) || killSelf))
		{
			if (killSelf && !nextPath.transform.position.Equals(panickedNPCPosition))
				destroyNPC();
			
			if (!sitting)
			{
				sitting = true;
				leaveTime = Time.time + investigateTime;
			}			
		}
		//objectAvoidance ();
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
			nextInvestigateTime = Time.time + sittingTime;

			if (Random.value > 0.5)
			{
				var rand = Random.Range(0, treeList.Count);
				var tree = (GameObject)treeList[rand];
				if (tree != null)
				{
					Vector3 nextTreePosition = tree.transform.position;
					treeList.RemoveAt(rand);	// Remove the tree so it won't be chopped again
					nextPath.transform.position = new Vector3(nextTreePosition.x, nextTreePosition.y - transform.renderer.bounds.size.y/5);
					treePath = true;

					if (tree.Equals(player))
						checkingPlayer = true;
						
					return;
				}
			}
			treePath = false;
			Vector2 position = Random.insideUnitCircle * wanderRadius;
			nextPath.transform.position = new Vector3(position.x, position.y, 0.0f) + panickedNPCPosition;
		}
	}

	public void setStationaryPoint(GameObject panickedNPC)
	{
		panickedNPCPosition = panickedNPC.transform.position;
		panickedNPCPosition = new Vector3 (panickedNPCPosition.x, panickedNPCPosition.y);
		calledToPoint = true;
		nextPath = new GameObject ();
		nextPath.transform.position = panickedNPCPosition;

		SpriteRenderer[] sceneObjects = FindObjectsOfType<SpriteRenderer>();
		treeList = new ArrayList ();
		treeList.Add (player);
		
		foreach (SpriteRenderer sceneObject in sceneObjects)
		{
			if (sceneObject.sprite == null)
				continue;
			
			string name = sceneObject.sprite.name;
			if (name.Equals("cw_tree_2") || name.Equals("cw_tree_3") || name.Equals ("cw_tree_4") || name.Equals("cw_tree_5") || name.Equals("cw_tree_6"))
			{
				if (Vector3.Distance(panickedNPCPosition, sceneObject.gameObject.transform.position) < wanderRadius) {
					treeList.Add (sceneObject.gameObject);
				}
			}
		}
	}
	
	protected override void alert()
	{
		if (!angry)
		{
			base.alert ();
			setAnimatorInteger (axeManWalkingKey, (int)AxeManWalkingDirection.STILL);
		}
	}

	override protected bool NPCHandleSeeingPlayer()
	{
		panic ();
		return true;
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
