﻿using UnityEngine;
using System.Collections;
using System.Collections.Generic;

public class Possessor : GameBehavior {
	int speed = 5;
	Dictionary<string, Color> colors = new Dictionary<string, Color>();
	protected bool canPlace;
	bool colliding;
	bool possessing;
	protected AbilityType type;
	private GameObject objectToPossess;
	public Texture2D possessionControls;
	// Use this for initialization
	protected virtual void Start () {
		colors.Add("red", new Color(1f, .5f, .5f, .5f));
		colors.Add("green", new Color(.5f, 1f, .5f, .5f));
		colors.Add("blue", new Color(.5f, .5f, 1f, .5f));
		colors.Add("transparent", new Color(1f, 1f, 1f, .5f));
		colors.Add("opaque", new Color(1f, 1f, 1f, 1f));
		canPlace = true;
		colliding = false;
		possessing = false;
		objectToPossess = null;
		MessageCenter.Instance.Broadcast(new CameraChangeFollowedMessage(transform, Vector3.zero));
		MessageCenter.Instance.Broadcast(new PossessorSpawnedMessage(this));
		MessageCenter.Instance.RegisterListener (MessageType.EnemyNPCInvestigatingPlayer, ExitPossession);

	}

	// Update is called once per frame
	protected override void GameUpdate () {
		HandleInput();
	}

	protected virtual void HandleInput(){
		if(!possessing){
			Vector3 prevPosition = transform.position;
			Vector2 velocity = new Vector2(Input.GetAxis("LSX"), Input.GetAxis("LSY"));
			velocity = velocity * speed * Time.deltaTime;
			transform.position += (Vector3) velocity;
		}

		if(Input.GetButtonDown("B")){
			if(possessing){
				//MessageCenter.Instance.Broadcast(new CameraChangeFollowedMessage(GameObject.FindGameObjectWithTag("Player").transform, Vector3.zero));
				Possessable possessable = objectToPossess.GetComponent<Possessable>();
				possessable.exorcise();
				possessing = false;
				MessageCenter.Instance.Broadcast(new PossessorSpawnedMessage(this));
				SpriteRenderer face = this.gameObject.GetComponent<SpriteRenderer>();
				face.color = new Color(1f, 1f, 1f, 1f);
				//ExitPossession();
			}
		}

		if (Input.GetButtonDown("A")) {
			if(!possessing){
				if(objectToPossess != null){
					Possessable possessable = objectToPossess.GetComponent<Possessable>();
					possessable.possess();
					possessing = true;
					MessageCenter.Instance.Broadcast(new PossessorDestroyedMessage(this));
					SpriteRenderer renderer = objectToPossess.GetComponent<SpriteRenderer>();
					SpriteRenderer face = this.gameObject.GetComponent<SpriteRenderer>();
					face.color = new Color(1f, 1f, 1f, 0f);

					Color color = Color.white;
					if(colors.TryGetValue("opaque", out color)){
						renderer.color = color;
					}
				}
			}
		}
		
		if (Input.GetAxis("LT") > 0.2f) {
			if(possessing){
				Possessable possessable = objectToPossess.GetComponent<Possessable>();
				possessable.useAbility();
			}
		}
	}

	void OnTriggerEnter2D(Collider2D collider){
		if(collider.gameObject.GetComponent<Possessable>() != null){
			if(objectToPossess == null){
				objectToPossess = collider.gameObject;
				SpriteRenderer renderer = objectToPossess.GetComponent<SpriteRenderer>();
				Color color = Color.white;
				if(colors.TryGetValue("transparent", out color)){
					renderer.color = color;
				}
			}
		}
	}
	
	void OnTriggerExit2D(Collider2D collider){
		if(collider.gameObject.Equals(objectToPossess)){
			SpriteRenderer renderer = objectToPossess.GetComponent<SpriteRenderer>();
			Color color = Color.white;
			if(colors.TryGetValue("opaque", out color)){
				renderer.color = color;
			}
			objectToPossess = null;
		}
	}

	void ExitPossession(Message message){
		ExitPossession();
	}

	protected virtual void ExitPossession(){
		GameObject player = GameObject.Find ("Player");
		player.GetComponent<PlayerAbilityScript_v2>().abilityInUse = false;
		if (objectToPossess != null) {
			SpriteRenderer renderer = objectToPossess.GetComponent<SpriteRenderer>();
			Color color = Color.white;
			if(colors.TryGetValue("opaque", out color)){
				renderer.color = color;
			}
		}
		MessageCenter.Instance.Broadcast(new PossessorDestroyedMessage(this));
		MessageCenter.Instance.UnregisterListener (MessageType.EnemyNPCInvestigatingPlayer, ExitPossession);
		Destroy(this.gameObject);
	}

	void OnGUI(){
		float width = 197/2;
		float height = 121/2;

		float top = 0;
		float right = Screen.width - 50;
		if (possessing) {
			Debug.Log("here");
			Debug.Log(possessionControls);
			GUI.DrawTexture( new Rect(right - width,top + height, width, height), possessionControls, ScaleMode.ScaleToFit );
		}
	}

}
