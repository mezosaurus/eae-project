using UnityEngine;
using System.Collections;

public class AIController : MonoBehaviour {

	public float speed;
	private GameObject nextPath;
	private Vector3 moveDir;
	//private Vector3 spawnMove;
	private SubpathScript movePath;
	private bool killSelf = false;

	// Use this for initialization
	void Start () {
		movePath = GameObject.Find ("Paths").GetComponent<PathingScript> ().getRandomPath().GetComponent<SubpathScript>();
		nextPath = movePath.getNextPath (null);
		//spawnMove = GameObject.Find ("Path1Point1").transform.position;
	}
	
	// Update is called once per frame
	void Update () {
		Vector3 pathPosition = nextPath.transform.position;
		if (killSelf)
		{
			pathPosition = GameObject.FindGameObjectWithTag("Respawn").transform.position;
		}
		Vector3 position = transform.position;
		//Vector3 goal = GameObject.Find ("SpawnMoves/SpawnMove1").transform.position;
		float step = speed * Time.deltaTime;
		Vector3 movement = Vector3.MoveTowards (position, pathPosition, step);
		//Vector3 movement = Vector3.MoveTowards (position, spawnMove, step);
		transform.position = movement;
		if (movement == pathPosition)
		{
			if (killSelf)
				Destroy(gameObject);

			int max = 10;
			int rand = Random.Range (0, max);
			if (rand < max - 1)
				nextPath = movePath.getNextPath(nextPath);
			else
				killSelf = true;
		}
	}
}
