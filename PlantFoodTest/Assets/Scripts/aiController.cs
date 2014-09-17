using UnityEngine;
using System.Collections;

public class aiController : MonoBehaviour 
{
	// Variables for alerting (alert radius, max alert level, ...)
	public float nourishment;
	public float normalSpeed;
	public float runSpeed;
	public GameObject player;

	protected bool alerted;
}
