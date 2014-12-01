using UnityEngine;
using System.Collections;

public class SubpathScript : MonoBehaviour 
{
	public GameObject[] paths;

	public GameObject getNextPath(GameObject currentPath, GameObject npc)
	{
		if (currentPath == null)
		{
			return getClosestPathPoint(npc);
			//return paths[0];
		}
		int index = 0;
		for (int i = 0; i < paths.Length ; i++)
		{
			if (paths[i] == currentPath)
			{
				index = i;
				break;
			}
		}
		index = (index + 1) % paths.Length;
		//if (paths[index])
		return paths[index];
	}

	private GameObject getClosestPathPoint (GameObject npc)
	{
		float minDistance = 0f;
		int retIndex = 0;
		if (paths.Length == 1)
			return paths[0];
		for (int i = 0; i < paths.Length; i++)
		{
			Vector3 pathPointPos = paths[i].transform.position;
			Vector3 npcPos = npc.transform.position;
			float distance = Vector3.Distance(pathPointPos, npcPos);
			if (minDistance == 0f)
				minDistance = distance;
			if (distance <= minDistance)
			{
				minDistance = distance;
				retIndex = i;
			}
		}
		return paths [retIndex];
	}
}
