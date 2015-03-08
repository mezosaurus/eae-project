using UnityEngine;
using System.Collections;

public class testCritter : CritterController, OnPossessListener {
	protected CritterPossessable critterPossessable;

	public override void Start() 
	{
		base.Start ();
		gameObject.AddComponent<CritterPossessable> ();
		critterPossessable = gameObject.GetComponent<CritterPossessable> ();
		critterPossessable.setOnPossessListener (this);
	}

	public void onPossess()
	{
		// TODO: Play explode animation
		audio.PlayOneShot (popSound);
		destroyNPC ();
	}
}	
