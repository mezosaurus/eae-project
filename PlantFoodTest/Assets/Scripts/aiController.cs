using UnityEngine;
using System.Collections;

public class aiController : MonoBehaviour 
{
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

	public void Start()
	{
 		audio.clip = chaseMusic;
		alerted = false;
		panicked = false;
	}
	
	void OnTriggerExit2D(Collider2D other)
	{
		if (other.tag == "Player")
		{
			panicked = false;
			alerted = false;
			GetComponent<SpriteRenderer>().sprite = normalTexture;
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
				return;
			}

			PlayerController controller = player.GetComponent<PlayerController>();
			float playerSpeed = controller.CurrentSpeed / controller.MaxSpeed;

			if (alerted == true)
			{
				Debug.Log ("Time: " + Time.time);
				Debug.Log ("Waiting: " + (alertedTime + alertTimer));
				if (Time.time >= alertedTime + alertTimer)
				{
					Debug.Log ("Alert Timer!");
					alerted = false;
					if ( playerSpeed >= 0.5)
					{
						Debug.Log ("BECOMING PANICKED!");
						panicked = true;
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
}
