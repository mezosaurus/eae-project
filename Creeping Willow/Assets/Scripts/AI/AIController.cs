using UnityEngine;
using System.Collections;

public class AIController : GameBehavior {

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

    protected bool nearWall;
    protected Vector2 moveDir;
    protected float alertedTime;
    protected float panicTime;
    public float panicCooldown;
    protected float timePanicked;

	protected GameObject nextPath;
	protected SubpathScript movePath;
	protected bool killSelf = false;

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
            alerted = false;
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

            PlayerScript controller = player.GetComponent<PlayerScript>();
            bool lowProfileMovement = controller.lowProfileMovement;
            float playerSpeed;
            if (controller.lowProfileMovement)
            {
                playerSpeed = controller.MaxLowProfileSpeed;
            }
            else
            {
                playerSpeed = controller.MaxHighProfileSpeed;
            }

            if (alerted == true)
            {
                //Debug.Log ("Time: " + Time.time);
                //Debug.Log ("Waiting: " + (alertedTime + alertTimer));
                if (Time.time >= alertedTime + alertTimer)
                {
                    //Debug.Log ("Alert Timer!");
                    alerted = false;
                    if (playerSpeed > 75)
                    {
                        //Debug.Log("BECOMING PANICKED!");
                        panicked = true;
                        panicTime = Time.time;
                        timePanicked = panicCooldown;
                        moveDir = transform.position - player.transform.position;
                        //GetComponent<SpriteRenderer>().sprite = panicTexture;
                    }
                    else
                    {
                        //Debug.Log("BECOMING normal");
                        //GetComponent<SpriteRenderer>().sprite = normalTexture;
                    }
                }
                return;
            }

            if (playerSpeed > 75)
            {
                alerted = true;
                alertedTime = Time.time;
                //GetComponent<SpriteRenderer>().sprite = alertTexture;
            }
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



