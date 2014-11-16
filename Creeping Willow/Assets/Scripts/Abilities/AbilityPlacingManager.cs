using UnityEngine;
using System.Collections;
using System.Collections.Generic;

public class AbilityPlacingManager : GameBehavior {

	Dictionary<AbilityType, string> prefabPaths;
	// Use this for initialization
	void Start () {
		prefabPaths = new Dictionary<AbilityType, string> ();
		prefabPaths.Add (AbilityType.Lure, "Prefabs/Abilities/CandyLure");
		MessageCenter.Instance.RegisterListener (MessageType.AbilityPlaced, HandleAbilityPlaced);
	}
	
	// Update is called once per frame
	protected override void GameUpdate () {
	
	}

	void HandleAbilityPlaced(Message message){
		AbilityPlacedMessage apmess = message as AbilityPlacedMessage;
		string path = "";
		if(prefabPaths.TryGetValue (apmess.AType, out path)){
			GameObject.Instantiate (Resources.Load (path), new Vector3 (apmess.X, apmess.Y), Quaternion.identity);
		}
	}
}
