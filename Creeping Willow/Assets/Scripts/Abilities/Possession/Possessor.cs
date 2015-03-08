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
	public Texture2D possessionControls;
	private GameObject triggerObject;
	// Use this for initialization
	protected virtual void Start () {
		Physics2D.IgnoreLayerCollision (8, 10, true);
		Physics2D.IgnoreLayerCollision (0, 10, true);
		colors.Add("transparent", new Color(1f, 1f, 1f, .5f));
		colors.Add("opaque", new Color(1f, 1f, 1f, 1f));
		canPlace = true;
		colliding = false;
		possessing = false;
		objectToPossess = null;
		ParticleSystem particleSystem = gameObject.GetComponent<ParticleSystem> ();
		particleSystem.renderer.sortingLayerID = 2;
		particleSystem.renderer.sortingOrder = -1;
		MessageCenter.Instance.Broadcast(new CameraChangeFollowedMessage(transform, Vector3.zero));
		MessageCenter.Instance.Broadcast(new PossessorSpawnedMessage(this));
	}

	// Update is called once per frame
	protected override void GameUpdate () {
		HandleInput();
	}

	protected virtual void HandleInput(){
			Vector3 prevPosition = transform.position;
			Vector2 velocity = new Vector2(Input.GetAxis("LSX"), Input.GetAxis("LSY"));
			velocity = velocity * speed * Time.deltaTime;
			transform.position += (Vector3) velocity;

//		if (Input.GetButtonDown("A")) {
//			if(objectToPossess != null){
//				Possessable possessable = objectToPossess.GetComponent<Possessable>();
//				possessable.possess();
//				MessageCenter.Instance.Broadcast(new PossessorDestroyedMessage(this));
//				if(possessable is PossessableItem){
//					PossessableItem item = possessable as PossessableItem;
//					item.possessionTexture.renderer.enabled = false;
//					SpriteRenderer renderer = objectToPossess.GetComponent<SpriteRenderer>();
//					SpriteRenderer face = this.gameObject.GetComponent<SpriteRenderer>();
//					face.color = new Color(1f, 1f, 1f, 0f);

//					Color color = Color.white;
//					if(colors.TryGetValue("opaque", out color)){
//						renderer.color = color;
//					}
//					Debug.Log ("playing sound");
//				}
//				Destroy(this.gameObject);
//			}
//		}
	}

//	void OnTriggerEnter2D(Collider2D collider){
//		if(collider.gameObject.GetComponent<Possessable>() != null){
//			if(objectToPossess == null){
//				objectToPossess = collider.gameObject;
//				Possessable possessable = objectToPossess.GetComponent<Possessable>();
//				if(possessable is PossessableItem){
//					PossessableItem item = possessable as PossessableItem;
//					item.possessionTexture.renderer.enabled = true;
//					SpriteRenderer renderer = objectToPossess.GetComponent<SpriteRenderer>();
//					Color color = Color.white;
//					if(colors.TryGetValue("transparent", out color)){
//						renderer.color = color;
//					}
//				}
//			}
//		}
//	}
	
//	void OnTriggerExit2D(Collider2D collider){
//		if(collider.gameObject.Equals(objectToPossess)){
//			Possessable possessable = objectToPossess.GetComponent<Possessable>();
//			if(possessable is PossessableItem){
//				SpriteRenderer renderer = objectToPossess.GetComponent<SpriteRenderer>();
//				PossessableItem item = possessable as PossessableItem;
//				item.possessionTexture.renderer.enabled = false;
//				Color color = Color.white;
//				if(colors.TryGetValue("opaque", out color)){
//					renderer.color = color;
//				}
//			}
//			objectToPossess = null;
//		}
//	}

//	void ExitPossession(Message message){
//		ExitPossession();
//	}

//	protected virtual void ExitPossession(){
//		GameObject player = GameObject.Find ("Player");
//		player.GetComponent<PlayerAbilityScript_v2>().abilityInUse = false;
//		if (objectToPossess != null) {
//			SpriteRenderer renderer = objectToPossess.GetComponent<SpriteRenderer>();
//			Color color = Color.white;
//			if(colors.TryGetValue("opaque", out color)){
//				renderer.color = color;
//			}
//		}
//		MessageCenter.Instance.Broadcast(new PossessorDestroyedMessage(this));
//		MessageCenter.Instance.UnregisterListener (MessageType.EnemyNPCInvestigatingPlayer, ExitPossession);
//		Destroy(this.gameObject);
//	}

	void OnDestroy(){
		MessageCenter.Instance.Broadcast(new PossessorDestroyedMessage(this));
	}
	void OnGUI(){
		float width = 197/2;
		float height = 121/2;

		float top = 0;
		float right = Screen.width - 50;
		if (possessing) {
			GUI.DrawTexture( new Rect(right - width,top + height, width, height), possessionControls, ScaleMode.ScaleToFit );
		}
	}

}
