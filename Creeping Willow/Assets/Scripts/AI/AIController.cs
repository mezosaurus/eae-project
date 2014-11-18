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

	//private Vector3 moveDir;
	//private Vector3 spawnMove;
    public float nourishment;
    public float alertTimer;
    public GameObject player;
	public float lurePower;

    //public Sprite normalTexture;
    //public Sprite alertTexture;
    //public Sprite panicTexture;
	//public Texture alertTexture;
	public GameObject alertTexture;
	public GameObject panicTexture;

	public float speed;
    public bool alerted;
	public bool grabbed;
    public bool panicked;
	public bool lured;
	protected bool playerInRange;

    protected bool nearWall;
    protected Vector2 moveDir;
    protected float alertedTime;
    protected float panicTime;
    public float panicCooldown;
    protected float timePanicked;
	protected Lure lastLure;

	protected GameObject nextPath;
	protected SubpathScript movePath;
	protected bool killSelf = false;
    protected float panicThreshold = 10;
    protected float alertThreshold = 5;
    protected float alertLevel;
    // This variable determines an NPC's awareness, or how easily they are able to detect things going on in their surroundings. Range (0,1)
    public float detectLevel;
    // playerSpeed = playerCurrentSpeed / playerMaxSpeed;
    // alertLevel += playerSpeed * detectLevel
    // if (alertLevel >= alertThreshold)
            // NPC ALERT
    // else if (alertLevel >= panicThreshold)
            // NPC PANIC

	// Tags
	public string spawnTag = "Respawn";
	public string npcTag = "NPC";

	// Cone of Vision variables
	public float visionDistance; // distance that player's view extends to
	public float visionAngleSize; // The total angle size that the player can see
	public Vector3 npcDir;
	
	protected float visionAngleOffset; // max offset of angle from NPC's view

	// Use this for initialization
	public void Start ()
	{
		// Alert Texture
		alertTexture = (GameObject)Instantiate (GameObject.Find ("AIAlert"));
		alertTexture.renderer.enabled = false;
		TextureScript alertTs = alertTexture.GetComponent<TextureScript> ();
		alertTs.target = gameObject;

		// Panic Texture
		panicTexture = (GameObject)Instantiate (GameObject.Find ("AIPanic"));
		panicTexture.renderer.enabled = false;
		TextureScript panicTs = panicTexture.GetComponent<TextureScript> ();
		panicTs.target = gameObject;
        
		player = GameObject.Find("Player");
        // Set initial alert/panick states
        timePanicked = panicCooldown;
        alerted = false;
        panicked = false;
        alertLevel = 0f;
		playerInRange = false;
		visionAngleOffset = .5f * visionAngleSize;

		// TODO: Make this vector match NPC's view
		//forwardLookingDirection = new Vector3 (-1, 0);
		npcDir = NPCDirection.L;

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
	}

	//-----------------------
	// Trigger Methods
	//-----------------------

    void OnTriggerExit2D(Collider2D other)
    {
        if (other.tag == "Player")
        {
			playerInRange = false;
        }
    }

	void OnTriggerEnter2D(Collider2D other)
	{
		if (other.tag == "Player") 
		{
			playerInRange = true;
		}
	}

    void OnTriggerStay2D(Collider2D other)
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
                timePanicked = panicCooldown;
                return;
            }

            var playerSpeed = player.rigidbody2D.velocity;

            if (alertLevel >= alertThreshold)
            {
                //Debug.Log("ALERTED");
				alertTexture.renderer.enabled = true;
                alerted = true;
                alertedTime = Time.time;
				broadcastAlertLevelChanged(AlertLevelType.Alert);

            }
            if (alertLevel >= panicThreshold)
            {
                //Debug.Log("PANICKED");
				/*
				alertTexture.renderer.enabled = false;
				panicTexture.renderer.enabled = true;
                speed = 1.5f;
                alerted = false;
                panicked = true;
                panicTime = Time.time;
                timePanicked = panicCooldown;
                moveDir = transform.position - player.transform.position;
				broadcastAlertLevelChanged(AlertLevelType.Panic);
				*/
				panic();
            }

            // Increment alertLevel
            //Debug.Log("ALERT LEVEL = " + alertLevel);
            //Debug.Log("MAGNITUDE = " + playerSpeed.magnitude);
            alertLevel += playerSpeed.magnitude * detectLevel;
        }
    }

	//-----------------------
	// Helper Methods
	//-----------------------

	private void panic()
	{
		alertTexture.renderer.enabled = false;
		panicTexture.renderer.enabled = true;
		speed = 1.5f;
		alerted = false;
		panicked = true;
		panicTime = Time.time;
		timePanicked = panicCooldown;
		moveDir = transform.position - player.transform.position;
		broadcastAlertLevelChanged(AlertLevelType.Panic);
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
		
		// Make sure alert level does not go below 0
		if (alertLevel < 0)
			alertLevel = 0;

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
				//GetComponent<SpriteRenderer>().sprite = normalTexture;
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

		if (checkForPlayer() && player.rigidbody2D.velocity != Vector2.zero)
		{
			// TODO: Balance better
			panic ();
			return true;
		}

		return false;
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
	}
	
	protected void broadcastAlertLevelChanged(AlertLevelType type)
	{
		NPCAlertLevelMessage message = new NPCAlertLevelMessage (gameObject, type);
		MessageCenter.Instance.Broadcast (message);
	}
	
	protected bool checkForPlayer()
	{
		Vector3 playerPos = player.transform.position;
		
		// check if NPC can see that far
		if( Vector3.Distance(transform.position, playerPos) <= visionDistance )
		{
			// get direction of player from NPC's point of view
			Vector3 direction = playerPos - transform.position;

			// TODO: Update forward vector by NPC's direction or what not
			// TODO: Get rid of above todo

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
		/*
		Debug.Log (direction);
		if (direction.Equals(NPCDirection.R))
			Debug.Log ("Right");
		else if (direction.Equals(NPCDirection.TR))
			Debug.Log ("Top Right");
		else if (direction.Equals(NPCDirection.T))
			Debug.Log ("Top");
		else if (direction.Equals(NPCDirection.TL))
			Debug.Log ("Top Left");
		else if (direction.Equals(NPCDirection.L))
			Debug.Log ("Left");
		else if (direction.Equals(NPCDirection.BL))
			Debug.Log ("Bottom Left");
		else if (direction.Equals(NPCDirection.B))
			Debug.Log ("Bottom");
		else
			Debug.Log ("Bottom Right");
		//*/

		npcDir = direction;
		// TODO: Sprite Magic
	}

	protected GameObject getLeavingPath()
	{
		GameObject[] spawnPoints = GameObject.FindGameObjectsWithTag (spawnTag);
		int rand = Random.Range(0, spawnPoints.Length);
		return spawnPoints[rand];
	}
	
	protected virtual GameObject getNextPath()
	{
		Debug.Log ("parent");
		GameObject path = new GameObject ();
		return path;
	}

	//-----------------------
	// Listener Methods
	//-----------------------

	void grabbedListener(Message message)
	{
        if (((PlayerGrabbedNPCsMessage)message).NPCs.Contains(gameObject))
        { grabbed = true; Debug.Log("Gotcha"); }
	}

	void releasedListener(Message message)
	{
		if (((PlayerReleasedNPCsMessage)message).NPCs.Contains(gameObject))
		{
			grabbed = false;
		}
	}

	void trapEnterListener(Message message)
	{
		GameObject trappedNPC = ((TrapEnteredMessage)message).NPC;
		if (trappedNPC.Equals(gameObject))
			grabbed = true;
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



