using UnityEngine;
using System.Collections;

public class AIGenerator : GameBehavior 
{
	public GameObject pathNPC;
	public GameObject stationaryNPC;
    public GameObject wanderNPC;
	public string spawnTag = "Respawn";
	public string pathTag = "Paths";
	public string benchTag = "Bench";
	public float spawnTime;

	private int numberOfNPCs = 3;
	private float lastSpawnTime;
	private GameObject[] spawnPoints;

	public int maxNumberOfEachNPC = 1;
	private ArrayList stationaryAIList;
	private ArrayList pathAIList;
	private ArrayList wanderAIList;

	void Start()
	{
		spawnPoints = GameObject.FindGameObjectsWithTag(spawnTag);

		stationaryAIList = new ArrayList ();
		pathAIList = new ArrayList ();
		wanderAIList = new ArrayList ();
	}

	// Update is called once per frame
	protected override void GameUpdate () 
	{
		if (lastSpawnTime <= Time.time - spawnTime && isRoomAvailableForNewNPC())
		{
			lastSpawnTime = Time.time;
			createNewNPC();
		}
	}

	bool isRoomAvailableForNewNPC()
	{
		if (pathAIList.Count >= maxNumberOfEachNPC
		    && stationaryAIList.Count >= maxNumberOfEachNPC
//		    && wanderAIList.Count >= maxNumberOfEachNPC	// Taken out for basic build
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
		GameObject newNPC = createNPC (this.pathNPC, pathAIList);

		SubpathScript movePath = GameObject.Find (pathTag).GetComponent<PathingScript> ().getRandomPath().GetComponent<SubpathScript>();
		newNPC.GetComponent<PathAIController>().setMovingPath(movePath);
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
        GameObject wanderNPC = createNPC(this.wanderNPC, wanderAIList);
    }

	GameObject createNPC(GameObject NPC, ArrayList aiList)
	{
		GameObject npc = (GameObject)Instantiate (NPC, getRandomSpawnPoint (), Quaternion.identity);
		aiList.Add (npc);
		return npc;
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








