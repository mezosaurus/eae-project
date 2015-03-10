using UnityEngine;
using System.Collections;

public class SubpathNormalScript : SubpathScript
{
	public GameObject[] paths;

	public override GameObject getNextPath(GameObject currentPath, GameObject npc)
	{
		if (currentPath == null)
		{
			return getClosestPathPoint(npc);
			//return paths[0];
		}
		int index = 0;
		Vector3 currentPathPosition = currentPath.transform.position;
		for (int i = 0; i < paths.Length ; i++)
		{
			if (paths[i].transform.position == currentPathPosition)
			{
				index = i;
				break;
			}
		}
		index = (index + 1) % paths.Length;
		//if (paths[index])
		return paths[index];
	}
	
	protected override GameObject getClosestPathPoint (GameObject npc)
	{
		if (paths.Length == 0)
			return null;

		if (npc == null)
		{
			return paths[Random.Range(0, paths.Length)];
		}

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
