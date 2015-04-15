using UnityEngine;
using System.Collections;

//using UnityEditor;

public class AIController : GameBehavior
{
	public class NPCDirection
	{
		public static Vector3 T = new Vector3 (0, 1),
		TL = new Vector3 (-Mathf.Sqrt (2) / 2, Mathf.Sqrt (2) / 2),
		TR = new Vector3 (Mathf.Sqrt (2) / 2, Mathf.Sqrt (2) / 2),
		B = new Vector3 (0, -1),
		BL = new Vector3 (-Mathf.Sqrt (2) / 2, -Mathf.Sqrt (2) / 2),
		BR = new Vector3 (Mathf.Sqrt (2) / 2, -Mathf.Sqrt (2) / 2),
		L = new Vector3 (-1, 0),
		R = new Vector3 (0, 1);
	}

	// Tags
	public string spawnTag = "Respawn";
	public string npcTag = "NPC";
	public string lureTag = "Lure";

	// Skin Type
	public NPCSkinType SkinType;
	public bool IsTaggedByTree = false;
	public bool isCritterType = false;

	// Global Keys
	protected string walkingKey = "direction";
	protected enum WalkingDirection
	{
		STILL_DOWN_LEFT = 0,
		MOVING_DOWN_LEFT = 1,
		MOVING_UP_LEFT = 2,
		STILL_ACTION = 3,
		STILL_DOWN_RIGHT = 4,
		MOVING_DOWN_RIGHT = 5,
		MOVING_UP_RIGHT = 6,
		PANIC_DOWN_LEFT = 7,
		PANIC_UP_LEFT = 8,
		PANIC_DOWN_RIGHT = 9,
		PANIC_UP_RIGHT = 10,
	}

	// NPC variables
	//public float nourishment = 1;			// Nourishment for player goal (Not currently Implemented)
	public float lurePower = 3;
	public float speed = 1;					// NPC speed (when not running)
	public float scaredCooldownSeconds = 6;	// How long the NPC will be scared for
	public float panicCooldownSeconds = 2;	// How long the NPC will be panicked for
	public float lureCooldownSeconds = 2;
	public float detectLevel = 0.2f; 		// This variable determines an NPC's awareness, or how easily they are able to detect things going on in their surroundings. Range (0,1)
	public float visionDistance = 3; 		// distance that player's view extends to
	public float visionAngleSize = 120; 	// The total angle size that the player can see
	public float hearingAlertMultiplier = 1.0f;	// How aware they are of their hearing
	public float sightAlertMultiplier = 1.5f; // How aware they are of their sight

	// NPC state variables
	public bool alerted;
	public bool grabbed;
	public bool panicked;
	public bool scared;
	public bool lured;
	protected bool playerInRange;
	protected bool nearWall;
	public bool killSelf = false;
	protected bool enteredMap;

	// Pre-population variables
	public int numPathStart;
	public int numStationaryStart;

	// Scene variables
	protected Lure lastLure;
	protected GameObject player;

	// Movement variables
	protected Vector2 moveDir;
	protected GameObject nextPath;
	protected SubpathScript movePath;

	// Scared variables
	protected float scaredTimeLeft;

	// lure variables
	protected float luredTimeLeft;

	// Panic variables
	protected float panicThreshold = 10;
	protected Vector3 panickedPos;
	protected float previousAlertLevel = 0.0f;
	//protected float panicTime;
	//protected float timePanicked;

	// Alert variables
	protected float alertThreshold = 5;
	protected float alertLevel;
	public float alertDecrement;
	public Texture2D emptyAlert;
	public Texture2D fullAlert;

	// Cone of Vision variables
	protected float visionAngleOffset; // max offset of angle from NPC's view
	protected Vector3 npcDir;

	// Sprite
	protected float xScale;
	protected bool lastDirectionWasRight = false;
	public GameObject alertTexture;
	public GameObject panicTexture;
	public GameObject scaredTexture;

	// collision detection efficiency
	int avoidCounter = 0;
	Vector3 avoidCurrentDirection;

	/********** NPC SOUNDS **********/
	// Mowing
	public AudioClip mowing;
	// Eaten
	public AudioClip bopperEaten;
	public AudioClip hippieEaten;
	public AudioClip mowerEaten;
	public AudioClip oldmanEaten;
	public AudioClip hottieEaten;
	// Curious
	public AudioClip[] bopperCuriousSounds;
	public AudioClip[] hippieCuriousSounds;
	public AudioClip[] mowerCuriousSounds;
	public AudioClip[] oldmanCuriousSounds;
	public AudioClip[] hottieCuriousSounds;
	// Idle
	public AudioClip[] bopperIdleSounds;
	public AudioClip[] hippieIdleSounds;
	public AudioClip[] mowerIdleSounds;
	public AudioClip[] oldmanIdleSounds;
	public AudioClip[] hottieIdleSounds;
	protected float idlePlayTime = 90.0f;
	protected float idleCounter = 0.0f;
	// Alert 
	public AudioClip[] bopperAlertSounds;
	public AudioClip[] hippieAlertSounds;
	public AudioClip[] mowerAlertSounds;
	public AudioClip[] oldmanAlertSounds;
	public AudioClip[] hottieAlertSounds;
	// Panic
	public AudioClip[] bopperPanicSounds;
	public AudioClip[] hippiePanicSounds;
	public AudioClip[] mowerPanicSounds;
	public AudioClip[] oldmanPanicSounds;
	public AudioClip[] hottiePanicSounds;
	// Boolean control vars
	protected bool playAlert = true;
	//protected bool playPanic = true;
	protected bool playCurious = true;
	protected bool playIdle = true;

	protected bool marked = false;

