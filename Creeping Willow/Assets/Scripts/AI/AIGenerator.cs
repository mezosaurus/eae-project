using UnityEngine;
using System.Collections;

public class AIGenerator : MonoBehaviour 
{
	public GameObject npc;
	public float spawnTime;

	private float lastSpawnTime;
	private GameObject[] spawnPoints;

	void Start()
	{
		spawnPoints = GameObject.FindGameObjectsWithTag("Respawn");
	}

	// Update is called once per frame
	void Update () 
	{
		if (lastSpawnTime <= Time.time - spawnTime)
		{
			lastSpawnTime = Time.time;

			//float xSpawn = getSpawnXPosition();
			//float ySpawn = getSpawnYPosition(xSpawn);

			//Vector2 spawnPoint1 = GameObject.FindGameObjectWithTag("Respawn").transform.position;
			int rand = Random.Range(0, spawnPoints.Length);
			Vector2 spawnPoint = spawnPoints[rand].transform.position;
			Instantiate (npc, spawnPoint, Quaternion.identity);
			//Instantiate (npc, new Vector3(xSpawn, ySpawn, 0), Quaternion.identity);
		}
	}

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
