using UnityEngine;
using System.Collections;

/**
 * Tire Swing Script
 **/
public class TireRangedScript : RangedAbilityClass
{
	float speed;
	float distanceCovered;
	bool reached;
	ArrayList npcCaught;

	// Use this for initialization
	new void Start () {
		base.Start ();

		distance = GAM.tireSwingDistance;
		speed = GAM.tireSwingSpeed;
		count = GAM.tireSwingCount;
		coolDown = GAM.tireSwingCooldown;

		distanceCovered = 0;
		reached = false;
		npcCaught = new ArrayList();

		// if player
		if( transform.name == "Player" )
		{
			isAbility = false;
		}
		// if tire swing
		else 
		{
			isAbility = true;
		}
	}
	
	// Update is called once per frame
	new void Update () {
		base.Update ();

		if( isAbility )
		{
			// if tire swing hasn't reached full length
			if( distanceCovered < distance && !reached)
			{
				distanceCovered += speed;
				transform.position += direction;
			}
			// at full length
			else if( distanceCovered >= distance && !reached)
			{
				reached = true;
			}
			// coming back to player
			else if( distanceCovered > 0 && reached)
			{
				distanceCovered -= speed;
				transform.position -= direction;
			}
			// tire swing end
			else if( distanceCovered <= 0 && reached  )
			{
				foreach( GameObject npc in npcCaught )
				{
					npc.GetComponent<CircleCollider2D>().enabled = true;
					// add in player having npcs
				}

				// broadcast ability no longer in use
				AbilityStatusChangedMessage message = new AbilityStatusChangedMessage(false);
				MessageCenter.Instance.Broadcast(message);

				// destroy tire swing
				Destroy ( this.gameObject );
			}
		}
		// if player activates ability and they are able to use the ability
		else if( Input.GetKeyDown(KeyCode.F) && count > 0 && coolDownInProgress == false && !abilityUsed )
		{
			count--;
			float diagMove = Mathf.Sqrt (2) * .5f;

			// update direction vector
			switch(PM.getDirection())
			{
				case (int)TmpPlayerManager.DirectionState.UP: direction = new Vector3(0,1) * speed; break;
				case (int)TmpPlayerManager.DirectionState.TOP_RIGHT: direction = new Vector3(diagMove,diagMove) * speed; break;
				case (int)TmpPlayerManager.DirectionState.RIGHT: direction = new Vector3(1,0) * speed; break;
				case (int)TmpPlayerManager.DirectionState.BOTTOM_RIGHT: direction = new Vector3(diagMove,-diagMove) * speed; break;
				case (int)TmpPlayerManager.DirectionState.DOWN: direction = new Vector3(0,-1) * speed; break;
				case (int)TmpPlayerManager.DirectionState.BOTTOM_LEFT: direction = new Vector3(-diagMove,-diagMove) * speed; break;
				case (int)TmpPlayerManager.DirectionState.LEFT: direction = new Vector3(-1,0) * speed; break;
				case (int)TmpPlayerManager.DirectionState.TOP_LEFT: direction = new Vector3(-diagMove,diagMove) * speed; break;
			}

			// create an instance
			GameObject go = (GameObject)Instantiate( Resources.Load("Prefabs/TireSwingRanged"), transform.position + Vector3.Scale(direction / speed, collider2D.bounds.extents * 1.5f), Quaternion.identity);
			go.name = "TireSwing";
			coolDownInProgress = true;
			go.GetComponent<TireRangedScript>().direction = direction;

			// broadcast a ability in use
			AbilityStatusChangedMessage message = new AbilityStatusChangedMessage(true);
			MessageCenter.Instance.Broadcast(message);
		}

		if( npcCaught.Count > 0 )
		{
			// have NPC be caught in the tire swing
			foreach( GameObject npc in npcCaught )
			{
				npc.transform.position = transform.position;
			}
		}
	}
	
	void OnCollisionEnter2D(Collision2D other)
	{
		// if the tire hits an immovable object
		if( other.gameObject.tag == "Immovable" && isAbility )
		{
			reached = true;
		}

		// if the tire swing hits an NPC
		if( other.gameObject.tag == "Enemy" && isAbility )
		{
			npcCaught.Add(other.gameObject);
			other.gameObject.GetComponent<CircleCollider2D>().enabled = false;
		}
	}
}
