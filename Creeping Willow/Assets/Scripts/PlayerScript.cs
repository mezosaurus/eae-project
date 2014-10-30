using UnityEngine;
using System.Collections;

public class PlayerScript : MonoBehaviour
{
    public float MaxLowProfileSpeed, MaxHighProfileSpeed;

    private PlayerMovementType movementType, lastMovementType;
    private bool lowProfileMovement;
    private Vector3 movementStart, movementDestination;
    private float movementAccumulator;
    private int direction;
    private bool canMove;
    
    private void Start()
    {
        movementType = PlayerMovementType.Stationary;
        lastMovementType = PlayerMovementType.Stationary;
        lowProfileMovement = true;
        movementDestination = Vector3.zero;

		direction = (int)DirectionState.DOWN;
		canMove = true;

		MessageCenter.Instance.RegisterListener (MessageType.AbilityStatusChanged, HandleAbilityStatusChanged);
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
            if(Input.GetButtonDown("Y"))
            {
                Vector3 offset = velocity.normalized;

                movementStart = transform.position;
                movementDestination = transform.position + (offset * 5f);
                movementAccumulator = 0f;
            }
        }

        if(movementDestination != Vector3.zero)
        {
            rigidbody2D.velocity = Vector2.zero;
            movementType = PlayerMovementType.VeryHighProfile;

            movementAccumulator += Time.deltaTime * 3f;

            transform.position = Vector3.Lerp(movementStart, movementDestination, movementAccumulator);

            if (movementAccumulator >= 1f) movementDestination = Vector3.zero;
        }
        else rigidbody2D.velocity = velocity;

        if (velocity.x < 0f) transform.localScale = new Vector3(-1f, transform.localScale.y, transform.localScale.z);
        else if (velocity.x > 0f) transform.localScale = new Vector3(1f, transform.localScale.y, transform.localScale.z);

        if (movementType != lastMovementType) MessageCenter.Instance.Broadcast(new PlayerMovementTypeChangedMessage(movementType));

        if (Input.GetButtonDown("Back")) lowProfileMovement = !lowProfileMovement;
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
