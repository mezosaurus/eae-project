using UnityEngine;
using System.Collections;

public class AIGenerator : GameBehavior 
{
	public GameObject pathNPC;
	public GameObject stationaryNPC;
	public string spawnTag = "Respawn";
	public string pathTag = "Paths";
	public string benchTag = "Bench";
	public float spawnTime;

	private int countNPC = 2;
	private float lastSpawnTime;
	private GameObject[] spawnPoints;

	void Start()
	{
		spawnPoints = GameObject.FindGameObjectsWithTag(spawnTag);
	}

	// Update is called once per frame
	protected override void GameUpdate () 
	{
		if (lastSpawnTime <= Time.time - spawnTime)
		{
			lastSpawnTime = Time.time;
			createNewNPC();
		}
	}
	
	Vector2 getRandomSpawnPoint()
	{
		int rand = Random.Range(0, spawnPoints.Length);
		return spawnPoints[rand].transform.position;
	}
	
	void createNewNPC()
	{
		int rand = Random.Range (0, countNPC);
		switch(rand)
		{
		case 0:
			createPathNPC();
			break;
		case 1:
			createStationaryNPC();
			break;
		}
	}

	void createPathNPC()
	{
		GameObject newNPC = createNPC (this.pathNPC);
				
		SubpathScript movePath = GameObject.Find (pathTag).GetComponent<PathingScript> ().getRandomPath().GetComponent<SubpathScript>();
		newNPC.GetComponent<PathAIController>().setMovingPath(movePath);
	}

	void createStationaryNPC()
	{
		GameObject newNPC = createNPC (this.stationaryNPC);

		GameObject[] benches = GameObject.FindGameObjectsWithTag (benchTag);
		int rand = Random.Range (0, benches.Length);
		newNPC.GetComponent<StationaryAIController> ().setStationaryPoint (benches [rand]);
 	}

	GameObject createNPC(GameObject NPC)
	{
		return (GameObject) Instantiate (NPC, getRandomSpawnPoint(), Quaternion.identity);
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