	/*void OnGUI() {
	int sizeX = 20;
	int sizeY = 60;
	GameObject camera = GameObject.Find ("Main Camera");
	Vector3 drawPos = camera.camera.WorldToScreenPoint(new Vector3 (transform.position.x, transform.position.y + (gameObject.renderer.bounds.size.y / 2), 0));
	float top = drawPos.y;
	Debug.Log ("top = " + top);
	float left = drawPos.x;
	Debug.Log ("left = " + left);
	float alertPercent = (alertLevel / alertThreshold) * 100;

	int alertGUI = EditorGUI.IntSlider(new Rect(sizeX,sizeY,left,top), "Alert:", 10, 0, 100);
	EditorGUI.ProgressBar(new Rect(left,top,left,top+5),alertGUI, "Alert");
	//if (alertLevel > 0)
	{
		

		Debug.Log ("alertPercent =  " + alertPercent);
		GUI.Box( new Rect( left, top, sizeX, sizeY ), GUIContent.none );
		GUI.Box( new Rect( left, top + sizeY * ( 1 - alertPercent ), sizeX, sizeY * alertPercent ), GUIContent.none );
	}*/
	//draw the background:
	/*GUI.BeginGroup(new Rect(transform.position.x, (transform.position.y + gameObject.renderer.bounds.size.y/2), sizeX, sizeY));
	GUI.Box (new Rect (0, 0, 20, 60), emptyAlert);
	GUI.BeginGroup(new Rect(0,0, sizeX * alertLevel, sizeY));
	GUI.Box(new Rect(0,0, sizeX, sizeY), fullAlert);

	GUI.EndGroup();
	GUI.EndGroup();
}*/

	// Use this for initialization
	public virtual void Start ()
	{
		if (this.SkinType.Equals (NPCSkinType.MowerMan)) 
		{
			audio.clip = mowing;
			audio.Play();
		}
		enteredMap = false;
		// Alert Texture
		alertTexture = (GameObject)Instantiate (Resources.Load ("prefabs/AI/SceneInitPrefabs/AIAlert"));
		alertTexture.transform.SetParent (transform, false);
		alertTexture.renderer.enabled = false;
		TextureScript alertTs = alertTexture.GetComponent<TextureScript> ();
		alertTs.target = gameObject;

		// Panic Texture
		panicTexture = (GameObject)Instantiate (Resources.Load ("prefabs/AI/SceneInitPrefabs/AIPanic"));
		panicTexture.transform.SetParent (transform, true);
		panicTexture.renderer.enabled = false;
		TextureScript panicTs = panicTexture.GetComponent<TextureScript> ();
		panicTs.target = gameObject;

		// Scare Texture
		scaredTexture = (GameObject)Instantiate (Resources.Load ("prefabs/AI/SceneInitPrefabs/AIScared"));
		scaredTexture.transform.SetParent (transform, true);
		scaredTexture.renderer.enabled = false;
		TextureScript scaredTs = scaredTexture.GetComponent<TextureScript> ();
		scaredTs.target = gameObject;

		player = GameObject.FindGameObjectWithTag ("Player");
		// Set initial alert/panick states
		//timePanicked = panicCooldownSeconds;
		alerted = false;
		panicked = false;
		alertLevel = 0f;
		playerInRange = false;
		visionAngleOffset = .5f * visionAngleSize;

		// TODO: Make this vector match NPC's view
		//forwardLookingDirection = new Vector3 (-1, 0);
		npcDir = NPCDirection.L;
		xScale = transform.localScale.x;

		// Register for all messages that entre necessary
		MessageCenter.Instance.RegisterListener (MessageType.PlayerGrabbedNPCs, grabbedListener);
		MessageCenter.Instance.RegisterListener (MessageType.NPCEaten, eatenListener);
		MessageCenter.Instance.RegisterListener (MessageType.PlayerReleasedNPCs, releasedListener);
		MessageCenter.Instance.RegisterListener (MessageType.TrapEntered, trapEnterListener);
		MessageCenter.Instance.RegisterListener (MessageType.TrapReleased, trapReleaseListener);
		MessageCenter.Instance.RegisterListener (MessageType.LureRadiusEntered, lureEnterListener);
		MessageCenter.Instance.RegisterListener (MessageType.LureReleased, lureReleaseListener);
		MessageCenter.Instance.RegisterListener (MessageType.AbilityPlaced, abilityPlacedListener);
		MessageCenter.Instance.RegisterListener (MessageType.NewMarkedBounty, markedBountyListener);

		// Ignore collision with other AI
		int npcLayer = LayerMask.NameToLayer (npcTag);
		Physics2D.IgnoreLayerCollision (npcLayer, npcLayer);
		Physics2D.IgnoreLayerCollision (npcLayer, 9);


		// Broadcast start of lifecycle
		MessageCenter.Instance.Broadcast (new NPCCreatedMessage (gameObject));
	}

	void OnDestroy ()
	{
		// Unregister for all messages that were previously registered for
		MessageCenter.Instance.UnregisterListener (MessageType.PlayerGrabbedNPCs, grabbedListener);
		MessageCenter.Instance.UnregisterListener (MessageType.NPCEaten, eatenListener);
		MessageCenter.Instance.UnregisterListener (MessageType.PlayerReleasedNPCs, releasedListener);
		MessageCenter.Instance.UnregisterListener (MessageType.TrapEntered, trapEnterListener);
		MessageCenter.Instance.UnregisterListener (MessageType.TrapReleased, trapReleaseListener);
		MessageCenter.Instance.UnregisterListener (MessageType.LureRadiusEntered, lureEnterListener);
		MessageCenter.Instance.UnregisterListener (MessageType.LureReleased, lureReleaseListener);
		MessageCenter.Instance.UnregisterListener (MessageType.AbilityPlaced, abilityPlacedListener);
		MessageCenter.Instance.UnregisterListener (MessageType.NewMarkedBounty, markedBountyListener);

		if (alertTexture != null) {
				Destroy (alertTexture.gameObject);
		}
		if (panicTexture != null) {
				Destroy (panicTexture.gameObject);
		}
		if (scaredTexture != null)
				Destroy (scaredTexture.gameObject);

		if (marked)
		{
			MessageCenter.Instance.Broadcast (new MarkedBountyDestroyedMessage());
		}

		NPCDestroyedMessage message = new NPCDestroyedMessage (gameObject, false);
		MessageCenter.Instance.Broadcast (message);

		NPCOnDestroy ();
	}

	protected virtual void NPCOnDestroy ()
	{

	}

	//-----------------------
	// Collision Methods
	//-----------------------

	protected virtual void OnCollisionEnter2D (Collision2D collision)
	{
	}

	protected virtual void OnCollisionExit2D (Collision2D collision)
	{
	}

	//-----------------------
	// Trigger Methods
	//-----------------------

