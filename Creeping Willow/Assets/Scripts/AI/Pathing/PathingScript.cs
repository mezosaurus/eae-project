using UnityEngine;
using System.Collections;

public class PathingScript : MonoBehaviour 
{
	private GameObject[] paths;

	void Start()
	{
		paths = GameObject.FindGameObjectsWithTag ("Path");
	}

	public GameObject getRandomPath()
	{
		int rand = Random.Range (0, paths.Length);
		return paths [rand];
	}
}
