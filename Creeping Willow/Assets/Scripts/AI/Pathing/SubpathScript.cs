using UnityEngine;
using System.Collections;

public class SubpathScript : MonoBehaviour 
{
	public GameObject[] paths;

	public GameObject getNextPath(GameObject currentPath)
	{
		if (currentPath == null) 
		{
			return paths[0];
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
}
