using UnityEngine;
using System.Collections;

public class TutorialLureNPCController : AIController {

	Vector3 sittingPoint;

	new public void Start()
	{
		base.Start ();
		AIGenerator.loadNPCWithSkin (gameObject, "hippie_skin", NPCSkinType.Hippie);
		nextPath = new GameObject();
		nextPath.transform.position = transform.position;
		nextPath.transform.SetParent (transform);

		sittingPoint = new Vector3(nextPath.transform.position.x, nextPath.transform.position.y);
	}

	protected override void GameUpdate () 
	{
		if (updateNPC())
		{
			return;
		}

		Vector3 pathPosition = nextPath.transform.position;
		Vector3 positionNPC = transform.position;
		float step = speed * Time.deltaTime;
		
		Vector3 movement = Vector3.MoveTowards (positionNPC, pathPosition, step);
		Vector3 direction = Vector3.Normalize(movement - transform.position);
		Vector3 biasPosition = new Vector3 (transform.position.x - movement.x, transform.position.y - movement.y);

		if (Mathf.Abs (biasPosition.x) < 0.1 && Mathf.Abs (biasPosition.y) < 0.001)
		{
			//To the right
			setAnimatorInteger(walkingKey, (int)WalkingDirection.STILL_DOWN_LEFT);
		}
		else
		{
			setAnimatorInteger(walkingKey, (int)WalkingDirection.MOVING_DOWN_LEFT);		
		}

		transform.position = movement;
	}

	protected override GameObject getNextPath ()
	{
		GameObject path = new GameObject ();
		path.transform.position = sittingPoint;
		return path;
	}

	protected override void scare (Vector3 scaredPosition)
	{
		// Do nothing
	}

	protected override void alert ()
	{
		// Do nothing
	}
	
	protected override void panic()
	{
		// Do nothing
	}
}
