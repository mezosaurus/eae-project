using UnityEngine;
using System.Collections;

public abstract class PossessableItem : Possessable {
	
	protected bool shaking = false;
	protected bool blinking = false;
	float blinkingColor = 0.0f;
	float shakeAmount = 1.0f;

	protected virtual void Start(){
		colors.Add("red", new Color(1.0f, 0.0f, 0.0f, 1.0f));
		colors.Add("green", new Color(0.0f, 1.0f, 0.0f, 1.0f));
		colors.Add("blue", new Color(0.0f, 0.0f, 1.0f, 1.0f));
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
		if(shaking){
			float newX = Random.Range(baseX-.05f, baseX+.05f);
			float newY = Random.Range(baseY-.05f, baseY+.05f);
			shakeAmount -= .1f;
			if(shakeAmount <= 0f){
				shaking = false;
				acting = false;
				newX = baseX;
				newY = baseY;
			}
			transform.position = new Vector3(newX, newY);

            Debug.Log("Do we get here?");
		}
		if(blinking){
			SpriteRenderer renderer = gameObject.GetComponent<SpriteRenderer>();
			Color color = Color.white;
			string key = "";
			if(blinkingColor < 1.0f){
				key = "red";
			}
			else if(blinkingColor < 2.0f){
				key = "green";
			}else{
				key = "blue";
			}
			if(colors.TryGetValue(key, out color)){
				Debug.Log("Changing Color");
				renderer.color = color;
			}
			blinkingColor += .5f;
			if(blinkingColor > 3.0f){
				blinking = false;
				renderer.color = Color.white;
				blinkingColor = 0.0f;
			}
		}
	}
	protected virtual void scare(){
		shakeAmount = 1.0f;
	}
	protected virtual void lure(){
		//blinkingColor = 0.0f;
	}

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
				acting = true;
				needToSend = true;
			}
		}
	}
	
	protected override void  HandleInput(){
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
		base.HandleInput();
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

	public override void possess(){
		base.possess ();
		Active = true;
	}

	public override void exorcise(){
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
