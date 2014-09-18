using UnityEngine;
using System.Collections;

public class setAIController : aiController
{
	public Transform[] moveWayPoints;

	private int wayPointIndex;
	private bool reverseDirection;
	private Vector3 moveDir;
	private bool nearWall;

	void Start()
	{
		reverseDirection = false;
	}

	void OnTriggerEnter(Collider other)
	{
		if (other.tag == "Player")
		{
			alerted = true;
			
			moveDir = transform.position - player.transform.position;
		}
		else if (other.tag == "Wall")
		{
			nearWall = true;
		}
	}

	void OnTriggerExit(Collider other)
	{
		if (other.tag == "Player")
			alerted = false;
		else if (other.tag == "Wall")
		{
			nearWall = false;
			
			//transform.rotation = Quaternion.AngleAxis(180, new Vector3(90, 0, 0)) * transform.rotation;
		}
	}

	void Update () 
	{
		if (grabbed)
			return;
		if (alerted)
		{
			if (nearWall)
			{
				nearWall = false;
				moveDir.Scale(new Vector3(-1, -1, 0));
			}
			else
				rigidbody.velocity = moveDir.normalized * runSpeed;

			if (!audio.isPlaying)
			{
				audio.Play();
			}
			
			return;
		}
		if(transform.position == moveWayPoints[wayPointIndex].position)
		{
			if (wayPointIndex == moveWayPoints.Length - 1)
				reverseDirection = true;
			else if (wayPointIndex == 0)
				reverseDirection = false;

			if (reverseDirection)
				wayPointIndex--;
			else wayPointIndex++;
		}

		//Debug.Log ("Way point: " + wayPointIndex);
		Vector3 position = transform.position;
		Vector3 goal = moveWayPoints [wayPointIndex].position;
		float step = normalSpeed * Time.deltaTime;
		Vector3 movement = Vector3.MoveTowards (position, goal, step);
		//if (movement > goal)
		//{
			//Debug.Log ("Setting to goal position");
			//transform.position = goal;
		//}
		transform.position = movement;
	}
}
