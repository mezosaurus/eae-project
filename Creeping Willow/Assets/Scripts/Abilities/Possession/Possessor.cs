using UnityEngine;
using System.Collections;
using System.Collections.Generic;

public class Possessor : GameBehavior {
	int speed = 5;
	Dictionary<string, Color> colors = new Dictionary<string, Color>();
	protected bool canPlace;
	bool colliding;
	protected AbilityType type;
	private GameObject objectToPossess;
	// Use this for initialization
	protected virtual void Start () {
		colors.Add("red", new Color(1f, .5f, .5f, .5f));
		colors.Add("green", new Color(.5f, 1f, .5f, .5f));
		colors.Add("blue", new Color(.5f, .5f, 1f, .5f));
		colors.Add("transparent", new Color(1f, 1f, 1f, .5f));
		colors.Add("opaque", new Color(1f, 1f, 1f, .5f));
		canPlace = true;
		colliding = false;
		objectToPossess = null;
	}
	
	// Update is called once per frame
	protected override void GameUpdate () {
		HandleInput();
	}

	protected virtual void HandleInput(){
		Vector2 velocity = new Vector2(Input.GetAxis("LSX"), Input.GetAxis("LSY"));
		velocity = velocity * speed * Time.deltaTime;
		transform.position += (Vector3) velocity;

		if(Input.GetButtonDown("X")){
			//Exit();
		}
	}

	void OnTriggerEnter2D(Collider2D collider){
		if(collider.gameObject.GetType().BaseType == typeof(Possessable)){
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
		if(collider.gameObject == objectToPossess){
			SpriteRenderer renderer = objectToPossess.GetComponent<SpriteRenderer>();
			Color color = Color.white;
			if(colors.TryGetValue("opaque", out color)){
				renderer.color = color;
			}
			objectToPossess = null;
		}
	}
	

	protected virtual void ExitPlacing(int numPlaced, AbilityType aType){
		AbilityObjectPlacedMessage objMess = new AbilityObjectPlacedMessage (numPlaced, aType);
		MessageCenter.Instance.Broadcast (objMess);
		AbilityStatusChangedMessage abMess = new AbilityStatusChangedMessage (false);
		MessageCenter.Instance.Broadcast (abMess);
		Destroy (this.gameObject);
	}

}
