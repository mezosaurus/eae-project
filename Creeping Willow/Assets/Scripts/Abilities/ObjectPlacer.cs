using UnityEngine;
using System.Collections;
using System.Collections.Generic;

public class ObjectPlacer : GameBehavior {
	int speed = 5;
	Dictionary<string, Color> colors = new Dictionary<string, Color>();
	bool canPlace;
	// Use this for initialization
	void Start () {
		colors.Add("red", new Color(1f, .5f, .5f, .5f));
		colors.Add("green", new Color(.5f, 1f, .5f, .5f));
		colors.Add("blue", new Color(.5f, .5f, 1f, .5f));
		colors.Add("white", new Color(1f, 1f, 1f, .5f));
		canPlace = true;
	}
	
	// Update is called once per frame
	protected override void GameUpdate () {
		GameObject player = GameObject.Find ("BushTrap");
		if (Vector3.Distance (player.transform.position, transform.position) > 5) {
			canPlace = false;
		}else{
			canPlace = true;
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

	void HandleInput(){
		Vector2 velocity = new Vector2(Input.GetAxis("LSX"), Input.GetAxis("LSY"));
		if(Input.GetButtonDown("A")&& canPlace){
			AbilityPlacedMessage message = new AbilityPlacedMessage (transform.position.x,transform.position.y, AbilityType.Lure);
			MessageCenter.Instance.Broadcast (message);
		}
		velocity = velocity * speed * Time.deltaTime;
		//rigidbody2D.velocity = velocity;
		transform.position += (Vector3) velocity;
	}

	void OnTriggerEnter2D(Collider2D collider){
		if(collider.GetType() == typeof(BoxCollider2D)){
			//canPlace = false;
		}
	}
	
	void OnTriggerExit2D(Collider2D collider){
		if(collider.GetType() == typeof(BoxCollider2D)){
			//canPlace = true;
		}
	}

}
