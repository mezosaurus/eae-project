using UnityEngine;
using System.Collections;

public class Trap : AbilityClass {
	
	public float releaseTime;
	protected bool npcCaught;
	protected float caughtTime;
	protected GameObject caughtNPC;

	// Use this for initialization
	new void Start () {
		base.Start ();
		npcCaught = false;
		type = AbilityType.Trap;
		prefabPath = "Prefabs/BushTrap";
	}
	
	void OnTriggerEnter2D(Collider2D collider){
		if(collider.GetType() == typeof(BoxCollider2D)){
			TrapEnteredMessage message = new TrapEnteredMessage (this, collider.gameObject);
			MessageCenter.Instance.Broadcast (message);
			npcCaught = true;
			caughtTime = Time.time;
			caughtNPC = collider.gameObject;
		}
	}
	
	// Update is called once per frame
	protected override void GameUpdate () {
		base.GameUpdate();
		if (npcCaught) {
			float currentTime = Time.time - caughtTime;
			if(currentTime >= releaseTime){
				TrapReleasedMessage message = new TrapReleasedMessage (this, caughtNPC);
				MessageCenter.Instance.Broadcast (message);
				npcCaught = false;
				caughtNPC = null;
			}
		}
	}
}
