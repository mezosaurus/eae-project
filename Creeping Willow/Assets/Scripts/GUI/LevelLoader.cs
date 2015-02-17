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
			DontDestroyOnLoad (this);
		}
		else
		{
			if( this != _instance )
				Destroy( this.gameObject );
		}
	}

	// Use this for initialization
	void Start()
	{

	}
	
	// Update is called once per frame
	void Update()
	{
	
	}
}
