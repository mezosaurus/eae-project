using UnityEngine;
using System.Collections;

public class PlayerAbilityScript : GameBehavior {

	float[] spawnTime;

	// AbilityID
	AbilityID[] abilityID; //{ Minion, Ranged, Lure, Trap, Movement }
	// ability types
	AbilityType[] abilityType; // { Minion, Ranged, Lure, Trap, Movement }

	// prefab paths for ALL abilities
	string[] prefabPath;

	// stored ability variables
	float[] coolDown;
	bool[] coolDownInProgress;

	int numAbilities;

	PlayerScript player;
	int direction;
	Vector3 offset; // offset for firing an ability
	bool abilityUsed;

	// Use this for initialization
	void Start () {
		player = this.gameObject.GetComponent<PlayerScript> ();
		abilityUsed = false;

		RegisterListeners ();

		initializeGlobalVariables ();
	}
	
	// Update is called once per frame
	protected override void GameUpdate () {
		// get current direction
		direction = player.getDirection ();

		// update current abilities available
		updateCooldowns ();

		// check for any abilities being used
		checkForInputs ();
	}

	/**
	 * Checks to see if a button is pressed relating to an ability
	 **/
	private void checkForInputs()
	{
		// Minion
		if( (Input.GetKeyDown(KeyCode.A) || Input.GetButtonDown("A") ) && !abilityUsed )
		{
			// check which minion ability is chosen
			if( abilityID[0] == AbilityID.ObstructionMinion )
			{
				if( !coolDownInProgress[0] )
				{
					coolDownInProgress[0] = true;
					spawnTime[0] = Time.time;
					activateAbility(direction,prefabPath[0],false);
				}
			}
		}
		// Ranged
		if( (Input.GetKeyDown(KeyCode.S) || Input.GetButtonDown("B") ) && !abilityUsed )
		{
			// check which ranged ability is chosen
			if( abilityID[1] == AbilityID.TireSwingRanged )
			{
				if( !coolDownInProgress[1] )
				{
					coolDownInProgress[1] = true;
					spawnTime[1] = Time.time;
					activateAbility(direction,prefabPath[1],true);
				}
			}
		}
		// Lure
		if( (Input.GetKeyDown(KeyCode.D) || Input.GetButtonDown("X") ) && !abilityUsed )
		{
			// check which lure ability is chosen
			if( abilityID[2] == AbilityID.CandyLure )
			{
				if( !coolDownInProgress[2] )
				{
					coolDownInProgress[2] = true;
					spawnTime[2] = Time.time;
					activateAbility(direction,prefabPath[2],false);
				}
			}
		}
		// Trap
		if( (Input.GetKeyDown(KeyCode.F) || Input.GetButtonDown("RB") ) && !abilityUsed )
		{
			// check which trap ability is chosen
			if( abilityID[3] == AbilityID.BushTrap )
			{
				if( !coolDownInProgress[3] )
				{
					coolDownInProgress[3] = true;
					spawnTime[3] = Time.time;
					activateAbility(direction,prefabPath[3],false);
				}
			}
		}
		// movement is 5th
		/*if( (Input.GetKeyDown(KeyCode.R) || Input.GetButtonDown("B") ) && !abilityUsed )
		{
			// check which movement ability is chosen
			if( abilityID[4] == AbilityID.PlayerDash )
			{
				if( !coolDownInProgress[4] )
				{
					coolDownInProgress[4] = true;
					spawnTime[4] = Time.time;
					// don't create object, do something else
				}
			}
		}*/
	}

	/**
	 * Update the cooldowns for each ability
	 **/
	private void updateCooldowns()
	{
		for( int i = 0; i < numAbilities; i++ )
		{
			if( coolDownInProgress[i] )
			{
				float timeElapsed = Time.time - spawnTime[i];
				if(timeElapsed > coolDown[i]){
					coolDownInProgress[i] = false;
					spawnTime[i] = Time.time;
				}
				AbilityCoolDownMessage message = new AbilityCoolDownMessage(abilityType[i], coolDown[i], timeElapsed);
				MessageCenter.Instance.Broadcast(message);
			}
		}
	}

