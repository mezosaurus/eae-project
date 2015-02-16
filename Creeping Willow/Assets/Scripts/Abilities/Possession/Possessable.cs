using UnityEngine;
using System.Collections;
using System.Collections.Generic;

public abstract class Possessable : GameBehavior {
	
	public bool Active;
	protected float baseX;
	protected float baseY;
	protected bool acting = false;
	protected bool needToSend = false;
	public AudioClip actionSound;
	protected Dictionary<string, Color> colors = new Dictionary<string, Color>();

	protected virtual void Start(){
		baseX = transform.position.x;
		baseY = transform.position.y;

		Active = false;
	}

	// Update is called once per frame
	protected override void GameUpdate (){
		HandleInput ();
	}
	
	protected virtual void HandleInput(){
		/*if (((Input.GetKeyDown(KeyCode.D) || Input.GetButtonDown("X"))) && GameObject.FindGameObjectWithTag("Player").GetComponent<TreeController>().state != Tree.State.Eating)
		{
			if(luresLeft > 0 && !abilityInUse){
				AbilityStatusChangedMessage message = new AbilityStatusChangedMessage(true);
				MessageCenter.Instance.Broadcast(message);
				GameObjec obj = (GameObject)Resources.Load("Prefabs/Abilities/LurePlacer");
				LurePlacer lp = obj.GetComponent<LurePlacer>();
				lp.luresAllowed = luresLeft;
				GameObject.Instantiate(lp, transform.position, Quaternion.identity);
				abilityInUse = true;
			}
		}*/
        if (Input.GetButtonDown("B") && Active && GlobalGameStateManager.PosessionState == PosessionState.EXORCISABLE)
        {// && (GameObject.FindGameObjectWithTag ("Player").GetComponent<TreeController> ().state != Tree.State.Eating && GameObject.FindGameObjectWithTag ("Player").GetComponent<TreeController> ().state != Tree.State.AxeManMinigame)) {
			AbilityStatusChangedMessage message = new AbilityStatusChangedMessage(true);
			MessageCenter.Instance.Broadcast(message);
			exorcise();
		}
	}

	public virtual void possess(){
	}

	public virtual void exorcise(){
			Active = false;
			Instantiate(Resources.Load("Particles/soulsmoke"), transform.position, Quaternion.identity);
	}

}
