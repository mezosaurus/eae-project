using UnityEngine;
using System.Collections;

public class PathNode : MonoBehaviour
{
	public PathNode[] connectedPaths;

	void OnDrawGizmos()
	{
		Vector3 pos = gameObject.transform.position;

		foreach (PathNode path in connectedPaths)
		{
			if (path != null && path.hasConnectingPath(this))
				Gizmos.DrawLine(pos, path.transform.position);
		}
	}

	public PathNode getRandomConnectedPath()
	{
		return connectedPaths[Random.Range(0, connectedPaths.Length)];
	}

	public bool hasConnectingPath(PathNode node)
	{
		foreach (PathNode path in connectedPaths)
		{
			if (node.Equals(path))
			{
				return true;
			}
		}
		return false;
	}
}