	protected virtual void OnTriggerExit2D (Collider2D other)
	{
		if (other.tag == "Player") 
		{
			playerInRange = false;
		}
		if (other.tag == "Border") 
		{
			if (panicked) 
			{
				NPCPanickedOffMapMessage message = new NPCPanickedOffMapMessage (panickedPos);
				MessageCenter.Instance.Broadcast (message);
				destroyNPC ();
			}
			enteredMap = true;
			ignoreBorder (false, other);
		}
		if (other.tag.Equals ("PossessorTrigger")) 
		{
			alertLevel = previousAlertLevel;
			decrementAlertLevel ();
		}
	}

	protected virtual void OnTriggerEnter2D (Collider2D other)
	{
		if (other.tag == "Player") 
		{
			playerInRange = true;
			if( getPlayer() == null )
				return;
			PossessableTree script = getPlayer ().GetComponent<PossessableTree> ();
			if (script != null && script.Eating) 
			{
				panic ();
			}
		}
		if (other.tag == "Border") 
		{
			// Only ignore collisions with the border if the NPC has not entered the map yet
			if (!enteredMap || killSelf || panicked) 
			{
				ignoreBorder (true, other);
			}
		}
	}

	protected virtual void OnTriggerStay2D (Collider2D other)
	{
		if (other.tag == "Wall") 
		{
			RaycastHit2D raycast = Physics2D.Raycast (transform.position, moveDir);
			if (raycast.collider != null && raycast.collider.tag == "Wall") 
			{
				float distance = Vector2.Distance (transform.position, raycast.point);
				if (distance < 1.5f)
					nearWall = true;
				else
					nearWall = false;
			}
		} 
		//else if (other.tag == "Player") 
		else if (other.gameObject.Equals(getPlayer())) 
		{
			if (panicked) 
			{
				//timePanicked = panicCooldownSeconds;
				return;
			}

			if (alertLevel >= alertThreshold) 
			{
				//Debug.Log("ALERTED");
				alert ();
			}

			if (alertLevel >= panicThreshold) 
			{
				//Debug.Log("PANICKED");
                GlobalGameStateManager.PanicTree = GetClosestPlayer(transform.position);

                //GlobalGameStateManager.PanicTree.GetComponent<PossessableTree>().BodyParts.Trunk.GetComponent<SpriteRenderer>().color = Color.red;

				panic ();
			}

			PossessableTree script = getPlayer ().GetComponent<PossessableTree> ();
			if (script != null && script.Eating) 
			{
				panic ();
			}

			// Increment alertLevel
			increaseAlertLevel (hearingAlertMultiplier);
		}
		if (other.tag.Equals ("PossessorTrigger")) 
		{
			if (panicked || alerted)
				return;

			if (Vector2.Distance (other.gameObject.transform.position, gameObject.transform.position) > (0.5f * GetComponent<CircleCollider2D> ().radius))
				return;

			previousAlertLevel = alertLevel;
			if (previousAlertLevel < alertThreshold) 
			{
				previousAlertLevel = (panicThreshold - alertThreshold) / 4 + alertThreshold;
			}
			alertLevel = panicThreshold - 0.01f;
			alert ();
		}
	}

    private GameObject GetClosestPlayer(Vector3 position)
    {
        GameObject player = null;
        float distance = float.MaxValue;

        foreach (PossessableTree tree in FindObjectsOfType<PossessableTree>())
        {
            float d = Vector3.Distance(tree.transform.position, position);

            if (d < distance)
            {
                distance = d;
                player = tree.gameObject;
            }
        }

        return player;
    }

	protected bool updateNPC ()
	{
		// Play idle sound0
		if (idleCounter > idlePlayTime)
		{
			idlePlayTime = Random.Range (90.0f, 120.0f);
			idleCounter = 0.0f;
			AudioClip idle = null;
			// Get skin type to know which NPC sounds to play
			if (this.SkinType.Equals (NPCSkinType.Bopper))
			{
				idle = (AudioClip)bopperIdleSounds[Random.Range (0, bopperIdleSounds.Length)];
			}
			else if (this.SkinType.Equals (NPCSkinType.Hippie))
			{
				idle = (AudioClip)hippieIdleSounds[Random.Range (0, hippieIdleSounds.Length)];
			}
			else if (this.SkinType.Equals (NPCSkinType.Hottie))
			{
				idle = (AudioClip)hottieIdleSounds[Random.Range (0, hottieIdleSounds.Length)];
			}
			else if (this.SkinType.Equals (NPCSkinType.MowerMan))
			{
				idle = (AudioClip)mowerIdleSounds[Random.Range (0, mowerIdleSounds.Length)];
			}
			else if (this.SkinType.Equals (NPCSkinType.OldMan))
			{
				idle = (AudioClip)oldmanIdleSounds[Random.Range (0, oldmanIdleSounds.Length)];
			}
			
			audio.PlayOneShot (idle, 1.0f);
		}
		idleCounter += Time.deltaTime;

		if (grabbed)
			return true;

		if (playerInRange) 
		{
			Vector2 playerSpeed = getPlayer ().rigidbody2D.velocity;
			if (playerSpeed == Vector2.zero && alertLevel > 0) 
			{
				decrementAlertLevel ();
			}
		} 
		else if (alertLevel > 0) 
		{
			decrementAlertLevel ();
		}

		if (alerted)
			return true;

		if (panicked) 
		{
			/*
			timePanicked -= Time.deltaTime;
			if (timePanicked <= 0)
			{
				panicked = false;
				panicTexture.renderer.enabled = false;
				speed = 1;
				alertLevel = alertThreshold - 0.1f;
				broadcastAlertLevelChanged(AlertLevelType.Normal);
				return true;
			}
			 */

			if (nearWall) 
			{
				nearWall = false;
				moveDir = Quaternion.AngleAxis (90, transform.forward) * -moveDir;
			}

			Vector3 npcPosition = transform.position;
			float step = speed * Time.deltaTime;
			Vector3 movement = Vector3.MoveTowards (npcPosition, new Vector3 (moveDir.normalized.x * step, moveDir.normalized.y * step) + npcPosition, step);
			//transform.position = new Vector3(moveDir.normalized.x * step, moveDir.normalized.y * step) + npcPosition;
			determineDirectionChange (npcPosition, transform.position);

			Vector3 direction = Vector3.Normalize (movement - transform.position);
			Vector3 changeMovement = avoid (direction);

			if (changeMovement != Vector3.zero) 
			{
				Vector3 newPos = Vector3.MoveTowards (npcPosition, changeMovement, step);
				determineDirectionChange (transform.position, newPos);
				transform.position = newPos;
			} 
			else 
			{
				determineDirectionChange (transform.position, movement);
				transform.position = movement;
			}

			// OLD
			//Vector3 oldVelocity = new Vector3(rigidbody2D.velocity.x, rigidbody2D.velocity.y);
			//rigidbody2D.velocity = moveDir.normalized * speed;
			//determineDirectionChange(oldVelocity, new Vector3(rigidbody2D.velocity.x, rigidbody2D.velocity.y));

			return true;
		}

		player = getPlayer ();
		if (checkForPlayer () && player.rigidbody2D != null && player.rigidbody2D.velocity != Vector2.zero) 
		{
			if (NPCHandleSeeingPlayer ())
				return true;
		}

		if (scared) 
		{
			scaredTimeLeft -= Time.deltaTime;
			if (scaredTimeLeft <= 0) 
			{
				scared = false;
				scaredTexture.renderer.enabled = false;
			}

			Vector3 npcPosition = transform.position;
			float step = speed * Time.deltaTime;
			transform.position = new Vector3 (moveDir.normalized.x * step, moveDir.normalized.y * step) + npcPosition;
			determineDirectionChange (npcPosition, transform.position);

			return true;
		}

		if (lured) 
		{
			return handleLured ();
		}

		return false;
	}

