using UnityEngine;
using System.Collections;

public abstract class Possessable : GameBehavior {
	bool possessed;

	void Start(){
		possessed = false;
	}
	// Update is called once per frame
	protected override abstract void GameUpdate ();
	protected abstract void act();

	protected void useAbility(){
		if (possessed) {
			act();
		}
	}

	protected void possess(){
		possessed = true;
	}

	protected void exorcise(){
		possessed = false;
	}

}
