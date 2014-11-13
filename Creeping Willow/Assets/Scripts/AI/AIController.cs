using UnityEngine;
using System.Collections;

public class AIController : GameBehavior {

	public enum NPCDirection
	{
		T,
		TL,
		TR,
		B,
		BL,
		BR,
		L,
		R
	}
	public NPCDirection npcDir;
	//private Vector3 moveDir;
	//private Vector3 spawnMove;
    public float nourishment;
    public float alertTimer;
    public GameObject player;
	public float lurePower;

    //public Sprite normalTexture;
    //public Sprite alertTexture;
    //public Sprite panicTexture;

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

	// Use this for initialization
	public void Start ()
	{
        player = GameObject.Find("Player");
        // Set initial alert/panick states
        timePanicked = panicCooldown;
        alerted = false;
        panicked = false;
        alertLevel = 0f;
		playerInRange = false;

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
                alerted = true;
                alertedTime = Time.time;
				NPCAlertLevelMessage message = new NPCAlertLevelMessage (gameObject, AlertLevelType.Alert);
				MessageCenter.Instance.Broadcast (message);

            }
            if (alertLevel >= panicThreshold)
            {
                //Debug.Log("PANICKED");
                speed = 1.5f;
                alerted = false;
                panicked = true;
                panicTime = Time.time;
                timePanicked = panicCooldown;
                moveDir = transform.position - player.transform.position;
				NPCAlertLevelMessage message = new NPCAlertLevelMessage (gameObject, AlertLevelType.Panic);
				MessageCenter.Instance.Broadcast (message);
            }

            // Increment alertLevel
            //Debug.Log("ALERT LEVEL = " + alertLevel);
            Debug.Log("MAGNITUDE = " + playerSpeed.magnitude);
            alertLevel += playerSpeed.magnitude * detectLevel;
        }
    }

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
				//TODO: make lure grab better
				//grabbed = true;
				// TODO: go to lure
				lured = true;
				nextPath = lureMessage.Lure.gameObject;
			}
		}
	}

	private Lure lastLure;
	void lureReleaseListener(Message message)
	{
		LureReleasedMessage lureMessage = message as LureReleasedMessage;
		if (lureMessage.NPC.Equals(gameObject))
		{
			//TODO: make lure release better
			//grabbed = false;
			lured = false;
			lastLure = lureMessage.Lure;
			nextPath = getNextPath();
		}
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
}



