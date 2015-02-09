using UnityEngine;

public class SubpathMazeScript : SubpathScript
{
	public GameObject[] nodes;

	public override GameObject getNextPath(GameObject currentPath, GameObject npc)
	{
		if (currentPath == null)
		{
			return getClosestPathPoint(npc);
			//return paths[0];
		}
		int index = 0;
		for (int i = 0; i < nodes.Length ; i++)
		{
			if (nodes[i].transform.position.Equals(currentPath.transform.position))
			{
				index = i;
				break;
			}
		}
		return nodes[index].GetComponent<PathNode>().getRandomConnectedPath();
	}

	protected override GameObject getClosestPathPoint (GameObject npc)
	{
		if (nodes.Length == 0)
			return null;
		
		if (npc == null)
		{
			return nodes[Random.Range(0, nodes.Length)];
		}
		
		float minDistance = 0f;
		int retIndex = 0;
		if (nodes.Length == 1)
			return nodes[0];
		for (int i = 0; i < nodes.Length; i++)
		{
			Vector3 pathPointPos = nodes[i].transform.position;
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
		return nodes [retIndex];
	}
}
