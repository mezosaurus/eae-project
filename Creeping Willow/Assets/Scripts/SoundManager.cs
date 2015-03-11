using UnityEngine;
using System.Collections;

public class SoundManager : MonoBehaviour
{
	private static SoundManager _instance;

	public static SoundManager instance
	{
		get
		{
			if( _instance == null )
			{
				_instance = GameObject.FindObjectOfType<SoundManager>();
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
		
	}
}
