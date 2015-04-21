using UnityEngine;
using System.Collections;

public abstract class PossessableItem : Possessable {
	
	public GameObject possessionTexture;
	public Vector2 hintOffset;

	protected virtual void Start(){
		base.Start ();
		possessionTexture = (GameObject)Instantiate(Resources.Load("prefabs/Abilities/PossessionClue"));
		possessionTexture.transform.parent = transform;
		possessionTexture.renderer.enabled = false;
		HintTextureScript alertTs = possessionTexture.GetComponent<HintTextureScript> ();
		alertTs.target = gameObject;
		MessageCenter.Instance.RegisterListener (MessageType.PossessorSpawned, ShowHint);
		MessageCenter.Instance.RegisterListener (MessageType.PossessorDestroyed, HideHint);
	}

	protected void ShowHint(Message message){
		this.possessionTexture.renderer.enabled = true;
	}

	protected void HideHint(Message message){
		this.possessionTexture.renderer.enabled = false;
	}

	public override void possess(){
		base.possess ();
		Active = true;
	}

	public override void exorcise(){
		base.exorcise ();
		Active = false;
	}

	void OnDestroy(){
		MessageCenter.Instance.UnregisterListener (MessageType.PossessorSpawned, ShowHint);
		MessageCenter.Instance.UnregisterListener (MessageType.PossessorDestroyed, HideHint);
	}
}