	//-----------------------
	// Helper Methods
	//-----------------------

	protected GameObject createTempGameObject(Vector3 pos, Transform parent)
	{
		GameObject tempObject = new GameObject ();
		tempObject.transform.position = pos;
		//tempObject.transform.SetParent (parent, true);
		
		return tempObject;
	}

	virtual protected bool NPCHandleSeeingPlayer ()
	{
		// TODO: Balance better
		increaseAlertLevel (sightAlertMultiplier);
		if (alertLevel >= alertThreshold) 
		{
			alert ();
			return true;
		} 
		else if (alertLevel >= panicThreshold) 
		{
			panic ();
			return true;
		}

		return false;
	}

	protected void destroyNPC ()
	{
		Destroy (gameObject);		
	}

	protected void ignoreBorder (bool ignore, Collider2D other)
	{
		BoxCollider2D box = gameObject.GetComponent<BoxCollider2D> ();
		Physics2D.IgnoreCollision (other, box, ignore);
	}

	protected void setAnimatorInteger (int animation)
	{
		Animator anim = gameObject.GetComponent<Animator> ();
		if (anim != null) 
		{
			gameObject.GetComponent<Animator> ().SetInteger (walkingKey, animation);
		}
	}

	protected void setAnimatorInteger (string key, int animation)
	{
		Animator anim = gameObject.GetComponent<Animator> ();
		if (anim != null) 
		{
			gameObject.GetComponent<Animator> ().SetInteger (key, animation);
		}
	}

	protected void broadcastAlertLevelChanged (AlertLevelType type)
	{
		NPCAlertLevelMessage message = new NPCAlertLevelMessage (gameObject, type);
		MessageCenter.Instance.Broadcast (message);
	}

	private void increaseAlertLevel (float sensitivity)
	{
		var playerSpeed = getPlayer ().rigidbody2D.velocity;
		alertLevel += playerSpeed.magnitude * detectLevel * sensitivity;
	}

	protected virtual void decrementAlertLevel ()
	{
		//float oldLevel = alertLevel;
		alertLevel -= alertDecrement;

		// Make sure alert level does not go below 0
		if (alertLevel < 0 || alertLevel == 0) 
		{
			broadcastAlertLevelChanged (AlertLevelType.Normal);
			alertTexture.renderer.enabled = false;
			alerted = false;
			alertLevel = 0;
			playAlert = true;
		}
	}

	protected virtual void alert ()
	{
		if (scared)
			return;

		alertTexture.renderer.enabled = true;
		alerted = true;
		if (playAlert)
		{
			AudioClip gasp = null;
			// Get skin type to know which NPC sounds to play
			if (this.SkinType.Equals (NPCSkinType.Bopper))
			{
				gasp = (AudioClip)bopperAlertSounds[Random.Range (0, bopperAlertSounds.Length)];
			}
			else if (this.SkinType.Equals (NPCSkinType.Hippie))
			{
				gasp = (AudioClip)hippieAlertSounds[Random.Range (0, hippieAlertSounds.Length)];
			}
			else if (this.SkinType.Equals (NPCSkinType.Hottie))
			{
				gasp = (AudioClip)hottieAlertSounds[Random.Range (0, hottieAlertSounds.Length)];
			}
			else if (this.SkinType.Equals (NPCSkinType.MowerMan))
			{
				gasp = (AudioClip)mowerAlertSounds[Random.Range (0, mowerAlertSounds.Length)];
			}
			else if (this.SkinType.Equals (NPCSkinType.OldMan))
			{
				gasp = (AudioClip)oldmanAlertSounds[Random.Range (0, oldmanAlertSounds.Length)];
			}

			audio.PlayOneShot (gasp, 1.0f);
			playAlert = false;
		}
		broadcastAlertLevelChanged (AlertLevelType.Alert);
		Vector3 direction = getPlayer ().transform.position - gameObject.transform.position;
		if (direction.x > 0)
			flipXScale (true);
		else
			flipXScale (false);
	}

