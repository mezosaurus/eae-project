using UnityEngine;
using System.Collections;

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

	// NPC variables
	//public float nourishment = 1;			// Nourishment for player goal (Not currently Implemented)
	public float lurePower = 3;
	public float speed = 1;					// NPC speed (when not running)
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
	public bool lured;
	protected bool playerInRange;
    protected bool nearWall;
	protected bool killSelf = false;

	// Scene variables
	protected Lure lastLure;
	protected GameObject player;

	// Movement variables
	protected Vector2 moveDir;
	protected GameObject nextPath;
	protected SubpathScript movePath;

	// Panic variables
    protected float panicThreshold = 10;
    protected float panicTime;
    protected float timePanicked;

	// Alert variables
    protected float alertThreshold = 5;
    protected float alertLevel;
	
	// Cone of Vision variables
	protected float visionAngleOffset; // max offset of angle from NPC's view
	protected Vector3 npcDir;

	// Sprite
	protected float xScale;
	public GameObject alertTexture;
	public GameObject panicTexture;

	// Use this for initialization
	public void Start ()
	{
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
        
		player = GameObject.Find("Player");
        // Set initial alert/panick states
        timePanicked = panicCooldownSeconds;
        alerted = false;
        panicked = false;
        alertLevel = 0f;
		playerInRange = false;
		visionAngleOffset = .5f * visionAngleSize;

		// TODO: Make this vector match NPC's view
		//forwardLookingDirection = new Vector3 (-1, 0);
		npcDir = NPCDirection.L;
		xScale = transform.localScale.x;

		// Register for all messages that are necessary
		MessageCenter.Instance.RegisterListener (MessageType.PlayerGrabbedNPCs, grabbedListener);
		MessageCenter.Instance.RegisterListener (MessageType.PlayerReleasedNPCs, releasedListener);
		MessageCenter.Instance.RegisterListener (MessageType.TrapEntered, trapEnterListener);
		MessageCenter.Instance.RegisterListener (MessageType.TrapReleased, trapReleaseListener);
		MessageCenter.Instance.RegisterListener (MessageType.LureRadiusEntered, lureEnterListener);
		MessageCenter.Instance.RegisterListener (MessageType.LureReleased, lureReleaseListener);

		// Ignore collision with other AI
		int npcLayer = LayerMask.NameToLayer (npcTag);
		Physics2D.IgnoreLayerCollision (npcLayer, npcLayer);
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

		NPCDestroyedMessage message = new NPCDestroyedMessage (gameObject);
		MessageCenter.Instance.Broadcast (message);

		if (alertTexture != null)
		{
			Destroy(alertTexture.gameObject);
		}
		if (panicTexture != null)
		{
			Destroy (panicTexture.gameObject);
		}
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
    }

	protected virtual void OnTriggerEnter2D(Collider2D other)
	{
		if (other.tag == "Player") 
		{
			playerInRange = true;
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
                timePanicked = panicCooldownSeconds;
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
			Vector2 playerSpeed = player.rigidbody2D.velocity;
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
			if (nearWall)
			{
				nearWall = false;
				moveDir = Quaternion.AngleAxis(90, transform.forward) * -moveDir;
				
			}
			//else  // In stationary, no else
			// TODO: Make more like other updates in AI
			Vector3 oldVelocity = new Vector3(rigidbody2D.velocity.x, rigidbody2D.velocity.y);
			rigidbody2D.velocity = moveDir.normalized * speed;
			determineDirectionChange(oldVelocity, new Vector3(rigidbody2D.velocity.x, rigidbody2D.velocity.y));
			
			return true;
		}
		
		if (checkForPlayer() && player.rigidbody2D != null && player.rigidbody2D.velocity != Vector2.zero)
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
		}
		
		return false;
	}

	//-----------------------
	// Helper Methods
	//-----------------------

	protected void setAnimatorInteger(string key, int animation)
	{
		gameObject.GetComponent<Animator>().SetInteger(key, animation);
	}

	protected void broadcastAlertLevelChanged(AlertLevelType type)
	{
		NPCAlertLevelMessage message = new NPCAlertLevelMessage (gameObject, type);
		MessageCenter.Instance.Broadcast (message);
	}
	
	private void increaseAlertLevel(float sensitivity)
	{
		var playerSpeed = player.rigidbody2D.velocity;
		alertLevel += playerSpeed.magnitude * detectLevel * sensitivity;
	}
	
	protected void decrementAlertLevel()
	{
		float oldLevel = alertLevel;
		alertLevel -= (panicThreshold * 0.05f);
		if (oldLevel >= alertThreshold && alertLevel < alertThreshold)
		{
			broadcastAlertLevelChanged(AlertLevelType.Normal);
			alertTexture.renderer.enabled = false;
			alerted = false;
		}
		
		// Make sure alert level does not go below 0
		if (alertLevel < 0)
			alertLevel = 0;
	}
	
	protected virtual void alert()
	{
		alertTexture.renderer.enabled = true;
		alerted = true;
		broadcastAlertLevelChanged(AlertLevelType.Alert);
	}
	
	protected virtual void panic()
	{
		alertTexture.renderer.enabled = false;
		panicTexture.renderer.enabled = true;
		speed = 1.5f;
		alerted = false;
		panicked = true;
		panicTime = Time.time;
		timePanicked = panicCooldownSeconds;
		moveDir = transform.position - player.transform.position;
		broadcastAlertLevelChanged(AlertLevelType.Panic);
	}

	protected bool checkForPlayer()
	{
		Vector3 playerPos = player.transform.position;
		
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

		float yScale = transform.localScale.y;
		float zScale = transform.localScale.z;

		if (biasPosition.x > 0)
		{
			transform.localScale = new Vector3 (-xScale, yScale, zScale);
		}
		else
		{
			transform.localScale = new Vector3(xScale, yScale, zScale);
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
		int rand = Random.Range(0, spawnPoints.Length);
		return spawnPoints[rand];
	}
	
	protected virtual GameObject getNextPath()
	{
		GameObject path = new GameObject ();
		return path;
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
			grabbed = true; 
			Debug.Log("Gotcha"); 
			alertTexture.renderer.enabled = false;
			panicTexture.renderer.enabled = false;
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
		LureReleasedMessage lureMessage = message as LureReleasedMessage;
		if (lureMessage.NPC.Equals(gameObject))
		{
			lured = false;
			lastLure = lureMessage.Lure;
			//TODO: make getNextPath better
			nextPath = getNextPath();
		}
	}
}



