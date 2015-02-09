using UnityEngine;
using System.Collections;

public class PathingScript : MonoBehaviour 
{
	private GameObject[] subPaths;

	void Start()
	{
		subPaths = GameObject.FindGameObjectsWithTag ("Path");
	}

	public GameObject getRandomPath()
	{
		int rand = Random.Range (0, subPaths.Length);
		return subPaths [rand];
	}
}
