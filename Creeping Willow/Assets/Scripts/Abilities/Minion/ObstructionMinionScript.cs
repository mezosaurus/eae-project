using UnityEngine;
using System.Collections;

/**
 * Obstruction Minion Script
 **/
public class ObstructionMinionScript : MinionAbilityClass
{
	// private properties
	public float growthTime;
	public float delayTime;
	float tmpTime;
	bool delayOver;

	// Use this for initialization
	new void Start () {
		base.Start ();

		//growthTime = GAM.obstructionGrowthTime;
		//delayTime = GAM.obstructionDelayTime;
		prefabPath = "Prefabs/ObstructionMinion";

		tmpTime = 0;
		delayOver = false;


		lifetime = -1;

	}
	
	// Update is called once per frame
	protected override void GameUpdate () {
		base.GameUpdate ();

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

}
