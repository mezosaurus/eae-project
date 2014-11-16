using UnityEngine;
using System.Collections;

/**
 * A base class template of the player's abilities.
 **/
public abstract class AbilityClass : GameBehavior {

	// protected properties
	protected PlayerScript PM;

	//public float coolDown;
	public float lifetime;
	protected float tmpCoolDown;
	protected float timeModifier; // used for certain abilities
	protected bool coolDownInProgress;
	protected Vector3 direction;

	protected bool abilityUsed;
	protected float spawnTime;
	protected AbilityType type;
	protected string prefabPath;

	// Use this for initialization
	public virtual void Start () 
	{
		//PM = GameObject.Find ("Player").GetComponent<PlayerScript> ();

		coolDownInProgress = false;
		timeModifier = .03f; // used for certain abilities
		//tmpCoolDown = coolDown;
		spawnTime = Time.time;

		abilityUsed = false;

		// register listeners
		RegisterListeners ();
	}
	
	// Update is called once per frame
	protected override void GameUpdate () 
	{
		float timeElapsed = Time.time - spawnTime;
		
		if( lifetime > 0 && timeElapsed > lifetime ){
			Destroy( this.gameObject );
		}
	}

	/**
	 * Set the direction that the ability should be facing
	 **/
	public void setDirection(Vector3 dir)
	{
		direction = dir;
	}

	/**
	 * Return the path of the prefab for this ability
	 **/
	public string getPath()
	{
		return prefabPath;
	}

	public virtual void OnDestroy()
	{
		// unregister listeners
		UnregisterListeners ();

		//send out a message that the object was removed
		AbilityObjectRemovedMessage mess = new AbilityObjectRemovedMessage(type);
		MessageCenter.Instance.Broadcast (mess);
	}

	/**
	 * Register listeners
	 **/
	protected void RegisterListeners()
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

		abilityUsed = mess.abilityInUseStatus;
	}

}
