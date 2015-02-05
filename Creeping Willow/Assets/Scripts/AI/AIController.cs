using UnityEngine;
using System.Collections;
//using UnityEditor;

public class AIController : GameBehavior {

	public class NPCDirection
	{
		public static Vector3 T = new Vector3(0,1),
		TL = new Vector3(-Mathf.Sqrt(2)/2, Mathf.Sqrt (2)/2),
		TR = new Vector3(Mathf.Sqrt(2)/2, Mathf.Sqrt(2)/2),
		B = new Vector3(0, -1),
		BL = new Vector3(-Mathf.Sqrt(2)/2, -Mathf.Sqrt (2)/2),
		BR = new Vector3(Mathf.Sqrt(2)/2, -Mathf.Sqrt (2)/2),
		L = new Vector3(-1,0),
		R = new Vector3(0,1);
	}

	// Tags
	public string spawnTag = "Respawn";
	public string npcTag = "NPC";

    // Skin Type
    public NPCSkinType SkinType;

	// Global Keys
	protected string walkingKey = "direction";
	protected enum WalkingDirection
	{
		STILL = 0,
		MOVING_DOWN = 1,
		MOVING_UP = 2,
	}

	// NPC variables
	//public float nourishment = 1;			// Nourishment for player goal (Not currently Implemented)
	public float lurePower = 3;
	public float speed = 1;					// NPC speed (when not running)
	public float scaredCooldownSeconds = 6;	// How long the NPC will be scared for
	public float panicCooldownSeconds = 2;	// How long the NPC will be panicked for
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

	// Panic variables
    protected float panicThreshold = 10;
	protected Vector3 panickedPos;
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
	public void Start ()
	{
		enteredMap = false;
		// Alert Texture
		alertTexture = (GameObject)Instantiate(Resources.Load("prefabs/AI/SceneInitPrefabs/AIAlert"));
		alertTexture.renderer.enabled = false;
		TextureScript alertTs = alertTexture.GetComponent<TextureScript> ();
		alertTs.target = gameObject;

		// Panic Texture
		panicTexture = (GameObject)Instantiate (Resources.Load ("prefabs/AI/SceneInitPrefabs/AIPanic"));
		panicTexture.renderer.enabled = false;
		TextureScript panicTs = panicTexture.GetComponent<TextureScript> ();
		panicTs.target = gameObject;

		// Scare Texture
		scaredTexture = (GameObject)Instantiate (Resources.Load ("prefabs/AI/SceneInitPrefabs/AIScared"));
		scaredTexture.renderer.enabled = false;
		TextureScript scaredTs = scaredTexture.GetComponent<TextureScript> ();
		scaredTs.target = gameObject;

		player = GameObject.FindGameObjectWithTag("Player");
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
		MessageCenter.Instance.RegisterListener (MessageType.PlayerReleasedNPCs, releasedListener);
		MessageCenter.Instance.RegisterListener (MessageType.TrapEntered, trapEnterListener);
		MessageCenter.Instance.RegisterListener (MessageType.TrapReleased, trapReleaseListener);
		MessageCenter.Instance.RegisterListener (MessageType.LureRadiusEntered, lureEnterListener);
		MessageCenter.Instance.RegisterListener (MessageType.LureReleased, lureReleaseListener);
		MessageCenter.Instance.RegisterListener (MessageType.AbilityPlaced, abilityPlacedListener);

		// Ignore collision with other AI
		int npcLayer = LayerMask.NameToLayer (npcTag);
		Physics2D.IgnoreLayerCollision (npcLayer, npcLayer);

		
		// Broadcast start of lifecycle
		MessageCenter.Instance.Broadcast (new NPCCreatedMessage (gameObject));
	}

