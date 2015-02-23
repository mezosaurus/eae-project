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
		if (subPaths == null)
		{
			subPaths = GameObject.FindGameObjectsWithTag("Path");
		}
		int rand = Random.Range (0, subPaths.Length);
		return subPaths [rand];
	}
}
