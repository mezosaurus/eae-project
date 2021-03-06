﻿using UnityEngine;
using System.Collections;

public abstract class SubpathScript : MonoBehaviour 
{
	public abstract GameObject getNextPath (GameObject currentPath, GameObject npc);

	protected abstract GameObject getClosestPathPoint (GameObject npc);
}
