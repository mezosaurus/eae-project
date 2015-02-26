using UnityEngine;	
using System.Collections;

public class EnemyAIController : AIController 
{
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

	// Hit sound vars
	public AudioClip hitSound1;
	public AudioClip hitSound2;
	public AudioClip hitSound3;
	protected int hitCounter = 1;

	new public void Start()
	{
		base.Start ();
		xScale = -xScale;
	}

	protected void initAxeMan()
	{
		SpriteRenderer[] sceneObjects = FindObjectsOfType<SpriteRenderer>();
		treeList = new ArrayList ();
		if (player == null)
			player = GameObject.Find("Player");
		
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

	protected override void NPCOnDestroy()
	{
		MessageCenter.Instance.Broadcast (new EnemyNPCDestroyedMessage (gameObject));
	}

	protected virtual bool updateEnemyNPC()
	{
		if (grabbed)
			return true;
		
		if (angry)
		{
			player = getPlayer();
			chasePlayer();
			return true;
		}
		
		return (updateNPC () || nextPath == null);
	}

	protected virtual void handleSitting()
	{
		/*
		if (treePath) 
		{
			nextInvestigateTime = Time.time + sittingTime;
			investigating = true;
			treePath = false;

			if (checkingPlayer)
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

	protected void animateCharacter(Vector3 movement, Vector3 moveTo)
	{
		determineDirectionChange (transform.position, movement);
		Vector3 biasPosition = new Vector3 (transform.position.x - movement.x, transform.position.y - movement.y);
		//if (biasPosition.x == 0 && biasPosition.y == 0)
		if (movement != moveTo || biasPosition.x != 0 || biasPosition.y != 0)
		{
			setAnimatorInteger(walkingKey, (int)WalkingDirection.MOVING_DOWN);
		}
		/*
		else
		{
			//no movement
			flipXScale(!lastDirectionWasRight);
			setAnimatorInteger(axeManWalkingKey, (int)AxeManWalkingDirection.STILL);
		}
		//*/
	}

	protected void chasePlayer()
	{
		rigidbody2D.velocity = Vector2.zero;
		Vector3 pathPosition = player.transform.position;
		Vector3 positionNPC = transform.position;
		float step = speed * Time.deltaTime;
		
		Vector3 movement = Vector3.MoveTowards (positionNPC, pathPosition, step);
		
		animateCharacter(movement, pathPosition);
		
		Vector3 changeMovement = avoid (Vector3.Normalize(movement - transform.position));
		
		if( changeMovement != Vector3.zero )
		{
			Vector3 newPos = Vector3.MoveTowards(positionNPC,changeMovement,step);
			transform.position = newPos;
			determineDirectionChange(transform.position, newPos);
		}
		else
		{
			transform.position = movement;
			determineDirectionChange(transform.position, movement);
		}
	}

	protected virtual void investigate()
	{
		/*
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
		//*/
	}

	protected override bool NPCHandleSeeingPlayer()
	{
		panic ();
		return true;
	}

	// NPC State Methods

	protected override void panic()
	{
		if (!angry)
		{
			base.panic ();
			panicked = false;
			
			angry = true;
		}
	}
	
	protected override void alert()
	{
		if (!angry)
		{
			base.alert ();
			setAnimatorInteger (walkingKey, (int)WalkingDirection.STILL);
		}
	}

	protected override void decrementAlertLevel()
	{
		if (!angry)
			base.decrementAlertLevel();
	}

	// Collision Methods

	protected override void OnCollisionEnter2D(Collision2D collision)
	{
		base.OnCollisionEnter2D (collision);

		if (angry && collision.gameObject.tag.Equals(player.tag))
		{
			PlayerKilledMessage message = new PlayerKilledMessage(gameObject);
			MessageCenter.Instance.Broadcast(message);
		}
	}
}
