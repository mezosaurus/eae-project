using UnityEngine;
using System.Collections;

/**
 * Tire Swing Script
 **/
public class TireRangedScript : RangedAbilityClass
{
	float distanceCovered;
	bool reached;
	ArrayList npcCaught;

	// Use this for initialization
	new void Start () {
		base.Start ();

		prefabPath = "Prefabs/TireSwingRanged";

		distanceCovered = 0;
		reached = false;
		npcCaught = new ArrayList();

		// broadcast this is in use
		AbilityStatusChangedMessage message = new AbilityStatusChangedMessage(true);
		MessageCenter.Instance.Broadcast(message);
	}
	
	// Update is called once per frame
	protected override void GameUpdate () {
		// if tire swing hasn't reached full length
		if( distanceCovered < distance && !reached)
		{
			distanceCovered += speed;
			transform.position += direction*speed;
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
			transform.position -= direction*speed;
		}
		// tire swing end
		else if( distanceCovered <= 0 && reached  )
		{
			foreach( GameObject npc in npcCaught )
			{
				npc.GetComponent<CircleCollider2D>().enabled = true;
				npc.GetComponent<SpriteRenderer>().enabled = true;
				npc.GetComponent<BoxCollider2D>().enabled = true;
				// add in player having npcs

			}

			// broadcast ability no longer in use
			AbilityStatusChangedMessage message = new AbilityStatusChangedMessage(false);
			MessageCenter.Instance.Broadcast(message);

			// destroy tire swing
			Destroy ( this.gameObject );
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
		if( other.gameObject.tag == "Immovable")
		{
			reached = true;
		}

		// if the tire swing hits an NPC
		if( other.gameObject.tag == "NPC" )
		{
			npcCaught.Add(other.gameObject);
			other.gameObject.GetComponent<CircleCollider2D>().enabled = false;
			other.gameObject.GetComponent<BoxCollider2D>().enabled = false;
			other.gameObject.GetComponent<SpriteRenderer>().enabled = false;

			// broadcast message
		}
	}
}
