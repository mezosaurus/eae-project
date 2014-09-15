using UnityEngine;
using System.Collections;

public class setAIController : aiController
{
	//public float boundaryX, boundaryZ;
	public Transform[] moveWayPoints;

	private NavMeshAgent nav;
	private int wayPointIndex;

	void Start()
	{
		nav = GetComponent<NavMeshAgent> ();
		wayPointIndex = moveWayPoints.Length - 1;
	}

	void Update () 
	{
		nav.speed = normalSpeed;

		if(nav.remainingDistance == nav.stoppingDistance)
		{
			//Debug.Log("waypoint: " + wayPointIndex);
			//Debug.Log ("last index: " + (moveWayPoints.Length - 1));
			if (wayPointIndex == moveWayPoints.Length - 1)
				wayPointIndex = 0;
			else wayPointIndex++;
		}

		nav.destination = moveWayPoints [wayPointIndex].position;

		// Move in clockwise fashion ( up Z, right X, down Z, left X)

		//float moveHorizontal, moveVertical;

		//if (rigidbody.position.z < boundaryZ)
			//moveVertical = speed;
		//else if (rigidbody.position
		//Vector3 movement = new Vector3 (moveHorizontal, 0.0f, moveVertical);
		//rigidbody.velocity = movement * speed;
	}
}
