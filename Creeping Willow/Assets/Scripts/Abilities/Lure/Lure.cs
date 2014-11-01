using UnityEngine;
using System.Collections;

public class Lure : AbilityClass {

	public int lurePower;
	public float releaseTime;
	protected bool npcCaught;
	protected float caughtTime;
	protected GameObject caughtNPC;

	void OnTriggerEnter2D(Collider2D collider){
		if(collider.GetType() == typeof(BoxCollider2D)){
			LureEnteredMessage message = new LureEnteredMessage (this, collider.gameObject);
			MessageCenter.Instance.Broadcast (message);
			npcCaught = true;
			caughtTime = Time.time;
			caughtNPC = collider.gameObject;
		}
	}

	new void Start(){
		base.Start ();
		type = AbilityType.Lure;
		prefabPath = "Prefabs/CandyLure";
		npcCaught = false;
	}

	protected override void GameUpdate ()
	{
		base.GameUpdate ();
		if (npcCaught) {
			float currentTime = Time.time - caughtTime;
			if(currentTime >= releaseTime){
				LureReleasedMessage message = new LureReleasedMessage (this, caughtNPC);
				MessageCenter.Instance.Broadcast (message);
				npcCaught = false;
				caughtNPC = null;
			}
		}
	}
}
