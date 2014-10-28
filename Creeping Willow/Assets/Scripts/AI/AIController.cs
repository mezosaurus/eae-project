using UnityEngine;
using System.Collections;

public class AIController : MonoBehaviour {

	private Vector3 moveDir;
	public float speed;
	private Vector3 spawnMove;

	// Use this for initialization
	void Start () {
		spawnMove = GameObject.Find ("SpawnMove1").transform.position;
	}
	
	// Update is called once per frame
	void Update () {
		Vector3 position = transform.position;
		//Vector3 goal = GameObject.Find ("SpawnMoves/SpawnMove1").transform.position;
		float step = speed * Time.deltaTime;
		Vector3 movement = Vector3.MoveTowards (position, spawnMove, step);
		transform.position = movement;
		if (movement == spawnMove)
			Destroy (gameObject);
	}
}
