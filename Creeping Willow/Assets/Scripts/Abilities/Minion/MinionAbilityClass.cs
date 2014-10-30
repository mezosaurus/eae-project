using UnityEngine;
using System.Collections;

/**
 * Inherits from the Ability Class.
 * Is a template for the Minion Ability Class.
 **/
public abstract class MinionAbilityClass : AbilityClass {

	// protected properties
	protected int size;

	// private properties
	private float tmpLife;

	// Use this for initialization
	new void Start () {
		base.Start ();
		
		type = AbilityType.Minion;
		tmpLife = lifetime;
	}
	
	// Update is called once per frame
	new void Update () {
		base.Update ();

		if( lifetime < 0 ) {} // Do nothing
		else if( tmpLife > 0 ) tmpLife -= timeModifier;
		else Destroy( this.gameObject );
	}

	/**
	 * Return the amount of minions produced when using this ability
	 **/
	public int getSize()
	{
		return size;
	}
}
