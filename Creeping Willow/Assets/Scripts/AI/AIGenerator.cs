using UnityEngine;
using System.Collections;

public class AIGenerator : GameBehavior 
{
	// AI Prefabs
	public GameObject pathNPC;
	public GameObject stationaryNPC;
	public GameObject wanderNPC;
	public GameObject enemyNPC;
	public GameObject critterNPC;
	public int maxNumberOfEachNPC = 1;

	public string spawnTag = "Respawn";
	public string pathTag = "Paths";
	public string benchTag = "Bench";
	public string critterSpawnTag = "CritterSpawn";
	public float spawnTime;
	public float critterSpawnTime;

	private int numberOfNPCs = 3;	// Decremented to 2 for no wander AI
	private float lastSpawnTime;
	private GameObject[] spawnPoints;
	private GameObject[] critterSpawnPoints;

	private float lastCritterTime; 
	private ArrayList stationaryAIList;
	private ArrayList pathAIList;
	private ArrayList wanderAIList;
	private ArrayList enemyAIList;
	private ArrayList critterAIList;
		
	void Start()
	{
		spawnPoints = GameObject.FindGameObjectsWithTag(spawnTag);
		critterSpawnPoints = GameObject.FindGameObjectsWithTag (critterSpawnTag);

		stationaryAIList = new ArrayList ();
		pathAIList = new ArrayList ();
		wanderAIList = new ArrayList ();
		enemyAIList = new ArrayList ();
		if (critterSpawnPoints.Length != 0)
			critterAIList = new ArrayList ();

		MessageCenter.Instance.RegisterListener (MessageType.NPCDestroyed, NPCDestroyListener);
		MessageCenter.Instance.RegisterListener (MessageType.NotorietyMaxed, NotorietyMeterListener);
		initMap ();
	}
	
	void OnDestroy()
	{
		MessageCenter.Instance.UnregisterListener (MessageType.NPCDestroyed, NPCDestroyListener);
		MessageCenter.Instance.UnregisterListener (MessageType.NotorietyMaxed, NotorietyMeterListener);
	}
	
	private void NotorietyMeterListener(Message message)
	{
		NotorietyMaxedMessage notorietyMessage = message as NotorietyMaxedMessage;
		createEnemyNPC (notorietyMessage.panickedPosition);
	}
	
	// Update is called once per frame
	protected override void GameUpdate () 
	{
		if (lastSpawnTime <= Time.time - spawnTime && isRoomAvailableForNewNPC())
		{
			lastSpawnTime = Time.time;
			createNewNPC();
		}

		if (critterAIList != null && lastCritterTime <= Time.time - critterSpawnTime && (critterAIList.Count < maxNumberOfEachNPC))
		{
			lastCritterTime = Time.time;
			createCritterNPC();
		}
	}
	
	private void initMap()
	{
		// 3 pathing npcs
		GameObject[] paths = GameObject.FindGameObjectsWithTag ("Path");		
		for (int i = 0; i < paths.Length; i++)
		{
			GameObject path = paths[i];
			SubpathScript movePath = path.GetComponent<SubpathScript>();
			Vector2 pathPos = movePath.getNextPath(null, null).transform.position;
			createPathNPC(pathPos, movePath);
		}

		//3 bench npcs
		GameObject[] benches = GameObject.FindGameObjectsWithTag (benchTag);
		for (int i = 0; i < 3; i++)
		{
			int rand = Random.Range (0, benches.Length);
			GameObject bench = benches[rand];
			Vector2 spawnPos = bench.transform.position;
			GameObject newStationary = createNPC (this.stationaryNPC, stationaryAIList, spawnPos);
			newStationary.GetComponent<StationaryAIController> ().setStationaryPoint (bench);
		}
	}
	
	bool isRoomAvailableForNewNPC()
	{
		if (pathAIList == null) {
			Debug.Log ("PATH ERROR");
			return false;
		}
		if (stationaryAIList == null) {
			Debug.Log ("STATIONARY ERROR");
			return false;
		}

		if (wanderAIList == null) {
			Debug.Log ("WANDER ERROR");
			return false;
		}

		if (pathAIList.Count >= maxNumberOfEachNPC
		    && stationaryAIList.Count >= maxNumberOfEachNPC
		    && wanderAIList.Count >= maxNumberOfEachNPC
		    )
			return false;
		
		return true;
	}
	
	Vector2 getRandomSpawnPoint()
	{
		int rand = Random.Range(0, spawnPoints.Length);
		return spawnPoints[rand].transform.position;
	}
	
	void createNewNPC()
	{
		int rand = Random.Range (0, numberOfNPCs);
		switch(rand)
		{
		case 0:
			if (pathAIList.Count < maxNumberOfEachNPC)
			{
				createPathNPC();
			}
			else
			{
				// TODO: Make better
				//	Idea: make a list of all available AI in isRoomAvailable... to get an enum of creating NPCs
				// 	Idea: Make a recursive method of createNewNPC that takes in the rand and the number of overflows (call from here with 0) that way each type gets called
				createNewNPC ();
			}
			break;
		case 1:
			if (stationaryAIList.Count < maxNumberOfEachNPC)
			{
				createStationaryNPC();
			}
			else
			{
				// TODO: Make better
				createNewNPC();
			}
			break;
		case 2:
			if (wanderAIList.Count < maxNumberOfEachNPC)
			{
				createWanderNPC();
			}
			else
			{
				// TODO: Make better
				createNewNPC();
			}
			break;
		}
	}
	
