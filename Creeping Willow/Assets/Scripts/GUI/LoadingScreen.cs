using UnityEngine;
using System.Collections;

public class LoadingScreen : MonoBehaviour
{
	private LevelLoader levelLoader;

	// Use this for initialization
	void Start()
	{
		levelLoader = GameObject.Find( "PersistentObject" ).GetComponent<LevelLoader>();
		Application.LoadLevel( levelLoader.levelName );
	}
	
	// Update is called once per frame
	void Update()
	{
	
	}
}

