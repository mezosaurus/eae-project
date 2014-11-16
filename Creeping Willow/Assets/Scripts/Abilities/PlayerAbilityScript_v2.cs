using UnityEngine;
using System.Collections;

public class PlayerAbilityScript_v2 : GameBehavior {

	public int luresAllowed;
	private int luresLeft;
	private bool abilityInUse;

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
		if ((Input.GetKeyDown(KeyCode.D) || Input.GetButtonDown("X")))
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
