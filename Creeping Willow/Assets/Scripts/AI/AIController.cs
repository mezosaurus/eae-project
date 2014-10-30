﻿using UnityEngine;
using System.Collections;

public class AIController : MonoBehaviour {

<<<<<<< HEAD
	//private Vector3 moveDir;
	public float speed;
	private Vector3 spawnMove;
	public float nourishment;
	public float normalSpeed;
	public float runSpeed;
	public float alertTimer;
	public GameObject player;
	
	public AudioClip chaseMusic;
	
	public Sprite normalTexture;
	public Sprite alertTexture;
	public Sprite panicTexture;
	
	public bool alerted;
	public bool panicked;
	public bool grabbed;
	
	protected bool nearWall;
	protected Vector2 moveDir; 
	protected float alertedTime;
	protected float panicTime;
	public float panicCooldown;
	protected float timePanicked;
	
	public void Start()
	{
		audio.clip = chaseMusic;
		timePanicked = panicCooldown;
		alerted = false;
		panicked = false;
		//spawnMove = GameObject.Find ("SpawnMove1").transform.position;
	}
	
	void OnTriggerExit2D(Collider2D other)
	{
		if (other.tag == "Player")
		{
			//panicked = false;
			alerted = false;
			//GetComponent<SpriteRenderer>().sprite = normalTexture;
		}
	}
	
	void OnTriggerStay2D(Collider2D other)
	{
		if (other.tag == "Wall")
		{
			RaycastHit2D raycast = Physics2D.Raycast(transform.position, moveDir);
			if (raycast.collider != null && raycast.collider.tag == "Wall")
			{
				float distance = Vector2.Distance(transform.position, raycast.point);
				if (distance < 1.5f)
					nearWall = true;
				else
					nearWall = false;
			}
		}
		else if (other.tag == "Player")
		{
			if (panicked)
			{
				timePanicked = panicCooldown;
				return;
			}
			
			PlayerScript controller = player.GetComponent<PlayerScript>();
			float playerSpeed = controller.CurrentSpeed / controller.MaxSpeed;
			
			if (alerted == true)
			{
				//Debug.Log ("Time: " + Time.time);
				//Debug.Log ("Waiting: " + (alertedTime + alertTimer));
				if (Time.time >= alertedTime + alertTimer)
				{
					//Debug.Log ("Alert Timer!");
					alerted = false;
					if ( playerSpeed >= 0.5)
					{
						Debug.Log ("BECOMING PANICKED!");
						panicked = true;
						panicTime = Time.time;
						timePanicked = panicCooldown;
						moveDir = transform.position - player.transform.position;
						GetComponent<SpriteRenderer>().sprite = panicTexture;
					}
					else
					{
						Debug.Log ("BECOMING normal");
						GetComponent<SpriteRenderer>().sprite = normalTexture;
					}
				}
				return;
			}
			
			if (playerSpeed >= 0.5)
			{
				alerted = true;
				alertedTime = Time.time;
				GetComponent<SpriteRenderer>().sprite = alertTexture;
			}
		}
	}

	// Use this for initialization
	/*void Start () {
		spawnMove = GameObject.Find ("SpawnMove1").transform.position;
=======
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
>>>>>>> 358724c430cf43ab3cd43c416f20474ae2df15bb
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
<<<<<<< HEAD
		if (movement == spawnMove)
			Destroy (gameObject);
	}*/
=======
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
>>>>>>> 358724c430cf43ab3cd43c416f20474ae2df15bb
}
