using UnityEngine;
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
	}
	
	// Update is called once per frame
	protected override void GameUpdate () {
		HandleInput();
	}

	protected virtual void HandleInput(){
		if(!possessing){

			GameObject player = GameObject.Find ("Player");
			Vector3 prevPosition = transform.position;
			Vector2 velocity = new Vector2(Input.GetAxis("LSX"), Input.GetAxis("LSY"));
			velocity = velocity * speed * Time.deltaTime;
			transform.position += (Vector3) velocity;
			
			if (Vector3.Distance (player.transform.position, transform.position) > 5) {
				transform.position = prevPosition;
			}
		}

		if(Input.GetButtonDown("B")){
			if(possessing){
				Possessable possessable = objectToPossess.GetComponent<Possessable>();
				possessable.exorcise();
				possessing = false;
				ExitPossession();
			}
		}

		if (Input.GetButtonUp("A")) {
			if(!possessing){
				if(objectToPossess != null){
					Possessable possessable = objectToPossess.GetComponent<Possessable>();
					possessable.possess();
					possessing = true;
					SpriteRenderer renderer = objectToPossess.GetComponent<SpriteRenderer>();
					Color color = Color.white;
					if(colors.TryGetValue("opaque", out color)){
						renderer.color = color;
					}
				}else{
					ExitPossession();
				}
			}
		}
		
		if (Input.GetButtonDown("A")) {
			if(possessing){
				Possessable possessable = objectToPossess.GetComponent<Possessable>();
				possessable.useAbility();
				ExitPossession();
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
		Destroy(this.gameObject);
	}

}
