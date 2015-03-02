using UnityEngine;
using System.Collections;

public class TutorialScareNPCController : AIController {

	Vector3 sittingPoint;

	new public void Start()
	{
		base.Start ();
		AIGenerator.loadNPCWithSkin (gameObject, "bopper_skin", NPCSkinType.Bopper);

		sittingPoint = transform.position;
		
		nextPath = new GameObject();
		nextPath.transform.position = sittingPoint;
		nextPath.transform.SetParent (transform);

	}

	protected override void GameUpdate () 
	{
		if (updateNPC())
		{
			return;
		}

		Vector3 pathPosition = sittingPoint;
		Vector3 positionNPC = transform.position;
		float step = speed * Time.deltaTime;
		
		Vector3 movement = Vector3.MoveTowards (positionNPC, pathPosition, step);
		Vector3 direction = Vector3.Normalize(movement - transform.position);
		Vector3 biasPosition = new Vector3 (transform.position.x - movement.x, transform.position.y - movement.y);
		
		if (Mathf.Abs (biasPosition.x) < 0.1 && Mathf.Abs (biasPosition.y) < 0.001)
		{
			//To the right
			setAnimatorInteger(walkingKey, (int)WalkingDirection.STILL);
		}
		else
		{
			setAnimatorInteger(walkingKey, (int)WalkingDirection.MOVING_DOWN);		
		}
		
		transform.position = movement;
	}

	protected override GameObject getNextPath ()
	{
		GameObject path = new GameObject ();
		path.transform.position = sittingPoint;
		return path;
	}

	protected override void lure (Vector3 lurePosition)
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
