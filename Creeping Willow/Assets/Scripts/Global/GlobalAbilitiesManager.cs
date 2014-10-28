using UnityEngine;
using System.Collections;

/**
 * A singleton class that stores the variables
 * of the all the player's abilities
 **/
public class GlobalAbilitiesManager : MonoBehaviour {
	/*
	private static GlobalAbilitiesManager instance;

	public static GlobalAbilitiesManager Instance
	{
		get
		{
			if (instance == null) instance = new GlobalAbilitiesManager();
			
			return instance;
		}
	} */

	/***	List of all public ability variables	***/

	/* Minion Ability Variables */

	// Obstruction Minion
	public float obstructionLifetime;
	public float obstructionCooldown;
	public float obstructionDelayTime;
	public float obstructionGrowthTime;
	public int obstructionCount;
	public int obstructionSize;


	/* Ranged Ability Variables */

	// Tire Swing Ranged
	public float tireSwingCooldown;
	public float tireSwingSpeed;
	public float tireSwingDistance;
	public int tireSwingCount;


}
