using UnityEngine;
using System.Collections;

public class AbilityPlacingManager : GameBehavior {

	// Use this for initialization
	void Start () {
		MessageCenter.Instance.RegisterListener (MessageType.AbilityPlaced, HandleAbilityPlaced);
	}
	
	// Update is called once per frame
	protected override void GameUpdate () {
	
	}

	void HandleAbilityPlaced(Message message){
		AbilityPlacedMessage apmess = message as AbilityPlacedMessage;
		GameObject.Instantiate (Resources.Load ("Prefabs/Abilities/CandyLure"), new Vector3 (apmess.X, apmess.Y), Quaternion.identity);
	}
}
