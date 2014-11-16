using UnityEngine;
using System.Collections;

public class LurePlacer : ObjectPlacer {
	public int luresAllowed;
	private int luresPlaced;
	// Use this for initialization
	protected override void Start () {
		base.Start();
		type = AbilityType.Lure;
		luresPlaced = 0;

	}

	protected override void HandleInput(){
		base.HandleInput();
		if(Input.GetButtonDown("A")&& canPlace && luresPlaced < luresAllowed){
			AbilityPlacedMessage message = new AbilityPlacedMessage (transform.position.x,transform.position.y, type);
			MessageCenter.Instance.Broadcast (message);
			luresPlaced++;
			if(luresPlaced >= luresAllowed){
				Exit();
			}
		}
	}

	protected override void Exit(){
		ExitPlacing (luresPlaced, AbilityType.Lure);
	}
}