	protected virtual void panic ()
	{
		alertTexture.renderer.enabled = false;
		panicTexture.renderer.enabled = true;
		scaredTexture.renderer.enabled = false;
		scared = false;

		AudioClip scream = null;
		// Get skin type to know which NPC sounds to play
		if (this.SkinType.Equals (NPCSkinType.Bopper))
		{
			scream = (AudioClip)bopperPanicSounds[Random.Range (0, bopperPanicSounds.Length)];
		}
		else if (this.SkinType.Equals (NPCSkinType.Hippie))
		{
			scream = (AudioClip)hippiePanicSounds[Random.Range (0, hippiePanicSounds.Length)];
		}
		else if (this.SkinType.Equals (NPCSkinType.Hottie))
		{
			scream = (AudioClip)hottiePanicSounds[Random.Range (0, hottiePanicSounds.Length)];
		}
		else if (this.SkinType.Equals (NPCSkinType.MowerMan))
		{
			scream = (AudioClip)mowerPanicSounds[Random.Range (0, mowerPanicSounds.Length)];
		}
		else if (this.SkinType.Equals (NPCSkinType.OldMan))
		{
		scream = (AudioClip)oldmanPanicSounds[Random.Range (0, oldmanPanicSounds.Length)];
		}
		
		audio.PlayOneShot (scream, 1.0f);

		speed = 1.5f;
		alerted = false;
		panicked = true;
		panickedPos = gameObject.transform.position;
		//panicTime = Time.time;
		//timePanicked = panicCooldownSeconds;
		moveDir = transform.position - getPlayer ().transform.position;
		broadcastAlertLevelChanged (AlertLevelType.Panic);

		if (GameObject.Find("AIGenerator").GetComponent<AIGenerator>().isMaze)
		{
			NPCPanickedOffMapMessage message = new NPCPanickedOffMapMessage (panickedPos);
			MessageCenter.Instance.Broadcast (message);
		}
	}

	protected virtual void scare (Vector3 scaredPosition)
	{
		if (panicked)
			return;
		// Play only if not scared so that sounds only plays once for a scare, and then can be played again when scare cooldown is up
		if (!scared)
		{
			// Play alert sound when NPC gets scared
			AudioClip gasp = null;
			// Get skin type to know which NPC sounds to play
			if (this.SkinType.Equals (NPCSkinType.Bopper))
			{
				gasp = (AudioClip)bopperAlertSounds[Random.Range (0, bopperAlertSounds.Length)];
			}
			else if (this.SkinType.Equals (NPCSkinType.Hippie))
			{
				gasp = (AudioClip)hippieAlertSounds[Random.Range (0, hippieAlertSounds.Length)];
			}
			else if (this.SkinType.Equals (NPCSkinType.Hottie))
			{
				gasp = (AudioClip)hottieAlertSounds[Random.Range (0, hottieAlertSounds.Length)];
			}
			else if (this.SkinType.Equals (NPCSkinType.MowerMan))
			{
				gasp = (AudioClip)mowerAlertSounds[Random.Range (0, mowerAlertSounds.Length)];
			}
			else if (this.SkinType.Equals (NPCSkinType.OldMan))
			{
				gasp = (AudioClip)oldmanAlertSounds[Random.Range (0, oldmanAlertSounds.Length)];
			}
			
			audio.PlayOneShot (gasp, 1.0f);
		}

		alertTexture.renderer.enabled = false;
		scaredTexture.renderer.enabled = true;

		scared = true;
		scaredTimeLeft = scaredCooldownSeconds;
		moveDir = transform.position - scaredPosition;
		broadcastAlertLevelChanged (AlertLevelType.Scared);

	}

	protected virtual void lure (Vector3 lurePosition)
	{
		if (panicked || alerted)
			return;

		//Debug.Log ("Becoming Lured: " + lurePosition);
		// Play only if not lured so that sounds only plays once for a lure, and then can be played again when lure cooldown is up
		if (!lured)
		{
			// Play curious sound for being lured
			AudioClip curious = null;
			// Get skin type to know which NPC sounds to play
			if (this.SkinType.Equals (NPCSkinType.Bopper))
			{
				curious = (AudioClip)bopperCuriousSounds[Random.Range (0, bopperCuriousSounds.Length)];
			}
			else if (this.SkinType.Equals (NPCSkinType.Hippie))
			{
				curious = (AudioClip)hippieCuriousSounds[Random.Range (0, hippieCuriousSounds.Length)];
			}
			else if (this.SkinType.Equals (NPCSkinType.Hottie))
			{
				curious = (AudioClip)hottieCuriousSounds[Random.Range (0, hottieCuriousSounds.Length)];
			}
			else if (this.SkinType.Equals (NPCSkinType.MowerMan))
			{
				curious = (AudioClip)mowerCuriousSounds[Random.Range (0, mowerCuriousSounds.Length)];
			}
			else if (this.SkinType.Equals (NPCSkinType.OldMan))
			{
				curious = (AudioClip)oldmanCuriousSounds[Random.Range (0, oldmanCuriousSounds.Length)];
			}
			
			audio.PlayOneShot (curious, 1.0f);
		}

		lured = true;
		nextPath = new GameObject ();
		nextPath.transform.position = lurePosition;
		nextPath.tag = lureTag;

		luredTimeLeft = lureCooldownSeconds;
	}

	protected bool checkForPlayer ()
	{
		if( getPlayer() == null )
			return false;
		Vector3 playerPos = getPlayer ().transform.position;

		// check if NPC can see that far
		if (Vector3.Distance (transform.position, playerPos) <= visionDistance) 
		{
			// get direction of player from NPC's point of view
			Vector3 direction = playerPos - transform.position;

			// get angle of direction 
			float angle = Vector3.Angle (direction, npcDir);

			if (angle <= visionAngleOffset)
				return true;
		}
		return false;
	}

	protected bool handleLured ()
	{
		if (Vector3.Distance (transform.position, nextPath.transform.position) <= 0.35) 
		{  
			// compute offset distance to avoid collisions
			if (luredTimeLeft == lureCooldownSeconds) 
			{
				Debug.Log ("Sitting");
				setAnimatorInteger (walkingKey, (int)WalkingDirection.STILL_DOWN_LEFT);
			}

			nextPath.transform.position = transform.position; // Set the nextpath to current path to stop moving
			luredTimeLeft -= Time.deltaTime;
			if (luredTimeLeft <= 0) 
			{
				lured = false;
				MessageCenter.Instance.Broadcast(new LureReleasedMessage(null, gameObject));
				if (nextPath.tag.Equals (lureTag)) 
				{
					Destroy (nextPath);
					Debug.Log ("Destroyed nextpath");
				} 
				else 
				{
					Debug.Log ("Didn't destroy: " + nextPath.tag);
				}
				nextPath = getNextPath (); // resume normal behaviour
			}

			return true;
		}

		return false;
	}

