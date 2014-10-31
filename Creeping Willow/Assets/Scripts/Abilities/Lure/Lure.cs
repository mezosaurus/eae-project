using UnityEngine;
using System.Collections;

public class Lure : AbilityClass {

	public int lurePower;

	void OnTriggerEnter2D(Collider2D collider){
		if(collider.GetType() == typeof(BoxCollider2D)){
			LureEnteredMessage message = new LureEnteredMessage (this, collider.gameObject);
			MessageCenter.Instance.Broadcast (message);
		}
	}

	new void Start(){
		base.Start ();
		type = AbilityType.Lure;
		prefabPath = "Prefabs/CandyLure";
	}
}
