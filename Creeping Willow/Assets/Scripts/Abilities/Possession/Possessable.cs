using UnityEngine;
using System.Collections;

public abstract class Possessable : GameBehavior {
	bool possessed;
	protected float baseX;
	protected float baseY;
	protected bool acting = false;
	bool needToSend = false;
	public AudioClip actionSound;

	void Start(){
		baseX = transform.position.x;
		baseY = transform.position.y;
		possessed = false;
		MessageCenter.Instance.RegisterListener (MessageType.PossessorSpawned, HandlePossessorSpawned);
		MessageCenter.Instance.RegisterListener (MessageType.PossessorDestroyed, HandlePossessorDestroyed);
	}
	// Update is called once per frame
	protected override void GameUpdate (){
		if(!acting){
			if(needToSend)
			{
				MessageCenter.Instance.Broadcast(new CameraChangeFollowedMessage(GameObject.FindGameObjectWithTag("Player").transform, Vector3.zero));
				needToSend = false;
			}
		}
	}
	protected abstract void act();

	public void useAbility(){
		if (possessed) {
			act();
			acting = true;
			needToSend = true;
		}
	}

	public virtual void possess(){
		possessed = true;
	}

	public virtual void exorcise(){
		possessed = false;
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
