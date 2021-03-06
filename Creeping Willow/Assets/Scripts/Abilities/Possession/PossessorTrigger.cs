﻿using UnityEngine;
using System.Collections;
using System.Collections.Generic;

public class PossessorTrigger : GameBehavior {
	int speed = 5;
	Dictionary<string, Color> colors = new Dictionary<string, Color>();
	protected AbilityType type;
	private GameObject objectToPossess;
	public Texture2D possessionControls;
	public GameObject parent;

    private float buttonScale, buttonScaleDirection, buttonLowerScale, buttonInitialScale, buttonUpperScale;
    bool triggered;

	// Use this for initialization
	protected virtual void Start () {
		colors.Add("transparent", new Color(1f, 1f, 1f, .5f));
		colors.Add("opaque", new Color(1f, 1f, 1f, 1f));
        objectToPossess = null;

        buttonScale = 1f;
        buttonScaleDirection = 1f;
        triggered = false;
	}

	// Update is called once per frame
	protected override void GameUpdate () {
		HandleInput();

        if(triggered)
        {
            buttonScale += (Time.deltaTime * buttonScaleDirection * 0.9f);

            if (buttonScale > buttonUpperScale)
            {
                buttonScale = buttonUpperScale;
                buttonScaleDirection = -1f;
            }

            if (buttonScale < buttonLowerScale)
            {
                buttonScale = buttonLowerScale;
                buttonScaleDirection = 1f;
            }

            objectToPossess.transform.GetChild(0).localScale = new Vector3(buttonScale, buttonScale, 1f);
        }
	}

	protected virtual void HandleInput(){
//			Vector3 prevPosition = transform.position;
//			Vector2 velocity = new Vector2(Input.GetAxis("LSX"), Input.GetAxis("LSY"));
//			velocity = velocity * speed * Time.deltaTime;
//			transform.position += (Vector3) velocity;

		if (Input.GetButtonDown("A")) {
			if(objectToPossess != null){
				Possessable possessable = objectToPossess.GetComponent<Possessable>();
				possessable.possess();
				//MessageCenter.Instance.Broadcast(new PossessorDestroyedMessage(this));
				if(possessable is PossessableItem){
					PossessableItem item = possessable as PossessableItem;
					item.possessionTexture.renderer.enabled = false;
					SpriteRenderer renderer = objectToPossess.GetComponent<SpriteRenderer>();


					Color color = Color.white;
					if(colors.TryGetValue("opaque", out color)){
						renderer.color = color;
					}
					Debug.Log ("playing sound");
				}
				Destroy(this.transform.parent.gameObject);
				Destroy(this.gameObject);

			}
		}
	}

	void OnTriggerEnter2D(Collider2D collider){
		if(collider.gameObject.GetComponent<Possessable>() != null){
			if(objectToPossess == null){
				objectToPossess = collider.gameObject;
				Possessable possessable = objectToPossess.GetComponent<Possessable>();
				if(possessable is PossessableItem){
					PossessableItem item = possessable as PossessableItem;
					SpriteRenderer renderer = objectToPossess.GetComponent<SpriteRenderer>();
					SpriteRenderer hintRenderer = item.possessionTexture.GetComponent<SpriteRenderer>();
					Color color = Color.white;
					if(colors.TryGetValue("transparent", out color)){
						renderer.color = color;
					}
					if(colors.TryGetValue("opaque", out color)){
						hintRenderer.color = color;
                    }

                    buttonInitialScale = objectToPossess.transform.GetChild(0).localScale.x;
                    buttonLowerScale = buttonInitialScale - 0.15f;
                    buttonUpperScale = buttonInitialScale + 0.15f;
                    triggered = true;
				}
			}
		}
	}
	
	void OnTriggerExit2D(Collider2D collider){
		if(collider.gameObject.Equals(objectToPossess)){
			Possessable possessable = objectToPossess.GetComponent<Possessable>();
			if(possessable is PossessableItem){
				SpriteRenderer renderer = objectToPossess.GetComponent<SpriteRenderer>();
				PossessableItem item = possessable as PossessableItem;
				SpriteRenderer hintRenderer = item.possessionTexture.GetComponent<SpriteRenderer>();
				Color color = Color.white;
				if(colors.TryGetValue("opaque", out color)){
					renderer.color = color;
				}
				if(colors.TryGetValue("transparent", out color)){
					hintRenderer.color = color;
                }

                buttonScale = 1f;
                buttonScaleDirection = 1f;
                triggered = false;
                objectToPossess.transform.GetChild(0).localScale = new Vector3(buttonInitialScale, buttonInitialScale, 1f);
			}
			objectToPossess = null;
		}
	}

}
