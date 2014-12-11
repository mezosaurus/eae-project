using UnityEngine;
using System.Collections;

public class PlayerAbilityScript_v2 : GameBehavior {

	public int luresAllowed;
	private int luresLeft;
	public bool abilityInUse;

	// Use this for initialization
	void Start () {
		luresLeft = luresAllowed;
		MessageCenter.Instance.RegisterListener (MessageType.AbilityObjectPlaced, HandleObjectPlaced);
		MessageCenter.Instance.RegisterListener (MessageType.AbilityObjectRemoved, HandleObjectRemoved);
	}
	
	// Update is called once per frame
	protected override void GameUpdate () {
		if (paused){
			return;
		}
		HandleInput ();
	}

	void HandleInput(){
		/*if (((Input.GetKeyDown(KeyCode.D) || Input.GetButtonDown("X"))) && GameObject.FindGameObjectWithTag("Player").GetComponent<TreeController>().state != Tree.State.Eating)
		{
			if(luresLeft > 0 && !abilityInUse){
				AbilityStatusChangedMessage message = new AbilityStatusChangedMessage(true);
				MessageCenter.Instance.Broadcast(message);
				GameObject obj = (GameObject)Resources.Load("Prefabs/Abilities/LurePlacer");
				LurePlacer lp = obj.GetComponent<LurePlacer>();
				lp.luresAllowed = luresLeft;
				GameObject.Instantiate(lp, transform.position, Quaternion.identity);
				abilityInUse = true;
			}
		}*/
		if (Input.GetButtonDown ("A") && !abilityInUse && (GameObject.FindGameObjectWithTag ("Player").GetComponent<TreeController> ().state != Tree.State.Eating && GameObject.FindGameObjectWithTag ("Player").GetComponent<TreeController> ().state != Tree.State.AxeManMinigame)) {
			AbilityStatusChangedMessage message = new AbilityStatusChangedMessage(true);
			MessageCenter.Instance.Broadcast(message);
			Instantiate(Resources.Load("Particles/soulsmoke"), transform.position, Quaternion.identity);
			abilityInUse = true;
		}
	}
	
	void HandleObjectPlaced(Message message){
		AbilityObjectPlacedMessage placed = message as AbilityObjectPlacedMessage;
		switch(placed.Atype){
		case AbilityType.Lure:
			luresLeft -= placed.NumPlaced;
			break;
		}
		abilityInUse = false;
	}

	void HandleObjectRemoved(Message message){
		AbilityObjectRemovedMessage removed = message as AbilityObjectRemovedMessage;
		switch(removed.Atype){
		case AbilityType.Lure:
			luresLeft++;
			break;
		}
	}
}