	void OnDestroy()
	{
		// Unregister for all messages that were previously registered for
		MessageCenter.Instance.UnregisterListener (MessageType.PlayerGrabbedNPCs, grabbedListener);
		MessageCenter.Instance.UnregisterListener (MessageType.PlayerReleasedNPCs, releasedListener);
		MessageCenter.Instance.UnregisterListener (MessageType.TrapEntered, trapEnterListener);
		MessageCenter.Instance.UnregisterListener (MessageType.TrapReleased, trapReleaseListener);
		MessageCenter.Instance.UnregisterListener (MessageType.LureRadiusEntered, lureEnterListener);
		MessageCenter.Instance.UnregisterListener (MessageType.LureReleased, lureReleaseListener);
		MessageCenter.Instance.UnregisterListener (MessageType.AbilityPlaced, abilityPlacedListener);

		if (alertTexture != null)
		{
			Destroy(alertTexture.gameObject);
		}
		if (panicTexture != null)
		{
			Destroy (panicTexture.gameObject);
		}
		if (scaredTexture != null)
			Destroy (scaredTexture.gameObject);

		NPCDestroyedMessage message = new NPCDestroyedMessage (gameObject, false);
		MessageCenter.Instance.Broadcast (message);

		NPCOnDestroy();
	}

	protected virtual void NPCOnDestroy()
	{

	}
	//-----------------------
	// Trigger Methods
	//-----------------------

