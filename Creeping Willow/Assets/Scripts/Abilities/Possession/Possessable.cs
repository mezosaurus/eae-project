using UnityEngine;
using System.Collections;

public abstract class Possessable : GameBehavior {
	bool possessed;
	protected float baseX;
	protected float baseY;

	void Start(){
		baseX = transform.position.x;
		baseY = transform.position.y;
		possessed = false;
	}
	// Update is called once per frame
	protected override abstract void GameUpdate ();
	protected abstract void act();

	public void useAbility(){
		if (possessed) {
			act();
		}
	}

	public virtual void possess(){
		possessed = true;
	}

	public virtual void exorcise(){
		possessed = false;
	}

}
