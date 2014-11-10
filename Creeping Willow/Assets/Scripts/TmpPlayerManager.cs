using UnityEngine;
using System.Collections;

public class TmpPlayerManager : MonoBehaviour 
{
	public float speed;
	bool canMove;
	int direction;
	
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
	
	
	// Use this for initialization
	void Start () {
		canMove = true;
		direction = (int)DirectionState.DOWN;
		speed = speed / 100;
		
		RegisterListeners ();
	}
	
	// Update is called once per frame
	void Update () 
	{
		if( canMove )
		{
			controllerMove();
			keyboardMove();
		}
		
	}
	
	/**
	 * Return the direction the player is moving
	 **/
	public int getDirection()
	{
		return direction;
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
	 * Update the position and direction of the player from the controller
	 **/
	private void controllerMove() 
	{
		float diagonalSpeed = Mathf.Sqrt (2) * .5f * speed;
		
		float tmpx = Input.GetAxis ("LSX");
		float tmpy = Input.GetAxis ("LSY");
		
		if( tmpx == 0 && tmpy == 0 )
			return;
		
		// recieve direction of joystick
		if( updateState(tmpx,tmpy) != -1 )
			direction = updateState(tmpx,tmpy);
		
		// move the player
		if( direction == (int)DirectionState.TOP_RIGHT )
		{
			transform.position += new Vector3( diagonalSpeed, diagonalSpeed );
		}
		else if( direction == (int)DirectionState.TOP_LEFT )
		{
			transform.position += new Vector3( -diagonalSpeed, diagonalSpeed );
		}
		else if( direction == (int)DirectionState.BOTTOM_RIGHT )
		{
			transform.position += new Vector3( diagonalSpeed, -diagonalSpeed );
		}
		else if( direction == (int)DirectionState.BOTTOM_LEFT )
		{
			transform.position += new Vector3( -diagonalSpeed, -diagonalSpeed );
		}
		else if( direction == (int)DirectionState.LEFT )
		{
			transform.position += new Vector3( -speed, 0 );
		}
		else if( direction == (int)DirectionState.RIGHT )
		{
			transform.position += new Vector3( speed, 0 );
		}
		else if( direction == (int)DirectionState.UP )
		{
			transform.position += new Vector3( 0, speed );
		}
		else if( direction == (int)DirectionState.DOWN )
		{
			transform.position += new Vector3( 0, -speed );
		}
		else
		{
			// Do nothing
		}
	}
	
	/**
	 * Update the position and direction of the player from the keyboard
	 **/
	private void keyboardMove() 
	{
		/*float diagonalSpeed = Mathf.Sqrt (2) * .5f * speed;
		
		/**** Needs to be updated for xbox controller
		if( ( Input.GetKey(KeyCode.RightArrow) && Input.GetKey(KeyCode.UpArrow) ) )
		{
			transform.position += new Vector3( diagonalSpeed, diagonalSpeed );
			direction = (int)DirectionState.TOP_RIGHT;
		}
		else if( ( Input.GetKey(KeyCode.LeftArrow) && Input.GetKey(KeyCode.UpArrow) ) )
		{
			transform.position += new Vector3( -diagonalSpeed, diagonalSpeed );
			direction = (int)DirectionState.TOP_LEFT;
		}
		else if( ( Input.GetKey(KeyCode.RightArrow) && Input.GetKey(KeyCode.DownArrow) ) )
		{
			transform.position += new Vector3( diagonalSpeed, -diagonalSpeed );
			direction = (int)DirectionState.BOTTOM_RIGHT;
		}
		else if( (Input.GetKey(KeyCode.LeftArrow) && Input.GetKey(KeyCode.DownArrow) ) )
		{
			transform.position += new Vector3( -diagonalSpeed, -diagonalSpeed );
			direction = (int)DirectionState.BOTTOM_LEFT;
		}
		else if( Input.GetKey(KeyCode.LeftArrow) )
		{
			transform.position += new Vector3( -speed, 0 );
			direction = (int)DirectionState.LEFT;
		}
		else if( Input.GetKey(KeyCode.RightArrow) )
		{
			transform.position += new Vector3( speed, 0 );
			direction = (int)DirectionState.RIGHT;
		}
		else if( Input.GetKey(KeyCode.UpArrow) )
		{
			transform.position += new Vector3( 0, speed );
			direction = (int)DirectionState.UP;
		}
		else if( Input.GetKey(KeyCode.DownArrow) )
		{
			transform.position += new Vector3( 0, -speed );
			direction = (int)DirectionState.DOWN;
		}*/
	}
	
	
	void OnDestroy()
	{
		UnregisterListeners ();
	}
	
	
	/**
	 * Register Listeners
	 **/
	void RegisterListeners()
	{
		MessageCenter.Instance.RegisterListener (MessageType.AbilityStatusChanged, HandleAbilityStatusChanged);
	}
	
	/**
	 * Unregister listeners
	 **/
	protected void UnregisterListeners()
	{
		MessageCenter.Instance.UnregisterListener (MessageType.AbilityStatusChanged, HandleAbilityStatusChanged);
	}
	
	/**
	 * AbilityStatusChanged Handler
	 **/
	protected void HandleAbilityStatusChanged(Message message)
	{
		AbilityStatusChangedMessage mess = message as AbilityStatusChangedMessage;
		
		if( mess.abilityInUseStatus )
			canMove = false;
		else
			canMove = true;
	}
}
