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
		audio.PlayOneShot (popSound);
		string popLocation = "Particles/Particle System_critterpop";
		GameObject pop = (GameObject)Instantiate (Resources.Load (popLocation), Vector3.zero, Quaternion.identity);
		pop.transform.SetParent (transform, false);
		pop.GetComponent<ParticleSystem> ().Play ();
		
		// Destroy objects
		float duration = pop.GetComponent<ParticleSystem> ().duration;
		Destroy (pop, duration);
		Destroy (gameObject, duration);

		Vector3 position = gameObject.transform.position;
		AbilityPlacedMessage message = new AbilityPlacedMessage (position.x, position.y, AbilityType.PossessionScare);
		MessageCenter.Instance.Broadcast (message);
	}
}	
