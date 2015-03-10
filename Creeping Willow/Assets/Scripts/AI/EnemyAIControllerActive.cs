using UnityEngine;	
using System.Collections;

public class EnemyAIControllerActive : EnemyAIController 
{
	/*
	public float investigateTime = 60f;
	public float sittingTime = 2.0f;
	public float wanderRadius = 2f;
	protected Vector3 panickedNPCPosition;
	protected bool investigating = false;
	protected bool angry = false;
	protected float leaveTime;
	protected bool calledToPoint = false;
	protected bool treePath = false;
	protected bool investigatePath = false;
	protected bool checkingPlayer = false;
	protected ArrayList treeList;

	protected float nextInvestigateTime = 0;
	//*/

	protected GameObject lastPathPosition;
	protected string deleteTag = "Immovable";
	protected bool sitting = false;

	public override void Start ()
	{
		base.Start ();
		lastPathPosition = createTempGameObject (nextPath.transform.position, transform);
	}

	protected override void GameUpdate () 
	{
		//Debug.Log ("NextPath: " + nextPath + "\t" + nextPath.transform.position);
		if (updateEnemyNPC ())
		{
			return;
		}

		if (investigating)
		{
			if (leaveTime <= Time.time)
			{
				investigating = false;
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
		
		Vector3 changeMovement = avoid (direction);
		
		if( changeMovement != Vector3.zero )
		{
			Vector3 newPos = Vector3.MoveTowards(positionNPC,changeMovement,step);
			determineDirectionChange(transform.position, newPos);
			transform.position = newPos;
		}
		else
		{
			determineDirectionChange(transform.position, movement);
			transform.position = movement;
		}
		
		if (movement == pathPosition)
		{
			if (!sitting) {
				handleSitting();
			}
		}
		else
		{
			sitting = false;
		}
	}
	
	protected override void handleSitting()
	{
		if (!sitting)
		{
			sitting = true;
			int rand = Random.Range(0, 4);

			if (nextPath.tag.Equals(deleteTag))
			{
				Destroy(nextPath);
			}
			else
			{
				lastPathPosition.transform.position = nextPath.transform.position;
			}

			if (rand == 0)
			{
				nextPath = createTempGameObject(getWanderPoint(lastPathPosition.transform.position), transform);
				nextPath.tag = deleteTag;
			}
			else
			{
				nextPath = movePath.getNextPath (lastPathPosition, gameObject);
			}
		}
		/*
		if (treePath) 
		{
			nextInvestigateTime = Time.time + sittingTime;
			investigating = true;
			treePath = false;
			
			//var playerPosition = player.transform.position;
			if (checkingPlayer)
				//&& 
				//(Vector3.Distance(playerPosition, panickedNPCPosition) > wanderRadius || 
				//!nextPath.Equals(new Vector3(playerPosition.x, playerPosition.y - transform.renderer.bounds.size.y/5))))
			{
				investigating = false;
				nextInvestigateTime = Time.time + 0.33f;
			}
			setAnimatorInteger(walkingKey, (int)WalkingDirection.STILL_ACTION);
		}
		
		if (checkingPlayer && nextInvestigateTime <= Time.time)
		{
			MessageCenter.Instance.Broadcast(new EnemyNPCInvestigatingPlayerMessage(gameObject));
		}
		
		if (investigatePath)
		{
			nextInvestigateTime = Time.time + sittingTime;
			investigating = true;
			investigatePath = false;
			setAnimatorInteger(walkingKey, (int)WalkingDirection.STILL);
		}
		
		if (nextPath.transform.position.Equals(panickedNPCPosition) || killSelf)
		{
			if (killSelf && !nextPath.transform.position.Equals(panickedNPCPosition))
				destroyNPC();
			
			if (!investigating)
			{
				investigating = true;
				leaveTime = Time.time + investigateTime;
			}			
		}
		//*/
	}

	// Shouldn't need to overwrite

	/*
	protected override void investigate()
	{
		if (nextInvestigateTime <= Time.time)
		{
			investigating = false;
			if (Random.value > 0.5)
			{
				GameObject tree = null;
				int rand = 0;
				
				while(tree == null)
				{
					if (treeList.Count == 0)
						break;
					
					rand = Random.Range(0, treeList.Count);
					tree = (GameObject)treeList[rand];
					if (tree.Equals(player) && Vector3.Distance(player.transform.position, panickedNPCPosition) > wanderRadius)
					{
						tree = null;
						treeList.RemoveAt(rand);
					}
				}
				
				if (tree != null)
				{
					Vector3 nextTreePosition = tree.transform.position;
					treeList.RemoveAt(rand);	// Remove the tree so it won't be chopped again
					nextPath.transform.position = new Vector3(nextTreePosition.x, nextTreePosition.y - transform.renderer.bounds.size.y/5);
					treePath = true;
					investigatePath = false;
					checkingPlayer = tree.Equals(player);
					Debug.Log (checkingPlayer);
					
					return;
				}
			}
			
			//*
			treePath = false;
			investigatePath = true;
			Vector2 position = Random.insideUnitCircle * wanderRadius;
			nextPath.transform.position = new Vector3(position.x, position.y, 0.0f) + panickedNPCPosition;
		}
	}
	//*/

	public void setMovingPath(SubpathScript movePath)
	{
		this.movePath = movePath;
		nextPath = movePath.getNextPath (null, gameObject);
		initAxeMan ();
	}

	override protected GameObject getNextPath()
	{
		return movePath.getNextPath (lastPathPosition, gameObject);
	}
}
