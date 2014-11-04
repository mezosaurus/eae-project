using UnityEngine;
using System.Collections;

/**
 * Obstruction Minion Script
 **/
public class ObstructionMinionScript : MinionAbilityClass
{
	public float growthTime;
	public float delayTime;
	public Sprite tree1;
	public Sprite tree2;

	float tmpTime;
	bool delayOver;

	// Use this for initialization
	new void Start () {
		base.Start ();

		prefabPath = "Prefabs/ObstructionMinion";

		tmpTime = 0;
		delayOver = false;


		lifetime = -1;


		// Randomize what tree grows
		SpriteRenderer sr = this.gameObject.GetComponent<SpriteRenderer> ();
		float num = Random.Range (0, 2);
		if( num < 1 )
			sr.sprite = tree1;
		else
			sr.sprite = tree2;
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
			transform.position += new Vector3(0, scalingModifier * .5f);
			transform.localScale += new Vector3(.007f, scalingModifier);
		}
	}

}
