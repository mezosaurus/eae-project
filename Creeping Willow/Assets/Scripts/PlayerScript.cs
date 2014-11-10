using UnityEngine;
using System.Collections.Generic;

public class PlayerScript : GameBehavior
{
    public float MaxLowProfileSpeed, MaxHighProfileSpeed;

    private PlayerMovementType movementType, lastMovementType;
    public bool lowProfileMovement;
    private Vector3 movementDirection;
    private float movementTimer;
    private int direction;
    private bool canMove;

    // Temp
    public enum State
    {
        Normal,
        Eating,
        EatingCinematic
    }

    public State state;
    private List<GameObject> npcsInRange;
    public Texture EatingBarBackground, EatingBarForeground;
    public Texture[] Buttons;
    public float EatingDecay, EatingIncrease;
    private string[] buttons = { "A", "B", "X", "Y" };
    private float percentage;
    private int qteButton;
    private float xScale;
    
    private void Start()
    {
        movementType = PlayerMovementType.Stationary;
        lastMovementType = PlayerMovementType.Stationary;
        lowProfileMovement = true;
        movementDirection = Vector3.zero;
        movementTimer = 0;

		direction = (int)DirectionState.DOWN;
		canMove = true;

		MessageCenter.Instance.RegisterListener (MessageType.AbilityStatusChanged, HandleAbilityStatusChanged);
		MessageCenter.Instance.RegisterListener (MessageType.PauseChanged, HandlePauseChanged);

        // Temp
        state = State.Normal;
        npcsInRange = new List<GameObject>();

        xScale = transform.localScale.x;
    }

    private void OnCollisionEnter2D(Collision2D collision)
    {
        if (movementTimer > 0f) movementTimer = 0f;
    }

    private void OnTriggerEnter2D(Collider2D collider)
    {
        if(collider.tag == "NPC")
        {
            npcsInRange.Add(collider.gameObject);
        }
    }

    private void OnTriggerExit2D(Collider2D collider)
    {
        if (collider.tag == "NPC")
        {
            npcsInRange.Remove(collider.gameObject);
        }
    }

    private void UpdateMovement()
    {
        Vector2 velocity = new Vector2(Input.GetAxis("LSX"), Input.GetAxis("LSY"));
        float speed = (lowProfileMovement) ? MaxLowProfileSpeed : MaxHighProfileSpeed;
        Vector2 zero = Vector2.zero;

        velocity = velocity * speed * Time.deltaTime;

        if (velocity.magnitude <= 0.01f) velocity = zero;

		// update the DirectionState
		direction = updateState (Input.GetAxis ("LSX"), Input.GetAxis ("LSY"));

        // Update movement type
        if (velocity == zero) movementType = PlayerMovementType.Stationary;
        else movementType = (lowProfileMovement) ? PlayerMovementType.LowProfile : PlayerMovementType.HighProfile;

        // Handle dashing
        if(velocity != zero)
        {
            if(Input.GetButtonDown("Y") && movementTimer <= 0f)
            {
                movementDirection = velocity.normalized * 30f;
                movementTimer = 0.2f;
            }
        }

        if(movementTimer > 0f)
        {
            rigidbody2D.velocity = movementDirection;
            movementType = PlayerMovementType.VeryHighProfile;

            movementTimer -= Time.deltaTime;
        }
        else rigidbody2D.velocity = velocity;

        if (velocity.x < 0f) transform.localScale = new Vector3(-xScale, transform.localScale.y, transform.localScale.z);
        else if (velocity.x > 0f) transform.localScale = new Vector3(xScale, transform.localScale.y, transform.localScale.z);

        if (movementType != lastMovementType) MessageCenter.Instance.Broadcast(new PlayerMovementTypeChangedMessage(movementType));

        if (Input.GetButtonDown("Back")) lowProfileMovement = !lowProfileMovement;

        // Temp
        if(Input.GetAxis("LT") > 0.2f && npcsInRange.Count > 0)
        {
            PlayerGrabbedNPCsMessage message = new PlayerGrabbedNPCsMessage(npcsInRange);

            foreach (GameObject npc in npcsInRange)
            {
                Vector3 offset = new Vector3(Random.Range(-0.5f, 0.5f), Random.Range(-0.5f, 0.5f));
                npc.transform.position = transform.position + offset;
            }

            MessageCenter.Instance.Broadcast(message);

            MessageCenter.Instance.Broadcast(new CameraZoomInMessage());

            state = State.Eating;

            percentage = 0.5f;
            qteButton = Random.Range(0, 3);

            rigidbody2D.velocity = Vector2.zero;
        }
    }

