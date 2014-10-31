using UnityEngine;
using System.Collections;

/**
 * Template for all ranged abilities.
 **/
public abstract class RangedAbilityClass : AbilityClass {

	public float distance;
	public float speed;

	// Use this for initialization
	new void Start () {
		base.Start ();
		type = AbilityType.Ranged;
	}
	
	// Update is called once per frame
	protected override void GameUpdate () {
		base.GameUpdate ();
	}
}
