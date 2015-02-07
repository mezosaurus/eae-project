using UnityEngine;
using System.Collections;
//using XInputDotNetPure;

public class LampPossessable: PossessableItem {

	// Update is called once per frame
	protected override void GameUpdate () {
		base.GameUpdate ();
	}
	
	protected override void lure(){
		base.lure();
		blinking = true;
		AbilityPlacedMessage message = new AbilityPlacedMessage (transform.position.x,transform.position.y, AbilityType.PossessionLure);
		MessageCenter.Instance.Broadcast (message);
	}

	protected override void scare ()
	{
		base.scare();
		shaking = true;
		AbilityPlacedMessage message = new AbilityPlacedMessage (transform.position.x,transform.position.y, AbilityType.PossessionScare);
		MessageCenter.Instance.Broadcast (message);
        //GamePad.SetVibration(PlayerIndex.One, 1f, 1f);
	}

	public override void possess ()
	{
		base.possess ();
	}
}