    private void UpdateEating()
    {
        if (Input.GetAxis("LT") < 0.5f || percentage <= 0f)
        {
            MessageCenter.Instance.Broadcast(new PlayerReleasedNPCsMessage(npcsInRange));
            state = State.Normal;

            npcsInRange.Clear();

            MessageCenter.Instance.Broadcast(new CameraZoomOutMessage());

            return;
        }

        if (percentage >= 1f)
        {
            foreach (GameObject npc in npcsInRange) GameObject.Destroy(npc);

            npcsInRange.Clear();

            state = State.Normal;

            MessageCenter.Instance.Broadcast(new CameraZoomOutMessage());

            return;
        }

        Vector3 offset = new Vector3(transform.position.x, transform.position.y - 0.35f, -1f);

        //foreach (NPCOffset npcOffset in npcsInRange) npcOffset.NPC.transform.position = offset + npcOffset.Offset;

        percentage -= (EatingDecay * Time.deltaTime);

        Debug.Log(percentage);

        if (Input.GetButtonDown(buttons[qteButton]))
        {
            percentage += (EatingIncrease * Time.deltaTime);

            if (percentage > 1f) percentage = 1f;
        }
    }

    private void UpdateEatingCinematic()
    {

    }

    protected override void GameUpdate()
    {
        switch (state)
        {
            case State.Normal:
                if (canMove)
                    UpdateMovement();
                break;

            case State.Eating:
                UpdateEating();
                break;

            case State.EatingCinematic:
                UpdateEatingCinematic();
                break;
        }
    }

    private void LateUpdate()
    {
        lastMovementType = movementType;
    }

    private void OnDestroy()
    {
		MessageCenter.Instance.UnregisterListener(MessageType.AbilityStatusChanged, HandleAbilityStatusChanged);
		MessageCenter.Instance.UnregisterListener(MessageType.AbilityStatusChanged, HandlePauseChanged);
    }

	/**
	 * Recieves the angle from the controller's input and outputs 
	 * the resulting DirectionState as an integer
	 **/
	private int updateState(float x, float y)
	{
		// get angle of input
		float angle = Mathf.Atan2 (y, x) * (180 / Mathf.PI);
		
		// get DirectionState from angle
		if( angle >= -22.5f && angle < 22.5f )
			return (int)DirectionState.RIGHT;
		else if( angle >= 22.5f && angle < 67.5f )
			return (int)DirectionState.TOP_RIGHT;
		else if( angle >= 67.5f && angle < 112.5f )
			return (int)DirectionState.UP;
		else if( angle >= 112.5 && angle < 157.5f )
			return (int)DirectionState.TOP_LEFT;
		else if( angle >= 157.5f || angle < -157.5f )
			return (int)DirectionState.LEFT;
		else if( angle >= -157.5f && angle < -112.5f )
			return (int)DirectionState.BOTTOM_LEFT;
		else if( angle >= -112.5f && angle < -67.5f )
			return (int)DirectionState.DOWN;
		else if( angle >= -67.5f && angle < -22.5f )
			return (int)DirectionState.BOTTOM_RIGHT;
		else 
			return -1;
	}

	/**
	 * Return the DirectionState of the player as an int
	 **/
	public int getDirection()
	{
		return direction;
	}

	/**
	 * AbilityStatusChanged Handler
	 **/
	protected void HandleAbilityStatusChanged(Message message)
	{
		AbilityStatusChangedMessage mess = message as AbilityStatusChangedMessage;
		
		if( mess.abilityInUseStatus )
		{
			canMove = false;
			transform.rigidbody2D.velocity = new Vector2(0,0);
		}
		else
			canMove = true;
	}

	protected void HandlePauseChanged(Message message)
	{
		PauseChangedMessage mess = message as PauseChangedMessage;

		canMove = !mess.isPaused;
		transform.rigidbody2D.velocity = new Vector2(0,0);
	}

    void OnGUI()
    {
        if (state == State.Eating)
        {
            float x = (Screen.width - EatingBarBackground.width - Buttons[qteButton].width) / 2f;
            //float y = 172f;
            float y = ((Camera.main.WorldToScreenPoint(transform.position - new Vector3(0f, 1f, 0)).y - EatingBarBackground.height) / 2f);

            GUI.DrawTexture(new Rect(x, y, EatingBarBackground.width, EatingBarBackground.height), EatingBarBackground);
            GUI.DrawTexture(new Rect(x + 5f, y + 5f, EatingBarForeground.width * percentage, EatingBarForeground.height), EatingBarForeground);
            GUI.DrawTexture(new Rect(x + EatingBarBackground.width, y, Buttons[qteButton].width, Buttons[qteButton].height), Buttons[qteButton]);
        }
    }
}

public enum PlayerMovementType
{
    Stationary,
    LowProfile,
    HighProfile,
    VeryHighProfile
}

/**
	 * List of states for the player regarding the
	 * direction they are facing
	 **/
public enum DirectionState
{
	UP,
	TOP_RIGHT,
	RIGHT,
	BOTTOM_RIGHT,
	DOWN,
	BOTTOM_LEFT,
	LEFT,
	TOP_LEFT,
};
