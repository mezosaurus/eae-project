using UnityEngine;
using System.Collections;

public class SoundManager : MonoBehaviour
{
	private static SoundManager _instance;
	private AudioSource musicSource;
	private AudioSource uiSource;
	private AudioSource effectsSource;

	public AudioClip clickSound;

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
		musicSource = this.gameObject.GetComponents<AudioSource>()[ 0 ];
		uiSource = this.gameObject.GetComponents<AudioSource>()[ 1 ];
		effectsSource = this.gameObject.GetComponents<AudioSource>()[ 2 ];

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
        /*if (GameObject.FindGameObjectWithTag("MainCamera").audio.isPlaying)
            musicSource.Stop();*/

        musicSource.audio.volume = 0.15f;
	}

	public void ChangeMusic( AudioClip clip )
	{
		if( musicSource.clip != clip && clip != null )
		{
			musicSource.Stop();
			musicSource.clip = clip;
		}

		if( !musicSource.isPlaying )
		{
			musicSource.Play();
		}
	}

	public void PauseMusic()
	{
		musicSource.Pause();
	}

	public void ResumeMusic()
	{
		musicSource.Play();
	}

	public void PlaySoundEffect( AudioClip clip )
	{
		effectsSource.clip = clip;
		effectsSource.Play();
	}

	public void PlayClickSound()
	{
		if( clickSound )
		{
			uiSource.clip = clickSound;
			uiSource.Play();
		}
	}
}
