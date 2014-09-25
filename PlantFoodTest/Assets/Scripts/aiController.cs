using UnityEngine;
using System.Collections;

public class aiController : MonoBehaviour 
{
	public float nourishment;
	public float normalSpeed;
	public float runSpeed;
	public GameObject player;

	public AudioClip chaseMusic;

	public Texture alertTexture;
	public Texture panicTexture;

	public bool alerted;
	public bool paniced;
	public bool grabbed;

	protected bool nearWall;
	protected Vector2 moveDir; 
	protected float alertedTime;

	public void Start()
	{
 		audio.clip = chaseMusic;
	}
	
	void OnTriggerExit2D(Collider2D other)
	{
		if (other.tag == "Player")
			alerted = false;
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
//			if (alerted = true)
//			{
//
//			}
			PlayerController controller = player.GetComponent<PlayerController>();
			if (controller.CurrentSpeed / controller.MaxSpeed >= 0.5)
			{
				alerted = true;
				
				moveDir = transform.position - player.transform.position;
			}
		}
	}
}
