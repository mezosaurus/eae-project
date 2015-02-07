using UnityEngine;
using System.Collections;

public abstract class PossessableItem : Possessable {

	protected float baseX;
	protected float baseY;
	protected bool acting = false;
	bool needToSend = false;
	public AudioClip actionSound;

	protected virtual void Start(){
		baseX = transform.position.x;
		baseY = transform.position.y;
		Active = false;
		MessageCenter.Instance.RegisterListener (MessageType.PossessorSpawned, HandlePossessorSpawned);
		MessageCenter.Instance.RegisterListener (MessageType.PossessorDestroyed, HandlePossessorDestroyed);
	}

	protected virtual void OnDestroy()
	{
		MessageCenter.Instance.UnregisterListener (MessageType.PossessorSpawned, HandlePossessorSpawned);
		MessageCenter.Instance.UnregisterListener (MessageType.PossessorDestroyed, HandlePossessorDestroyed);
	}

	// Update is called once per frame
	protected override void GameUpdate (){
		HandleInput ();
	}
	protected abstract void scare();
	protected abstract void lure();

	public void useAbility(bool leftTrigger){
		if (Active) {
			if(leftTrigger){
				scare();
				audio.Stop();
				audio.clip = actionSound;
				audio.Play();
				acting = true;
				needToSend = true;
			}else{
				lure();
				audio.Stop();
				audio.clip = actionSound;
				audio.Play();
				acting = true;
				needToSend = true;
			}
		}
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
		
		if (Input.GetAxis("LT") > 0.2f) {
			if(Active){
				useAbility(true);
			}
		}else if (Input.GetAxis("RT") > 0.2f) {
			if(Active){
				useAbility(false);
			}
		}
	}

	public virtual void possess(){
		base.possess ();
		Active = true;
	}

	public virtual void exorcise(){
		base.exorcise ();
		Active = false;
	}

	void HandlePossessorSpawned(Message message){
		ParticleSystem ps = gameObject.GetComponent<ParticleSystem>();
		ps.Play();
//		Debug.Log ("spawned");
	}

	void HandlePossessorDestroyed(Message message){
		ParticleSystem ps = gameObject.GetComponent<ParticleSystem>();
		ps.Stop();
//		Debug.Log ("destroyed");
	}

}
