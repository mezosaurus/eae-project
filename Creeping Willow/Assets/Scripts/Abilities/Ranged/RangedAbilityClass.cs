using UnityEngine;
using System.Collections;

/**
 * Template for all ranged abilities.
 **/
public abstract class RangedAbilityClass : AbilityClass {

	// protected properties
	protected float distance;
	protected Vector3 direction;

	// Use this for initialization
	new void Start () {
		base.Start ();
		type = AbilityType.Ranged;
	}
	
	// Update is called once per frame
	new void Update () {
		base.Update ();
	}
}
