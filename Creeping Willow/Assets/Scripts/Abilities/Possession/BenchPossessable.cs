using UnityEngine;
using System.Collections;
using System.Collections.Generic;
//using XInputDotNetPure;

public class BenchPossessable : PossessableItem {

	protected override void Start(){
		base.Start ();
		hintOffset = new Vector2(.25f, .85f);
	}


	// Update is called once per frame
	protected override void GameUpdate () {
		base.GameUpdate ();
	}

	protected override void lure(){
		base.lure ();
		blinking = true;
		AbilityPlacedMessage message = new AbilityPlacedMessage (transform.position.x,transform.position.y, AbilityType.PossessionLure);
		MessageCenter.Instance.Broadcast (message);
	}

	protected override void scare ()
	{
		base.scare ();
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
