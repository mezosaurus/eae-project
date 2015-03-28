using UnityEngine;
using System.Collections;

public class LevelLoader : MonoBehaviour
{
	public string levelName;
	public string modeName;

	private static LevelLoader _instance;

	public static LevelLoader instance
	{
		get
		{
			if( _instance == null )
			{
				_instance = GameObject.FindObjectOfType<LevelLoader>();
				DontDestroyOnLoad( _instance.gameObject );
			}

			return _instance;
		}
	}

	void Awake()
	{
		if( _instance == null )
		{
			_instance = this;
			DontDestroyOnLoad( this );
		}
		else
		{
			if( this != _instance )
				Destroy( this.gameObject );
		}
	}

	void Start()
	{

	}

	void Update()
	{
		// Provide a way to force kill the game
		if( Input.GetButtonDown( "Cancel" ) )
		{
			Application.Quit();
		}
	}
}
