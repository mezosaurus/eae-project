using UnityEngine;
using System.Collections;
using System.Collections.Generic;

public abstract class ObjectPlacer : GameBehavior {
	int speed = 5;
	Dictionary<string, Color> colors = new Dictionary<string, Color>();
	protected bool canPlace;
	bool colliding;
	protected AbilityType type;
	// Use this for initialization
	protected virtual void Start () {
		colors.Add("red", new Color(1f, .5f, .5f, .5f));
		colors.Add("green", new Color(.5f, 1f, .5f, .5f));
		colors.Add("blue", new Color(.5f, .5f, 1f, .5f));
		colors.Add("white", new Color(1f, 1f, 1f, .5f));
		canPlace = true;
		colliding = false;
	}
	
	// Update is called once per frame
	protected override void GameUpdate () {
		GameObject player = GameObject.Find ("Player");
		if (Vector3.Distance (player.transform.position, transform.position) > 5) {
			canPlace = false;
		}else{
			canPlace = !colliding;
		}
		if (canPlace == false) {
			SpriteRenderer renderer = gameObject.GetComponent<SpriteRenderer>();
			Color color = Color.white;
			if(colors.TryGetValue("red", out color)){
				renderer.color = color;
			}
		}else{
			SpriteRenderer renderer = gameObject.GetComponent<SpriteRenderer>();
			Color color = Color.white;
			if(colors.TryGetValue("white", out color)){
				renderer.color = color;
			}
		}
		HandleInput();
	}

	protected virtual void HandleInput(){
		Vector2 velocity = new Vector2(Input.GetAxis("LSX"), Input.GetAxis("LSY"));
		velocity = velocity * speed * Time.deltaTime;
		transform.position += (Vector3) velocity;

		if(Input.GetButtonDown("X")){
			Exit();
		}
	}

	void OnTriggerEnter2D(Collider2D collider){
		if(collider.GetType() == typeof(BoxCollider2D)||
		   collider.GetType() == typeof(EdgeCollider2D)){
			colliding = true;
		}
	}
	
	void OnTriggerExit2D(Collider2D collider){
		
		if(collider.GetType() == typeof(BoxCollider2D)||
		   collider.GetType() == typeof(EdgeCollider2D)){
			colliding = false;
		}
	}

	protected abstract void Exit ();

	protected virtual void ExitPlacing(int numPlaced, AbilityType aType){
		AbilityObjectPlacedMessage objMess = new AbilityObjectPlacedMessage (numPlaced, aType);
		MessageCenter.Instance.Broadcast (objMess);
		AbilityStatusChangedMessage abMess = new AbilityStatusChangedMessage (false);
		MessageCenter.Instance.Broadcast (abMess);
		Destroy (this.gameObject);
	}

}
