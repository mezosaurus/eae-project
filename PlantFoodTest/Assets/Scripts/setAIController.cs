using UnityEngine;
using System.Collections;

public class setAIController : aiController
{
	public Transform[] moveWayPoints;

	private int wayPointIndex;
	private bool reverseDirection;

	new void Start()
	{
		base.Start ();
		reverseDirection = false;
	}

//	void OnTriggerEnter2D(Collider2D other)
//	{
//		if (other.tag == "Player")
//		{
//			alerted = true;
//			
//			moveDir = transform.position - player.transform.position;
//		}
//	}
//
//	void OnTriggerExit2D(Collider2D other)
//	{
//		if (other.tag == "Player")
//			alerted = false;
//	}
//
//	void OnTriggerStay2D(Collider2D other)
//	{
//		if (other.tag == "Wall")
//		{
//			RaycastHit2D raycast = Physics2D.Raycast(transform.position, moveDir);
//			if (raycast.collider != null && raycast.collider.tag == "Wall")
//			{
//				float distance = Vector2.Distance(transform.position, raycast.point);
//				if (distance < 1.5f)
//					nearWall = true;
//				else
//					nearWall = false;
//			}
//		}
//	}

	void Update ()
	{
		if (grabbed || alerted)
			return;
		if (panicked)
		{
			timePanicked -= Time.deltaTime;
			if (timePanicked <= 0) {
				panicked = false;
				GetComponent<SpriteRenderer>().sprite = normalTexture;
				return;
			}
			if (nearWall)
			{
				nearWall = false;
				moveDir = Quaternion.AngleAxis(90, transform.forward) * -moveDir;
			}
			else
				rigidbody2D.velocity = moveDir.normalized * runSpeed;

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
