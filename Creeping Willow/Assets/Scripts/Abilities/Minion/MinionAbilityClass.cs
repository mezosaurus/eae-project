using UnityEngine;
using System.Collections;

/**
 * Inherits from the Ability Class.
 * Is a template for the Minion Ability Class.
 **/
public abstract class MinionAbilityClass : AbilityClass {

	// protected properties

	// private properties
	private float tmpLife;

	// Use this for initialization
	new void Start () {
		base.Start ();
		
		type = AbilityType.Minion;
	}
	
	// Update is called once per frame
	protected override void GameUpdate () {
		base.GameUpdate ();
	}
}
