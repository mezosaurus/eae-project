using UnityEngine;
using System.Collections;
using System.Collections.Generic;

public class PossessorTrigger : GameBehavior {
	int speed = 5;
	Dictionary<string, Color> colors = new Dictionary<string, Color>();
	protected AbilityType type;
	private GameObject objectToPossess;
	public Texture2D possessionControls;
	public GameObject parent;
	// Use this for initialization
	protected virtual void Start () {
		colors.Add("transparent", new Color(1f, 1f, 1f, .5f));
		colors.Add("opaque", new Color(1f, 1f, 1f, 1f));
		objectToPossess = null;
	}

	// Update is called once per frame
	protected override void GameUpdate () {
		transform.position = parent.transform.position;
		HandleInput();
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
				Destroy(this.parent);
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
					item.possessionTexture.renderer.enabled = true;
					SpriteRenderer renderer = objectToPossess.GetComponent<SpriteRenderer>();
					Color color = Color.white;
					if(colors.TryGetValue("transparent", out color)){
						renderer.color = color;
					}
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
				item.possessionTexture.renderer.enabled = false;
				Color color = Color.white;
				if(colors.TryGetValue("opaque", out color)){
					renderer.color = color;
				}
			}
			objectToPossess = null;
		}
	}

}
