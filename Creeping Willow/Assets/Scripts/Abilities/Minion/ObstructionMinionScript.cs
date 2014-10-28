using UnityEngine;
using System.Collections;

/**
 * Obstruction Minion Script
 **/
public class ObstructionMinionScript : MinionAbilityClass
{
	// private properties
	float growthTime;
	float delayTime;
	float tmpTime;
	bool delayOver;

	// Use this for initialization
	new void Start () {
		base.Start ();

		count = GAM.obstructionCount;
		growthTime = GAM.obstructionGrowthTime;
		delayTime = GAM.obstructionDelayTime;
		coolDown = GAM.obstructionCooldown;
		size = GAM.obstructionSize;

		tmpTime = 0;
		delayOver = false;

		if( transform.name == "Player" )
		{
			isAbility = false;
			lifetime = -1;
		}
		else
		{
			count = 0;
			isAbility = true;
			lifetime = GAM.obstructionLifetime;
		}

	}
	
	// Update is called once per frame
	new void Update () {
		base.Update ();

		if( isAbility )
		{
			// if growth hasn't started yet
			if( tmpTime < delayTime && !delayOver )
			{
				tmpTime += timeModifier;
			}
			// update the variables
			else if( tmpTime >= delayTime && !delayOver )
			{
				delayOver = true;
				tmpTime = 0;
			}
			// while growth is occuring
			else if( tmpTime < growthTime && delayOver )
			{
				float scalingModifier = .01f;

				tmpTime += timeModifier;
				transform.position += new Vector3(0, scalingModifier * .5f, 0);
				transform.localScale += new Vector3(0, scalingModifier, 0);
			}
		}
		// if minion is created
		else if( Input.GetKeyDown(KeyCode.A) && count > 0 && coolDownInProgress == false && !abilityUsed )
		{
			Vector3 direction = new Vector3();
			float diagMove = Mathf.Sqrt (2) * .5f;

			// update direction vector
			switch(PM.getDirection())
			{
			case (int)TmpPlayerManager.DirectionState.UP: direction = Vector3.Scale(new Vector3(0,1.5f),collider2D.bounds.extents); break;
			case (int)TmpPlayerManager.DirectionState.TOP_RIGHT: direction = Vector3.Scale(new Vector3(diagMove,diagMove)*1.5f,collider2D.bounds.extents*1.6f); break;
			case (int)TmpPlayerManager.DirectionState.RIGHT: direction = Vector3.Scale(new Vector3(1.5f,0),collider2D.bounds.extents); break;
			case (int)TmpPlayerManager.DirectionState.BOTTOM_RIGHT: direction = Vector3.Scale(new Vector3(diagMove,-diagMove)*1.5f,collider2D.bounds.extents*1.6f); break;
			case (int)TmpPlayerManager.DirectionState.DOWN: direction = Vector3.Scale(new Vector3(0,-1.5f),collider2D.bounds.extents); break;
			case (int)TmpPlayerManager.DirectionState.BOTTOM_LEFT: direction = Vector3.Scale(new Vector3(-diagMove,-diagMove)*1.5f,collider2D.bounds.extents*1.6f); break;
			case (int)TmpPlayerManager.DirectionState.LEFT: direction = Vector3.Scale(new Vector3(-1.5f,0),collider2D.bounds.extents); break;
			case (int)TmpPlayerManager.DirectionState.TOP_LEFT: direction = Vector3.Scale(new Vector3(-diagMove,diagMove)*1.5f,collider2D.bounds.extents*1.6f); break;
			}

			count--;
			GameObject go = (GameObject)Instantiate( Resources.Load("Prefabs/ObstructionMinion"), transform.position + direction, Quaternion.identity);
			go.name = "ObstructionMinion";
			coolDownInProgress = true;
		}
	}

}
