using UnityEngine;
using System.Collections;

public class LoadingScreen : MonoBehaviour
{
	private LevelLoader levelLoader;
	private SoundManager soundManager;

	void Start()
	{
		soundManager = GameObject.FindObjectOfType<SoundManager>();
		soundManager.GetComponents<AudioSource>()[ 0 ].Stop();
		soundManager.GetComponents<AudioSource>()[ 0 ].clip = null;

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