	protected void determineDirectionChange (Vector3 npcPosition, Vector3 newPosition)
	{
		// Translate the new position to compare against the origin (easier)
		Vector3 biasPosition = new Vector3 (newPosition.x - npcPosition.x, newPosition.y - npcPosition.y);
		// Compute the tangent of the new position to compare for angles
		float biasTan = (Mathf.Abs (biasPosition.y) / Mathf.Abs (biasPosition.x));

		if (biasPosition.x > 0) {
			lastDirectionWasRight = true;
			flipXScale (true);
		} else if (biasPosition.x < 0) {
			lastDirectionWasRight = false;
			flipXScale (false);
		} else if (lastDirectionWasRight) {
			flipXScale (true);
		} else {
			flipXScale (false);
		}
		if (biasPosition.x >= 0 && biasPosition.y >= 0) {
			// Quadrant 1
			testDirectionChange (biasTan, NPCDirection.R, NPCDirection.TR, NPCDirection.T);
		} else if (biasPosition.x < 0 && biasPosition.y >= 0) {
			// Quadrant 2
			testDirectionChange (biasTan, NPCDirection.L, NPCDirection.TL, NPCDirection.T);
		} else if (biasPosition.x < 0 && biasPosition.y < 0) {
			// Quadrant 3
			testDirectionChange (biasTan, NPCDirection.L, NPCDirection.BL, NPCDirection.B);
		} else {
			// Quadrant 4
			testDirectionChange (biasTan, NPCDirection.R, NPCDirection.BR, NPCDirection.B);
		}
	}

	protected void flipXScale (bool right)
	{
		float scaleByX = xScale;
		if (right)
			scaleByX = -xScale;

		transform.localScale = new Vector3 (scaleByX, transform.localScale.y, transform.localScale.z);
	}

	private void testDirectionChange (float biasTan, Vector3 low, Vector3 middle, Vector3 high)
	{
		float smallTan = Mathf.Tan (22.5f);
		float largeTan = Mathf.Tan (67.5f);

		if (biasTan < smallTan)
			setNPCDirection (low);
		else if (biasTan < largeTan)
			setNPCDirection (middle);
		else
			setNPCDirection (high);
	}

	protected void setNPCDirection (Vector3 direction)
	{
		npcDir = direction;

		WalkingDirection anim = WalkingDirection.STILL_DOWN_LEFT;
		/*
		 * Old
		switch (npcDir)
		{
		case NPCDirection.L:
		case NPCDirection.BL:
		case NPCDirection.B:
			anim = WalkingDirection.MOVING_DOWN_LEFT;
			break;
		case NPCDirection.R:
		case NPCDirection.BR:
			anim = WalkingDirection.MOVING_DOWN_RIGHT;
			break;
		case Vector3.zero:
			anim = WalkingDirection.STILL_DOWN_LEFT;
		}
		*/

		// Testing
		if (npcDir.Equals(NPCDirection.TL) 
		    || npcDir.Equals(NPCDirection.T) 
		    || npcDir.Equals(NPCDirection.TR))
		{
			anim = WalkingDirection.MOVING_UP_LEFT;
		}
		else if (npcDir.Equals(NPCDirection.R)
		         || npcDir.Equals(NPCDirection.BR)
		         || npcDir.Equals(NPCDirection.BL)
		         || npcDir.Equals(NPCDirection.L)
		         || npcDir.Equals(NPCDirection.B)
		         )
		{
			anim = WalkingDirection.MOVING_DOWN_LEFT;
		}

		/*
		 * New
		if (npcDir.Equals(NPCDirection.TL))
		{
			anim = WalkingDirection.MOVING_UP_LEFT;
		}
		else if (npcDir.Equals(NPCDirection.T) || npcDir.Equals(NPCDirection.TR))
		{
			anim = WalkingDirection.MOVING_UP_RIGHT;
		}
		setAnimatorInteger ((int)anim);
		*/

	}

	protected GameObject getLeavingPath ()
	{
		GameObject[] spawnPoints = GameObject.FindGameObjectsWithTag (spawnTag);
		float minDistance = 0f;
		int retIndex = 0;
		if (spawnPoints.Length == 1)
			return spawnPoints [0];
		for (int i = 0; i < spawnPoints.Length; i++) {
			Vector3 pathPointPos = spawnPoints [i].transform.position;
			Vector3 npcPos = gameObject.transform.position;
			float distance = Vector3.Distance (pathPointPos, npcPos);
			if (minDistance == 0f)
					minDistance = distance;
			if (distance <= minDistance) {
					minDistance = distance;
					retIndex = i;
			}
		}
		return spawnPoints [retIndex];
		//int rand = Random.Range(0, spawnPoints.Length);
		//return spawnPoints[rand];
	}

	protected virtual GameObject getNextPath ()
	{
		GameObject path = new GameObject ();
		return path;
	}

	protected GameObject getPlayer ()
	{
		GameObject[] trees = GameObject.FindGameObjectsWithTag ("Player");
		foreach (GameObject player in trees) {
			if (player.GetComponent<PossessableTree> ().Active) {
				return player;
			}
		}

		if (trees.Length > 0) {
			return trees [0];
		} else {
			return null;
		}
	}

	//-----------------------
	// Accessor Methods
	//-----------------------

	// needed for scoring
	public Lure getLastLure ()
	{
		return lastLure;
	}

	//-----------------------
	// Listener Methods
	//-----------------------

	void grabbedListener (Message message)
	{
		if (((PlayerGrabbedNPCsMessage)message).NPCs.Contains (gameObject)) { 
			alerted = false;
			scared = false;

			grabbed = true; 

			alertTexture.renderer.enabled = false;
			panicTexture.renderer.enabled = false;
			scaredTexture.renderer.enabled = false;
		}
	}

	void eatenListener (Message message)
	{
		NPCEatenMessage msg = message as NPCEatenMessage;
		
		if( msg.NPC == null )
			return;
		if (!(msg.NPC.Equals(gameObject)))
			return;

		// Play eaten sound
		AudioClip eaten = null;
		// Get skin type to know which NPC sounds to play
		if (this.SkinType.Equals (NPCSkinType.Bopper))
		{
			eaten = bopperEaten;
		}
		else if (this.SkinType.Equals (NPCSkinType.Hippie))
		{
			eaten = hippieEaten;
		}
		else if (this.SkinType.Equals (NPCSkinType.Hottie))
		{
			eaten = hottieEaten;
		}
		else if (this.SkinType.Equals (NPCSkinType.MowerMan))
		{
			eaten = mowerEaten;
		}
		else if (this.SkinType.Equals (NPCSkinType.OldMan))
		{
			eaten = oldmanEaten;
		}
		
		audio.PlayOneShot (eaten, 1.0f);

	}

