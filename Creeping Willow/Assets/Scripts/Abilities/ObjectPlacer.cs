using UnityEngine;
using System.Collections;
using System.Collections.Generic;

public class ObjectPlacer : GameBehavior {
	int speed = 2;
	Dictionary<string, Color> colors = new Dictionary<string, Color>();
	// Use this for initialization
	void Start () {
		colors.Add("red", new Color(1f, .5f, .5f, .5f));
		colors.Add("green", new Color(.5f, 1f, .5f, .5f));
		colors.Add("blue", new Color(.5f, .5f, 1f, .5f));
	}
	
	// Update is called once per frame
	void GameUpdate () {
		HandleInput();
	}

	void HandleInput(){
		Vector2 velocity = new Vector2(Input.GetAxis("LSX"), Input.GetAxis("LSY"));
		velocity = velocity * speed * Time.deltaTime;
		rigidbody2D.velocity = velocity;
	}


}
