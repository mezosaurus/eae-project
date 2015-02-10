using UnityEngine;
using System.Collections;

public class LevelLoader : MonoBehaviour
{
	public string levelName;

	// Use this for initialization
	void Start()
	{
		Object.DontDestroyOnLoad( this.gameObject );
	}
	
	// Update is called once per frame
	void Update()
	{
	
	}
}
