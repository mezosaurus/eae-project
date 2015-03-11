using UnityEngine;
using System.Collections;

public class LoadingScreen : MonoBehaviour
{
	private LevelLoader levelLoader;

	void Start()
	{
		levelLoader = GameObject.FindObjectOfType<LevelLoader>();
		Application.LoadLevel( levelLoader.levelName );
	}

	void Update()
	{
		// The user wants to quit
		if( Input.GetButtonDown( "Cancel" ) )
		{
			Application.Quit();
		}
	}
}