	/**
	 * Launch an ability in the appropriate direction
	 **/
	private void activateAbility(int dir, string path, bool isRanged)
	{
		Vector3 tmpDir;
		float diagMove = Mathf.Sqrt (2) * .5f;

		switch(dir)
		{
			case (int)DirectionState.UP: tmpDir = Vector3.Scale(new Vector3(0,1.5f),collider2D.bounds.extents); break;
			case (int)DirectionState.TOP_RIGHT: tmpDir = Vector3.Scale(new Vector3(diagMove,diagMove)*1.5f,collider2D.bounds.extents*1.6f); break;
			case (int)DirectionState.RIGHT: tmpDir = Vector3.Scale(new Vector3(1.5f,0),collider2D.bounds.extents); break;
			case (int)DirectionState.BOTTOM_RIGHT: tmpDir = Vector3.Scale(new Vector3(diagMove,-diagMove)*1.5f,collider2D.bounds.extents*1.6f); break;
			case (int)DirectionState.DOWN: tmpDir = Vector3.Scale(new Vector3(0,-1.5f),collider2D.bounds.extents); break;
			case (int)DirectionState.BOTTOM_LEFT: tmpDir = Vector3.Scale(new Vector3(-diagMove,-diagMove)*1.5f,collider2D.bounds.extents*1.6f); break;
			case (int)DirectionState.LEFT: tmpDir = Vector3.Scale(new Vector3(-1.5f,0),collider2D.bounds.extents); break;
			case (int)DirectionState.TOP_LEFT: tmpDir = Vector3.Scale(new Vector3(-diagMove,diagMove)*1.5f,collider2D.bounds.extents*1.6f); break;
			default: tmpDir = new Vector3(); break;
		}

		GameObject go = (GameObject)Instantiate( Resources.Load(path), transform.position + tmpDir, Quaternion.identity);
		if( isRanged )
			go.GetComponent<RangedAbilityClass> ().setDirection (tmpDir);
	}

	/**
	 * Initialize global variables
	 **/
	void initializeGlobalVariables()
	{
		// number of abilities available
		numAbilities = sizeof(AbilityID);

		// abilityIDs
		abilityID = new AbilityID[numAbilities];
		abilityID [0] = AbilityID.ObstructionMinion;
		abilityID [1] = AbilityID.TireSwingRanged;
		abilityID [2] = AbilityID.CandyLure;
		abilityID [3] = AbilityID.BushTrap;
		//abilityID [4] = AbilityID.PlayerDash;

		// abilityTypes
		abilityType = new AbilityType[5];
		abilityType [0] = AbilityType.Minion;
		abilityType [1] = AbilityType.Ranged;
		abilityType [2] = AbilityType.Lure;
		abilityType [3] = AbilityType.Trap;
		abilityType [4] = AbilityType.Movement;

		// spawnTimes are initialized when an instance is created on a button click
		spawnTime = new float[4];

		// cooldowns
		coolDown = new float[numAbilities];
		coolDownInProgress = new bool[numAbilities];

		for( int i = 0; i < numAbilities; i++ )
		{
			coolDown[i] = 5;
			coolDownInProgress[i] = false;
		}

		// prefab paths
		prefabPath = new string[numAbilities];
		prefabPath[0] = "Prefabs/Abilities/ObstructionMinion";
		prefabPath[1] = "Prefabs/Abilities/TireSwingRanged";
		prefabPath[2] = "Prefabs/Abilities/CandyLure";
		prefabPath[3] = "Prefabs/Abilities/BushTrap";
	}

	void OnDestroy()
	{
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
	void HandleAbilityStatusChanged(Message message)
	{
		AbilityStatusChangedMessage mess = message as AbilityStatusChangedMessage;

		if( mess.abilityInUseStatus )
		{
			abilityUsed = true;
		}
		else
			abilityUsed = false;
	}
}

/**
 * Enumerator that contains the every type of ability
 **/
public enum AbilityID
{
	ObstructionMinion,
	TireSwingRanged,
	CandyLure,
	BushTrap//,
	//PlayerDash
}
