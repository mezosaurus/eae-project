using UnityEngine;
using System.Collections;

public class ConeOfVisionTestScript : GameBehavior {

	public float distance; // distance that player's view extends to
	public float angleSize; // The total angle size that the player can see

	float angleOffset; // max offset of angle from NPC's view
	bool aware;
	Vector3 playerPos;
	Vector3 forward; // Defined where NPC is looking.
	GameObject player;


	// Use this for initialization
	void Start () {
		angleOffset = .5f * angleSize;
		aware = false;
		player = GameObject.Find("Player");
		playerPos = player.transform.position;

		// TODO: Make this vector match NPC's view
		forward = new Vector3 (-1, 0);
	}
	
	// Update is called once per frame
	protected override void GameUpdate () {
		aware = checkForPlayer();

		if( aware )
			Debug.Log("I see you");
	}

	/*
	 * Returns true if player is in view of NPC
	 */
	bool checkForPlayer()
	{
		playerPos = player.transform.position;

		// check if NPC can see that far
		if( Vector3.Distance(transform.position, playerPos) <= distance )
		{
			// get direction of player from NPC's point of view
			Vector3 direction = playerPos - transform.position;

			// TODO: Update forward vector by NPC's direction or what not
			
			// get angle of direction 
			float angle = Vector3.Angle(direction, forward);
			
			if( angle <= angleOffset )
				return true;
		}
		return false;
	}
}