	void createPathNPC()
	{
		SubpathScript movePath = GameObject.Find (pathTag).GetComponent<PathingScript> ().getRandomPath().GetComponent<SubpathScript>();
		createPathNPC (getRandomSpawnPoint (), movePath);
	}

	void createPathNPC(Vector2 spawnPoint, SubpathScript movePath)
	{
		GameObject newNPC = createNPC (this.pathNPC, pathAIList, spawnPoint);

		newNPC.GetComponent<PathAIController>().setMovingPath(movePath);
		
		if (Random.Range(0,2) == 0)
		{
			loadNPCWithSkin(newNPC, "bopper_skin", NPCSkinType.Bopper);
		}
		else
		{
			loadNPCWithSkin(newNPC, "mower_skin", NPCSkinType.MowerMan);
		}
	}

	void createStationaryNPC()
	{
		GameObject newNPC = createNPC (this.stationaryNPC, stationaryAIList);
		
		GameObject[] benches = GameObject.FindGameObjectsWithTag (benchTag);
		int rand = Random.Range (0, benches.Length);
		newNPC.GetComponent<StationaryAIController> ().setStationaryPoint (benches [rand]);
	}
	
	void createWanderNPC()
	{
		GameObject newNPC = createNPC(this.wanderNPC, wanderAIList);
		loadNPCWithSkin(newNPC, "hippie_skin", NPCSkinType.Hippie);
	}

	void createEnemyNPC(Vector3 panickedPosition)
	{
		if (enemyAIList.Count > 15)
			return;
		
		GameObject newNPC = createNPC (this.enemyNPC, enemyAIList);
		GameObject panickedPoint = new GameObject ();
		panickedPoint.transform.position = panickedPosition;
		newNPC.GetComponent<EnemyAIController> ().setStationaryPoint (panickedPoint);
	}

	void createCritterNPC()
	{
		int rand = Random.Range (0, critterSpawnPoints.Length);
		Vector2 pos = critterSpawnPoints [rand].transform.position;
		GameObject newNPC = createNPC (this.critterNPC, critterAIList, pos);
		newNPC.GetComponent<CritterController> ().setSpawnPosition (pos);
		loadNPCWithSkin (newNPC, "Critters/testSkin", NPCSkinType.CritterTest);
	}

	GameObject createNPC(GameObject NPC, ArrayList aiList)
	{
		GameObject npc = (GameObject)Instantiate (NPC, getRandomSpawnPoint (), Quaternion.identity);
		aiList.Add (npc);
		return npc;
	}
	
	GameObject createNPC(GameObject NPC, ArrayList aiList, Vector2 spawnPoint)
	{
		GameObject npc = (GameObject)Instantiate (NPC, spawnPoint, Quaternion.identity);
		aiList.Add (npc);
		return npc;
	}
		
	void loadNPCWithSkin(GameObject npc, string skinName, NPCSkinType skinType)
	{
		string skinLoc = "prefabs/AI/NPCSkinPrefabs/" + skinName;
		GameObject skin = (GameObject)Instantiate (Resources.Load (skinLoc));
		npc.GetComponent<SpriteRenderer> ().sprite = skin.GetComponent<SpriteRenderer> ().sprite;
		npc.GetComponent<Animator> ().runtimeAnimatorController = skin.GetComponent<Animator> ().runtimeAnimatorController;
        npc.GetComponent<AIController>().SkinType = skinType;
		Destroy (skin);
	}

	void NPCDestroyListener(Message message)
	{
		NPCDestroyedMessage npcMessage = message as NPCDestroyedMessage;
		GameObject NPC = npcMessage.NPC;
		
		if (lastSpawnTime <= Time.time - spawnTime && NPC.GetComponent<EnemyAIController>() == null)
			lastSpawnTime = Time.time;
		
		if (pathAIList.Contains(NPC))
			pathAIList.Remove(NPC);
		else if (stationaryAIList.Contains(NPC))
			stationaryAIList.Remove(NPC);
		else if (wanderAIList.Contains(NPC))
			wanderAIList.Remove(NPC);
		else if (enemyAIList.Contains(NPC)) 
			enemyAIList.Remove(NPC);
		else if (critterAIList.Contains(NPC))
			critterAIList.Remove(NPC);
	}
	
	// For use when updating spawn points to 'gates'
	float getSpawnXPosition()
	{
		return -11;
	}
	
	float getSpawnYPosition(float xPosition)
	{
		Vector2 point1 = new Vector2 (-26.5f, 0.0f);
		Vector2 point2 = new Vector2 (0.0f, 15.3f);
		
		float slope = (point2.y - point1.y) / (point2.x - point1.x);
		float intercept = point1.y - (slope * point1.x);
		
		return (slope * xPosition) + intercept;
	}
}

public enum NPCSkinType
{
    OldMan,
    MowerMan,
    Bopper,
    Hippie,
    AxeMan,
    CritterTest
}








