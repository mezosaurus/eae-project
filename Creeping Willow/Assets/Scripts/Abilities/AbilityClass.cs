using UnityEngine;
using System.Collections;

/**
 * A base class template of the player's abilities.
 **/
public abstract class AbilityClass : MonoBehaviour {

	// protected properties
	protected GlobalAbilitiesManager GAM;
	protected PlayerScript PM;

	protected float coolDown;
	protected float tmpCoolDown;
	protected float timeModifier;
	protected bool coolDownInProgress;

	protected bool isAbility;
	protected int count;
	protected bool abilityUsed;

	// Use this for initialization
	public virtual void Start () 
	{
		GAM = GameObject.Find ("Global Manager").GetComponent<GlobalAbilitiesManager> ();
		PM = GameObject.Find ("Player").GetComponent<PlayerScript> ();

		coolDownInProgress = false;
		timeModifier = .03f;
		tmpCoolDown = coolDown;

		abilityUsed = false;

		// register listeners
		RegisterListeners ();
	}
	
	// Update is called once per frame
	public virtual void Update () 
	{
		// update the cooldown;
		if( coolDownInProgress )
		{
			tmpCoolDown -= timeModifier;
			if( tmpCoolDown < 0 )
			{
				coolDownInProgress = false;
				tmpCoolDown = coolDown;
			}
		}
	}

	/**
	 * Return the number of minions available to the player
	 **/
	public int getCount()
	{
		return count;
	}

	void OnDestroy()
	{
		// unregister listeners
		UnregisterListeners ();
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