    protected virtual void OnTriggerExit2D(Collider2D other)
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
				destroyNPC();
			}
			enteredMap = true;
			ignoreBorder (false, other);
		}
    }

	protected virtual void OnTriggerEnter2D(Collider2D other)
	{
		if (other.tag == "Player") 
		{
			playerInRange = true;
			TreeController script = other.gameObject.GetComponent<TreeController>();
			if (script != null && script.state != Tree.State.Normal)
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

	protected virtual void OnTriggerStay2D(Collider2D other)
    {

        if (other.tag == "Wall")
        {
            RaycastHit2D raycast = Physics2D.Raycast(transform.position, moveDir);
            if (raycast.collider != null && raycast.collider.tag == "Wall")
            {
                float distance = Vector2.Distance(transform.position, raycast.point);
                if (distance < 1.5f)
                    nearWall = true;
                else
                    nearWall = false;
            }
        }
        else if (other.tag == "Player")
        {
            if (panicked)
            {
                //timePanicked = panicCooldownSeconds;
                return;
            }

            if (alertLevel >= alertThreshold)
            {
                //Debug.Log("ALERTED");
				alert();
            }

			if (alertLevel >= panicThreshold)
            {
                //Debug.Log("PANICKED");
				panic();
            }

            // Increment alertLevel
			increaseAlertLevel(hearingAlertMultiplier);
        }
    }
	
	protected bool updateNPC()
	{
		if (grabbed)
			return true;
		
		if (playerInRange)
		{
			Vector2 playerSpeed = getPlayer().rigidbody2D.velocity;
			if (playerSpeed == Vector2.zero && alertLevel > 0)
			{
				decrementAlertLevel();
			}
		}
		else if (alertLevel > 0)
		{
			decrementAlertLevel();
		}
		
		if (alerted)
			return true;
		
		if (panicked)
		{
			/*timePanicked -= Time.deltaTime;
			if (timePanicked <= 0)
			{
				panicked = false;
				panicTexture.renderer.enabled = false;
				speed = 1;
				alertLevel = alertThreshold - 0.1f;
				broadcastAlertLevelChanged(AlertLevelType.Normal);
				return true;
			}*/

			if (nearWall)
			{
				nearWall = false;
				moveDir = Quaternion.AngleAxis(90, transform.forward) * -moveDir;
				
			}

			Vector3 npcPosition = transform.position;
			float step = speed * Time.deltaTime;
			Vector3 movement = Vector3.MoveTowards (npcPosition, new Vector3(moveDir.normalized.x * step, moveDir.normalized.y * step) + npcPosition, step);
			//transform.position = new Vector3(moveDir.normalized.x * step, moveDir.normalized.y * step) + npcPosition;
			determineDirectionChange(npcPosition, transform.position);

			Vector3 direction = Vector3.Normalize(movement - transform.position);
			Vector3 changeMovement = avoid (direction);
			
			if( changeMovement != Vector3.zero )
			{
				Vector3 newPos = Vector3.MoveTowards(npcPosition,changeMovement,step);
				transform.position = newPos;
				determineDirectionChange (transform.position, newPos);
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
		if (checkForPlayer() && player.rigidbody2D != null && player.rigidbody2D.velocity != Vector2.zero)
		{
			if (NPCHandleSeeingPlayer())
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
			transform.position = new Vector3(moveDir.normalized.x * step, moveDir.normalized.y * step) + npcPosition;
			determineDirectionChange(npcPosition, transform.position);

			return true;
		}

		return false;
	}

	//-----------------------
	// Helper Methods
	//-----------------------

	virtual protected bool NPCHandleSeeingPlayer() 
	{
		// TODO: Balance better
		increaseAlertLevel(sightAlertMultiplier);
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
	
	protected void destroyNPC()
	{
		Destroy (gameObject);		
	}

	protected void ignoreBorder(bool ignore, Collider2D other)
	{
		BoxCollider2D box = gameObject.GetComponent<BoxCollider2D>();
		Physics2D.IgnoreCollision(other, box, ignore);
	}

	protected void setAnimatorInteger(string key, int animation)
	{
		Animator anim = gameObject.GetComponent<Animator> ();
		if (anim != null)
		{
			gameObject.GetComponent<Animator>().SetInteger(key, animation);
		}
	}

	protected void broadcastAlertLevelChanged(AlertLevelType type)
	{
		NPCAlertLevelMessage message = new NPCAlertLevelMessage (gameObject, type);
		MessageCenter.Instance.Broadcast (message);
	}
	
	private void increaseAlertLevel(float sensitivity)
	{
		var playerSpeed = getPlayer().rigidbody2D.velocity;
		alertLevel += playerSpeed.magnitude * detectLevel * sensitivity;
	}
	
	protected virtual void decrementAlertLevel()
	{
		//float oldLevel = alertLevel;
		alertLevel -= alertDecrement;
		
		// Make sure alert level does not go below 0
		if (alertLevel < 0 || alertLevel == 0)
		{
			broadcastAlertLevelChanged(AlertLevelType.Normal);
			alertTexture.renderer.enabled = false;
			alerted = false;
			alertLevel = 0;
		}
	}
	
	protected virtual void alert()
	{
		if (scared)
			return;

		alertTexture.renderer.enabled = true;
		alerted = true;
		broadcastAlertLevelChanged(AlertLevelType.Alert);
		Vector3 direction = getPlayer ().transform.position - gameObject.transform.position;
		if (direction.x > 0)
			flipXScale (true);
		else
			flipXScale (false);
	}
	
	protected virtual void panic()
	{
		alertTexture.renderer.enabled = false;
		panicTexture.renderer.enabled = true;
		scaredTexture.renderer.enabled = false;
		scared = false;

		speed = 1.5f;
		alerted = false;
		panicked = true;
		panickedPos = gameObject.transform.position;
		//panicTime = Time.time;
		//timePanicked = panicCooldownSeconds;
		moveDir = transform.position - getPlayer().transform.position;
		broadcastAlertLevelChanged(AlertLevelType.Panic);
	}

	protected virtual void scare(Vector3 scaredPosition)
	{
		if (panicked)
			return;

		alertTexture.renderer.enabled = false;
		scaredTexture.renderer.enabled = true;

		scared = true;
		scaredTimeLeft = scaredCooldownSeconds;
		moveDir = transform.position - scaredPosition;
		broadcastAlertLevelChanged (AlertLevelType.Scared);
	}

	protected bool checkForPlayer()
	{
		Vector3 playerPos = getPlayer().transform.position;
		
		// check if NPC can see that far
		if( Vector3.Distance(transform.position, playerPos) <= visionDistance )
		{
			// get direction of player from NPC's point of view
			Vector3 direction = playerPos - transform.position;

			// get angle of direction 
			float angle = Vector3.Angle(direction, npcDir);

			if( angle <= visionAngleOffset )
				return true;
		}
		return false;
	}

	protected void determineDirectionChange(Vector3 npcPosition, Vector3 newPosition)
	{
		// Translate the new position to compare against the origin (easier)
		Vector3 biasPosition = new Vector3 (newPosition.x - npcPosition.x, newPosition.y - npcPosition.y);
		// Compute the tangent of the new position to compare for angles
		float biasTan = (Mathf.Abs(biasPosition.y) / Mathf.Abs (biasPosition.x));

		if (biasPosition.x > 0)
		{
			lastDirectionWasRight = true;
			flipXScale(true);
		}
		else if (biasPosition.x < 0)
		{
			lastDirectionWasRight = false;
			flipXScale(false);
		}
		else if (lastDirectionWasRight)
		{
			flipXScale(true);
		}
		else
		{
			flipXScale(false);
		}
		if (biasPosition.x >= 0 && biasPosition.y >= 0)
		{
			// Quadrant 1
			testDirectionChange(biasTan, NPCDirection.R, NPCDirection.TR, NPCDirection.T);
		}
		else if (biasPosition.x < 0 && biasPosition.y >= 0)
		{
			// Quadrant 2
			testDirectionChange(biasTan, NPCDirection.L, NPCDirection.TL, NPCDirection.T);
		}
		else if (biasPosition.x < 0 && biasPosition.y < 0)
		{
			// Quadrant 3
			testDirectionChange(biasTan, NPCDirection.L, NPCDirection.BL, NPCDirection.B);
		}
		else
		{
			// Quadrant 4
			testDirectionChange(biasTan, NPCDirection.R, NPCDirection.BR, NPCDirection.B);
		}
	}

	protected void flipXScale(bool right)
	{
		float scaleByX = xScale;
		if (right)
			scaleByX = -xScale;

		transform.localScale = new Vector3 (scaleByX, transform.localScale.y, transform.localScale.z);
	}

	private void testDirectionChange(float biasTan, Vector3 low, Vector3 middle, Vector3 high)
	{
		float smallTan = Mathf.Tan (22.5f);
		float largeTan = Mathf.Tan (67.5f);

		if (biasTan < smallTan)
			setNPCDirection(low);
		else if (biasTan < largeTan)
			setNPCDirection(middle);
		else
			setNPCDirection(high);
	}
	
	protected void setNPCDirection(Vector3 direction)
	{
		npcDir = direction;
	}

	protected GameObject getLeavingPath()
	{
		GameObject[] spawnPoints = GameObject.FindGameObjectsWithTag (spawnTag);
		float minDistance = 0f;
		int retIndex = 0;
		if (spawnPoints.Length == 1)
			return spawnPoints[0];
		for (int i = 0; i < spawnPoints.Length; i++)
		{
			Vector3 pathPointPos = spawnPoints[i].transform.position;
			Vector3 npcPos = gameObject.transform.position;
			float distance = Vector3.Distance(pathPointPos, npcPos);
			if (minDistance == 0f)
				minDistance = distance;
			if (distance <= minDistance)
			{
				minDistance = distance;
				retIndex = i;
			}
		}
		return spawnPoints [retIndex];
		//int rand = Random.Range(0, spawnPoints.Length);
		//return spawnPoints[rand];
	}
	
	protected virtual GameObject getNextPath()
	{
		GameObject path = new GameObject ();
		return path;
	}

	protected GameObject getPlayer()
	{
		GameObject[] trees = GameObject.FindGameObjectsWithTag("Player");
		foreach (GameObject player in trees)
		{
			if (player.GetComponent<PossessableTree>().Active)
			{
				return player;
			}
		}

		if (trees.Length > 0)
		{
			return trees[0];
		}
		else
		{
			return null;
		}
	}

	//-----------------------
	// Accessor Methods
	//-----------------------

	// needed for scoring
	public Lure getLastLure()
	{
		return lastLure;
	}

	//-----------------------
	// Listener Methods
	//-----------------------

	void grabbedListener(Message message)
	{
        if (((PlayerGrabbedNPCsMessage)message).NPCs.Contains(gameObject))
        { 
			alerted = false;
			scared = false;

			grabbed = true; 

			alertTexture.renderer.enabled = false;
			panicTexture.renderer.enabled = false;
			scaredTexture.renderer.enabled = false;
		}
	}

	void releasedListener(Message message)
	{
		if (((PlayerReleasedNPCsMessage)message).NPCs.Contains(gameObject))
		{
			grabbed = false;
			panic();
		}
	}

	void trapEnterListener(Message message)
	{
		GameObject trappedNPC = ((TrapEnteredMessage)message).NPC;
		if (trappedNPC.Equals(gameObject))
		{
			grabbed = true;
			alertTexture.renderer.enabled = false;
			panicTexture.renderer.enabled = false;
		}
	}

	void trapReleaseListener(Message message)
	{
		if (((TrapEnteredMessage)message).NPC.Equals(gameObject))
		{
			grabbed = false;
			// TODO: set alert level to panic!
		}
	}

	void lureEnterListener(Message message)
	{
//		if (true)
//			return;

		LureEnteredMessage lureMessage = message as LureEnteredMessage;
		if (lureMessage.NPC.Equals(gameObject))
		{
			if (lastLure != null && lastLure.Equals(lureMessage.Lure))
			{
				return;
			}

			if (lureMessage.Lure.lurePower >= lurePower)
			{
				lured = true;
				nextPath = lureMessage.Lure.gameObject;
			}
		}
	}

	void lureReleaseListener(Message message)
	{
//		if (true)
//			return;
		LureReleasedMessage lureMessage = message as LureReleasedMessage;
		if (lureMessage.NPC.Equals(gameObject))
		{
			lured = false;
			lastLure = lureMessage.Lure;
			//TODO: make getNextPath better
			nextPath = getNextPath();
		}
	}

	void abilityPlacedListener(Message message)
	{
		AbilityPlacedMessage placedMessage = message as AbilityPlacedMessage;
		if (placedMessage.AType.Equals(AbilityType.PossessionLure))
	    {
			Vector3 possessedPosition = new Vector3(placedMessage.X, placedMessage.Y);
			if (Vector3.Distance(transform.position, possessedPosition) <= GetComponent<CircleCollider2D>().radius)
				scare(possessedPosition);
		}
	}


	protected Vector3 avoid(Vector3 currentNPCDirection)
	{
		float checkDistance = 1f;
		Vector3 direction = currentNPCDirection;
		// get width/height
		float radius = (float)Mathf.Max (gameObject.GetComponent<BoxCollider2D> ().size.x, gameObject.GetComponent<BoxCollider2D> ().size.y) / 2f;

		// check for object in the way
		if( Physics2D.CircleCast (transform.position, radius, direction,checkDistance) )
		{
			RaycastHit2D hit = Physics2D.CircleCast (transform.position, radius, direction);
			
			if( hit == null )
				return Vector3.zero;
			
			if( hit.transform.gameObject == nextPath )
				return Vector3.zero;
			
			if( hit.transform.gameObject.GetComponent<Rigidbody2D>() == null 
			   && hit.transform.gameObject.GetComponent<EdgeCollider2D>() == null
			   && hit.transform.gameObject.GetComponent<BoxCollider2D>() == null ) // hit is invalid
				return Vector3.zero;
			
			if( hit.transform.gameObject.tag == "NPC" ||
			   hit.transform.gameObject.tag == "Border" ) // also invalid
				return Vector3.zero;
			
			// VALID HIT!!!

			//Debug.Log ("in cast");

			// if object is on top of next path location
			if( Vector3.Distance(hit.transform.position,nextPath.transform.position) < .3f && hit.transform.gameObject != nextPath.transform.gameObject )
			{
				if( transform.gameObject.GetComponent<PathAIController>() != null )
				{
					PathAIController script = transform.gameObject.GetComponent<PathAIController>();
					nextPath = script.movePath.getNextPath(nextPath,gameObject); // go to next 
					
					return Vector3.zero;
				}
				else if( transform.gameObject.GetComponent<StationaryAIController>() != null )
				{
					nextPath = getLeavingPath();
					
					return Vector3.zero;
				}
			}

			// avoid object
			Vector3 newPos;

			// go left for now
			Vector3 rightDir = Quaternion.AngleAxis(-45, new Vector3(0,0,1)) * direction;
			Vector3 leftDir = Quaternion.AngleAxis(45, new Vector3(0,0,1)) * direction;

			if( Physics2D.CircleCast (transform.position, radius, leftDir,.2f) )
			{
				leftDir = Quaternion.AngleAxis(90, new Vector3(0,0,1)) * direction;
			}

			if( Physics2D.CircleCast (transform.position, radius, leftDir,.2f) )
			{
				newPos = transform.position + 5*rightDir;
			}
			else
			{
				newPos = transform.position + 5*leftDir;
			}

			return newPos;
		}

		return Vector3.zero;

	}
}