	void releasedListener (Message message)
	{
		if (((PlayerReleasedNPCsMessage)message).NPCs.Contains (gameObject)) {
			grabbed = false;
			panic ();
		}
	}

	void trapEnterListener (Message message)
	{
		GameObject trappedNPC = ((TrapEnteredMessage)message).NPC;
		if (trappedNPC.Equals (gameObject)) {
			grabbed = true;
			alertTexture.renderer.enabled = false;
			panicTexture.renderer.enabled = false;
		}
	}

	void trapReleaseListener (Message message)
	{
		if (((TrapEnteredMessage)message).NPC.Equals (gameObject)) {
			grabbed = false;
			// TODO: set alert level to panic!
		}
	}

	void lureEnterListener (Message message)
	{
		if (true)
			return;

		LureEnteredMessage lureMessage = message as LureEnteredMessage;
		if (lureMessage.NPC.Equals (gameObject)) {
			if (lastLure != null && lastLure.Equals (lureMessage.Lure)) {
				return;
			}

			if (lureMessage.Lure.lurePower >= lurePower) {
				lured = true;
				nextPath = lureMessage.Lure.gameObject;
			}
		}
	}

	void lureReleaseListener (Message message)
	{
		if (true)
			return;
		LureReleasedMessage lureMessage = message as LureReleasedMessage;
		if (lureMessage.NPC.Equals (gameObject)) {
			lured = false;
			lastLure = lureMessage.Lure;
			//TODO: make getNextPath better
			nextPath = getNextPath ();
		}
	}

	void abilityPlacedListener (Message message)
	{
		AbilityPlacedMessage placedMessage = message as AbilityPlacedMessage;
		Vector3 possessedPosition = new Vector3 (placedMessage.X, placedMessage.Y);
		float radius = GetComponent<CircleCollider2D> ().radius;
		if (placedMessage.AType.Equals (AbilityType.PossessionScare)) {
			if (Vector3.Distance (transform.position, possessedPosition) <= radius)
				scare (possessedPosition);
		} else if (placedMessage.AType.Equals (AbilityType.PossessionLure)) {
			if (Vector3.Distance (transform.position, possessedPosition) <= radius)
			{
				lure (possessedPosition);
				MessageCenter.Instance.Broadcast(new LureEnteredMessage(null,gameObject));
			}
		}
	}

	void markedBountyListener (Message message)
	{
		NewMarkedBountyMessage markedMessage = message as NewMarkedBountyMessage;
		if (markedMessage.NPC.Equals(this.gameObject))
		{
			marked = true;
		}
	}

	// --------------------
	// Avoid
	// --------------------

