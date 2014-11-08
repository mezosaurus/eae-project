using UnityEngine;
using System.Collections;

[System.Serializable]
public class Boundary
{
	public float xMin, xMax, zMin, zMax;	
}

public class WanderAIController : AIController {

	public float waitTime;
	public Boundary boundary;
	
	private float previousMovement;
	private int moveCounter;
	private float nextMoveTime;
    private Vector2 pathPosition;
	
	new void Start()
	{
		base.Start ();
		float time = Time.time;
		nextMoveTime = time + waitTime;
	}
	
	void FixedUpdate () 
	{
		if( paused )
			return;

		if (grabbed || alerted)
			return;
		if (panicked)
		{
			timePanicked -= Time.deltaTime;
			if (timePanicked <= 0) {
				panicked = false;
				//GetComponent<SpriteRenderer>().sprite = normalTexture;
				return;
			}
			/*if (!audio.isPlaying)
			{
				//audio.Play();
			}*/
			//Debug.Log ("Move: " + moveDir.x + ", " + moveDir.y);
			if (nearWall)
			{
				nearWall = false;
				moveDir = Quaternion.AngleAxis(90, transform.forward) * -moveDir;
			}
			
			rigidbody2D.velocity = moveDir.normalized * speed;
			return;
		}

        if (killSelf)
        {
            Destroy(gameObject);
        }
		
		if (Time.time >= nextMoveTime)
		{

			nextMoveTime = Time.time + waitTime;
            pathPosition = Random.insideUnitCircle * 2;
			//nextPath = Random.insideUnitCircle * 2;
			Vector3 direction = new Vector3(pathPosition.x, pathPosition.y, 0.0f);
			rigidbody2D.velocity = direction * speed;

            int max = 30;
            int rand = Random.Range(0, max);
            if (rand >= max - 1)
            {
                Destroy(gameObject);
            }

            //Vector3 pathPosition = nextPath.transform.position;
            /*Vector3 position = transform.position;
            float step = speed * Time.deltaTime;
            Vector3 movement = Vector3.MoveTowards(position, pathPosition, step);
            transform.position = movement;*/

            /*
            if (movement == pathPosition)
            {
                if (killSelf)
                    Destroy(gameObject);

                int max = 10;
                int rand = Random.Range(0, max);
                if (rand >= max - 1)
                {
                    killSelf = true;
                    pathPosition = getLeavingPath().transform.position;
                    //nextPath = getLeavingPath();
                }
            }
             */
		}
	}
	
	float getRandomNumber()
	{
		float rand;
		if (moveCounter == 0) 
		{
			//moveCounter = Mathf.FloorToInt(Random.Range(minMovementRepeat,maxMovementRepeat));
			rand = Random.Range (0, 5);
			previousMovement = rand;
		} 
		else
		{
			moveCounter--;
			rand = previousMovement;
		}
		
		return rand;
	}
	
	//	void OnTriggerEnter2D(Collider2D other)
	//	{
	//		if (other.tag == "Player")
	//		{
	//			moveDir = transform.position - player.transform.position;
	//			alerted = true;
	//		}
	//		else if (other.tag == "Wall")
	//		{
	//			nearWall = true;
	//		}
	//	}
	//
	//	void OnTriggerExit2D(Collider2D other)
	//	{
	//		if (other.tag == "Player")
	//		{
	//			alerted = false;
	//		}
	//		else if (other.tag == "Wall")
	//		{
	//			nearWall = false;
	//		}
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
}
