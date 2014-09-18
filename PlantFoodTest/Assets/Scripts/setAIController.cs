using UnityEngine;
using System.Collections;

public class setAIController : aiController
{
	public Transform[] moveWayPoints;

	private int wayPointIndex;
	private bool reverseDirection;

	void Start()
	{
		reverseDirection = false;
	}

	void Update () 
	{
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
