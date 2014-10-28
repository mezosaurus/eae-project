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
			Move();

	}

	/**
	 * Return the direction the player is moving
	 **/
	public int getDirection()
	{
		return direction;
	}

	/**
	 * Update the position and direction of the player
	 **/
	private void Move() 
	{
		float diagonalSpeed = Mathf.Sqrt (2) * .5f * speed;

		/**** Needs to be updated for xbox controller ****/
		if( Input.GetKey(KeyCode.RightArrow) && Input.GetKey(KeyCode.UpArrow) )
		{
			transform.position += new Vector3( diagonalSpeed, diagonalSpeed );
			direction = (int)DirectionState.TOP_RIGHT;
		}
		else if( Input.GetKey(KeyCode.LeftArrow) && Input.GetKey(KeyCode.UpArrow) )
		{
			transform.position += new Vector3( -diagonalSpeed, diagonalSpeed );
			direction = (int)DirectionState.TOP_LEFT;
		}
		else if( Input.GetKey(KeyCode.RightArrow) && Input.GetKey(KeyCode.DownArrow) )
		{
			transform.position += new Vector3( diagonalSpeed, -diagonalSpeed );
			direction = (int)DirectionState.BOTTOM_RIGHT;
		}
		else if( Input.GetKey(KeyCode.LeftArrow) && Input.GetKey(KeyCode.DownArrow) )
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
		}
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
