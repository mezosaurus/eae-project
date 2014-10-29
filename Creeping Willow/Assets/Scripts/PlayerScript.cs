using UnityEngine;
using System.Collections;

public class PlayerScript : MonoBehaviour
{
    public float MaxSpeed;
	int direction;
	bool canMove;
    
    private PlayerMovementType movementType, lastMovementType;
    
    private void Start()
    {
        movementType = PlayerMovementType.Stationary;
        lastMovementType = PlayerMovementType.Stationary;

		direction = (int)DirectionState.DOWN;
		canMove = true;

        MessageCenter.Instance.RegisterListener(MessageType.PlayerMovementTypeChanged, EventTest);
		MessageCenter.Instance.RegisterListener (MessageType.AbilityStatusChanged, HandleAbilityStatusChanged);
    }

    private void UpdateMovement()
    {
        Vector2 velocity = new Vector2(Input.GetAxis("LSX"), Input.GetAxis("LSY")) * MaxSpeed * Time.deltaTime;
        Vector2 zero = Vector2.zero;

        if (velocity.magnitude <= 0.01f) velocity = zero;

		// update the DirectionState
		direction = updateState (Input.GetAxis ("LSX"), Input.GetAxis ("LSY"));

        // Update movement type
        if (velocity == zero) movementType = PlayerMovementType.Stationary;
        else movementType = PlayerMovementType.LowProfile;

        rigidbody2D.velocity = velocity;

        if (velocity.x < 0f) transform.localScale = new Vector3(-1f, transform.localScale.y, transform.localScale.z);
        else if (velocity.x > 0f) transform.localScale = new Vector3(1f, transform.localScale.y, transform.localScale.z);

        if (movementType != lastMovementType) MessageCenter.Instance.Broadcast(new PlayerMovementTypeChangedMessage(movementType));
    }

    private void EventTest(Message message)
    {
        PlayerMovementTypeChangedMessage theMessage = message as PlayerMovementTypeChangedMessage;

        Debug.Log("Player movement type changed to: " + theMessage.NewMovementType.ToString());
    }

    private void Update()
    {
		if( canMove )
        	UpdateMovement();
    }

    private void LateUpdate()
    {
        lastMovementType = movementType;
    }

    private void OnDestroy()
    {
        MessageCenter.Instance.UnregisterListener(MessageType.PlayerMovementTypeChanged, EventTest);
		MessageCenter.Instance.UnregisterListener(MessageType.AbilityStatusChanged, HandleAbilityStatusChanged);
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
	 * Return the DirectionState of the player
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
