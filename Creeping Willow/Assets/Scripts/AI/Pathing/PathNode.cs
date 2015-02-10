using UnityEngine;
using System.Collections;

public class PathNode : MonoBehaviour
{
	public GameObject[] connectedPaths;

	void OnDrawGizmos()
	{
		Vector3 pos = gameObject.transform.position;

		foreach (GameObject path in connectedPaths)
		{
			if (path != null && path.GetComponent<PathNode>().hasConnectingPath(gameObject))
			{
				Gizmos.DrawLine(pos, path.transform.position);
			}
		}
	}

	public GameObject getRandomConnectedPath()
	{
		if (connectedPaths.Length == 1)
		{
			return connectedPaths[0];
		}

		int pos = Random.Range (0, connectedPaths.Length);

		return connectedPaths[pos];
	}

	public bool hasConnectingPath(GameObject node)
	{
		foreach (GameObject path in connectedPaths)
		{
			if (node.Equals(path))
			{
				return true;
			}
		}
		return false;
	}
}