	protected Vector3 avoid (Vector3 currentNPCDirection)
	{
		// ignore critters
		if (transform.gameObject.GetComponent<CritterController> () != null) 
		{
			avoidCurrentDirection = Vector3.zero;
			return Vector3.zero;
		}

		//return Vector3.zero; // uncomment if testing without object avoidance
		if( avoidCounter < 25 )
		{
			avoidCounter++;
			return avoidCurrentDirection;
		}
		else
		{
			avoidCounter = 0;
		}
		
		float checkDistance = 1f;
		Vector3 direction = currentNPCDirection;
		// get width/height
		float radius = (float)Mathf.Max (gameObject.GetComponent<BoxCollider2D> ().size.x, gameObject.GetComponent<BoxCollider2D> ().size.y) / 2f;

		// set layer to disregard (NPCS)
		LayerMask layermask = ~(1 << 8);

		// get offset positions
		Vector3 leftPos = transform.position + Quaternion.AngleAxis (90, new Vector3 (0, 0, 1)) * direction * radius;
		Vector3 rightPos = transform.position - Quaternion.AngleAxis (90, new Vector3 (0, 0, 1)) * direction * radius;

		RaycastHit2D hit;
		// check for object in the way
		if (hit = Physics2D.Raycast (rightPos, direction, checkDistance, layermask)) // right raycast
		{
			//RaycastHit2D hit = Physics2D.CircleCast (transform.position, radius, direction, checkDistance, layermask);
			
			// ignore self
			if( hit.transform != transform )
			{
				if (hit.transform.gameObject == nextPath)
				{
					avoidCurrentDirection = Vector3.zero;
					return Vector3.zero;
				}
				
				if (hit.transform.gameObject.GetComponent<Rigidbody2D> () == null 
				    && hit.transform.gameObject.GetComponent<EdgeCollider2D> () == null
				    && hit.transform.gameObject.GetComponent<BoxCollider2D> () == null) // hit is invalid
				{
					avoidCurrentDirection = Vector3.zero;
					return Vector3.zero;
				}
				
				if (hit.transform.gameObject.tag == "NPC" ||
				    hit.transform.gameObject.tag == "PossessorTrigger" ||
				    hit.transform.gameObject.tag == "Border") // also invalid
				{
					avoidCurrentDirection = Vector3.zero;
					return Vector3.zero;
				}
				
				// VALID HIT!!!
				
				// if object is on top of next path location
				if (Vector3.Distance (hit.transform.position, nextPath.transform.position) < .5f && hit.transform.gameObject != nextPath.transform.gameObject) 
				{
					if (transform.gameObject.GetComponent<PathAIController> () != null) 
					{
						PathAIController script = transform.gameObject.GetComponent<PathAIController> ();
						nextPath = script.movePath.getNextPath (nextPath, gameObject); // go to next 
						
						avoidCurrentDirection = Vector3.zero;
						return Vector3.zero;
					}
					else if (transform.gameObject.GetComponent<StationaryAIController> () != null) 
					{
						nextPath = getLeavingPath ();
						
						avoidCurrentDirection = Vector3.zero;
						return Vector3.zero;
					}
					else if (transform.gameObject.GetComponent<EnemyAIController> () != null) 
					{
						avoidCurrentDirection = Vector3.zero;
						return Vector3.zero;
					}
				}
				
				// avoid object
				Vector3 newPos;
				
				// go left
				Vector3 leftDir = Quaternion.AngleAxis (45, new Vector3 (0, 0, 1)) * direction;
				
				if (Physics2D.Raycast (transform.position, leftDir, .2f, layermask)) {
					leftDir = Quaternion.AngleAxis (90, new Vector3 (0, 0, 1)) * direction;
				}

				newPos = transform.position + 5 * leftDir;

				avoidCurrentDirection = newPos;
				return newPos;
			}
			else
			{
				avoidCurrentDirection = Vector3.zero;
				return Vector3.zero;
			}
		}
		else if (hit = Physics2D.Raycast (leftPos, direction, checkDistance, layermask)) // left raycast
		{
			//RaycastHit2D hit = Physics2D.CircleCast (transform.position, radius, direction, checkDistance, layermask);
			
			// ignore self
			if( hit.transform != transform )
			{
				if (hit.transform.gameObject == nextPath)
				{
					avoidCurrentDirection = Vector3.zero;
					return Vector3.zero;
				}
				
				if (hit.transform.gameObject.GetComponent<Rigidbody2D> () == null 
				    && hit.transform.gameObject.GetComponent<EdgeCollider2D> () == null
				    && hit.transform.gameObject.GetComponent<BoxCollider2D> () == null) // hit is invalid
				{
					avoidCurrentDirection = Vector3.zero;
					return Vector3.zero;
				}
				
				if (hit.transform.gameObject.tag == "NPC" ||
				    hit.transform.gameObject.tag == "Possessor" ||
				    hit.transform.gameObject.tag == "Border") // also invalid
				{
					avoidCurrentDirection = Vector3.zero;
					return Vector3.zero;
				}
				
				// VALID HIT!!!
				
				// if object is on top of next path location
				if (Vector3.Distance (hit.transform.position, nextPath.transform.position) < .5f && hit.transform.gameObject != nextPath.transform.gameObject) 
				{
					if (transform.gameObject.GetComponent<PathAIController> () != null) 
					{
						PathAIController script = transform.gameObject.GetComponent<PathAIController> ();
						nextPath = script.movePath.getNextPath (nextPath, gameObject); // go to next 
						
						avoidCurrentDirection = Vector3.zero;
						return Vector3.zero;
					}
					else if (transform.gameObject.GetComponent<StationaryAIController> () != null) 
					{
						nextPath = getLeavingPath ();
						
						avoidCurrentDirection = Vector3.zero;
						return Vector3.zero;
					}
					else if (transform.gameObject.GetComponent<EnemyAIController> () != null) 
					{
						avoidCurrentDirection = Vector3.zero;
						return Vector3.zero;
					}
				}
				
				// avoid object
				Vector3 newPos;
				
				// go left for now
				Vector3 rightDir = Quaternion.AngleAxis (-45, new Vector3 (0, 0, 1)) * direction;
				
				if (Physics2D.Raycast (transform.position, rightDir, .2f, layermask)) {
					rightDir = Quaternion.AngleAxis (90, new Vector3 (0, 0, 1)) * direction;
				}

				newPos = transform.position + 5 * rightDir;

				avoidCurrentDirection = newPos;
				return newPos;
			}
			else
			{
				avoidCurrentDirection = Vector3.zero;
				return Vector3.zero;
			}
		}
		else if (hit = Physics2D.Raycast (transform.position, direction, checkDistance, layermask)) // center raycast
		{
			//RaycastHit2D hit = Physics2D.CircleCast (transform.position, radius, direction, checkDistance, layermask);

			// ignore self
			if( hit.transform != transform )
			{
				if (hit.transform.gameObject == nextPath)
				{
					avoidCurrentDirection = Vector3.zero;
					return Vector3.zero;
				}
				
				if (hit.transform.gameObject.GetComponent<Rigidbody2D> () == null 
				    && hit.transform.gameObject.GetComponent<EdgeCollider2D> () == null
				    && hit.transform.gameObject.GetComponent<BoxCollider2D> () == null) // hit is invalid
				{
					avoidCurrentDirection = Vector3.zero;
					return Vector3.zero;
				}
				
				if (hit.transform.gameObject.tag == "NPC" ||
				    hit.transform.gameObject.tag == "Possessor" ||
				    hit.transform.gameObject.tag == "Border") // also invalid
				{
					avoidCurrentDirection = Vector3.zero;
					return Vector3.zero;
				}
				
				// VALID HIT!!!
				
				// if object is on top of next path location
				if (Vector3.Distance (hit.transform.position, nextPath.transform.position) < .5f && hit.transform.gameObject != nextPath.transform.gameObject) 
				{
					if (transform.gameObject.GetComponent<PathAIController> () != null) 
					{
						PathAIController script = transform.gameObject.GetComponent<PathAIController> ();
						nextPath = script.movePath.getNextPath (nextPath, gameObject); // go to next 
						
						avoidCurrentDirection = Vector3.zero;
						return Vector3.zero;
					}
					else if (transform.gameObject.GetComponent<StationaryAIController> () != null) 
					{
						nextPath = getLeavingPath ();
						
						avoidCurrentDirection = Vector3.zero;
						return Vector3.zero;
					}
					else if (transform.gameObject.GetComponent<EnemyAIController> () != null) 
					{
						avoidCurrentDirection = Vector3.zero;
						return Vector3.zero;
					}
				}
				
				// avoid object
				Vector3 newPos;
				
				// go left for now
				Vector3 rightDir = Quaternion.AngleAxis (-45, new Vector3 (0, 0, 1)) * direction;
				Vector3 leftDir = Quaternion.AngleAxis (45, new Vector3 (0, 0, 1)) * direction;
				
				if (Physics2D.Raycast (transform.position, leftDir, .2f, layermask)) {
					leftDir = Quaternion.AngleAxis (90, new Vector3 (0, 0, 1)) * direction;
				}
				
				if (Physics2D.Raycast (transform.position, leftDir, .2f, layermask)) {
					newPos = transform.position + 5 * rightDir;
				} else {
					newPos = transform.position + 5 * leftDir;
				}
				avoidCurrentDirection = newPos;
				
				return newPos;
			}
			else
			{
				avoidCurrentDirection = Vector3.zero;
				return Vector3.zero;
			}
		}
		else
		{
			avoidCurrentDirection = Vector3.zero;
			return Vector3.zero;
		}
	}
}