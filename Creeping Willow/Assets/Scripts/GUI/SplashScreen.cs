using UnityEngine;
using System.Collections;
using UnityEngine.UI;

public class SplashScreen : MonoBehaviour
{
	private enum SplashNumber
	{
		None,
		Unity,
		Utah,
		Team,
		Game,
		Load,
	}

	public Image unityImage;
	public Image eaeImage;
	public Image utahImage;
	public Image teamImage;
	public Image gameImage;

	public Image coverImage;
	public Image coverImage1;
	public Image coverImage2;
	public Image coverImage3;

	private bool fadeStarted;
	private bool axisBusy;

	private SplashNumber splash;

	private float timer;
	private const float st = 3.0f;
	private const float ft = 1.0f;
	private const float bt = 1.0f;

	private GameObject PersistentObject;

	public void Start()
	{
		fadeStarted = false;
		splash = SplashNumber.None;
		timer = 0.0f;

		coverImage.enabled = true;
		coverImage1.enabled = false;
		coverImage2.enabled = false;
		coverImage3.enabled = false;

		unityImage.enabled = true;
		eaeImage.enabled = false;
		utahImage.enabled = false;
		teamImage.enabled = false;
		gameImage.enabled = false;

		axisBusy = false;

		PersistentObject = GameObject.Find( "PersistentObject" );
		GameObject.DontDestroyOnLoad( PersistentObject );
	}

	void Update()
	{
		timer += Time.deltaTime;

		if( !axisBusy && ( Input.GetMouseButtonDown( 0 ) || Input.GetButtonDown( "Start" ) || Input.GetButtonDown( "A" ) ) )
		{
			axisBusy = true;

			switch( splash )
			{
			case SplashNumber.Load:
				break;

			case SplashNumber.Game:
				timer = ( 5*bt + 8*ft + 4*st );
				gameImage.enabled = false;
				break;

			case SplashNumber.Team:
				timer = ( 4*bt + 7*ft + 3*st );
				teamImage.enabled = false;
				break;

			case SplashNumber.Utah:
				timer = ( 3*bt + 5*ft + 2*st );
				eaeImage.enabled = false;
				utahImage.enabled = false;
				break;

			case SplashNumber.Unity:
				timer = ( 2*bt + 3*ft + st );
				unityImage.enabled = false;
				break;

			default:
				timer = ( bt + ft );
				break;
			}
		}
		// The user wants to go back
		else if( !axisBusy && ( Input.GetButtonDown( "Back" ) || Input.GetButtonDown( "Cancel" ) ) )
		{
			Application.Quit();
			
			axisBusy = true;
		}
		else
		{
			axisBusy = false;
		}

		// Load Level
		if( timer >= ( 5*bt + 8*ft + 4*st ) )
		{
			splash = SplashNumber.Load;
			GoToMenu();
		}
		// Blank
		else if( timer > ( 4*bt + 8*ft + 4*st ) && timer < ( 5*bt + 8*ft + 4*st ) && splash == SplashNumber.Game )
		{
			gameImage.enabled = false;
			fadeStarted = false;
		}
		// Fade out logo
		else if( timer > ( 4*bt + 7*ft + 4*st ) && timer < ( 4*bt + 8*ft + 4*st ) && !fadeStarted )
		{
			gameImage.CrossFadeAlpha( 0, ft, true );
			fadeStarted = true;
		}
		// Show Logo
		else if( timer >= ( 4*bt + 7*ft + 3*st ) && timer < ( 4*bt + 7*ft + 4*st ) )
		{
			gameImage.enabled = true;
			coverImage3.enabled = false;
			splash = SplashNumber.Game;
			fadeStarted = false;
		}
		// Fade in game logo
		else if( timer > ( 4*bt + 6*ft + 3*st ) && timer < ( 4*bt + 7*ft + 3*st ) && !fadeStarted )
		{
			gameImage.enabled = true;
			coverImage3.CrossFadeAlpha( 0, ft, true );
			fadeStarted = true;
		}






		// Blank
		else if( timer > ( 3*bt + 6*ft + 3*st ) && timer < ( 4*bt + 6*ft + 3*st ) && splash == SplashNumber.Team )
		{
			teamImage.enabled = false;
			coverImage3.enabled = true;
			fadeStarted = false;
		}
		// Fade out team logo
		else if( timer > ( 3*bt + 5*ft + 3*st ) && timer < ( 3*bt + 6*ft + 3*st ) && !fadeStarted )
		{
			teamImage.CrossFadeAlpha( 0, ft, true );
			fadeStarted = true;
		}
		// Show team logo
		else if( timer >= ( 3*bt + 5*ft + 2*st ) && timer < ( 3*bt + 5*ft + 3*st ) )
		{
			teamImage.enabled = true;
			coverImage2.enabled = false;
			splash = SplashNumber.Team;
			fadeStarted = false;
		}
		// Fade in team logo
		else if( timer > ( 3*bt + 4*ft + 2*st ) && timer < ( 3*bt + 5*ft + 2*st ) && !fadeStarted )
		{
			teamImage.enabled = true;
			coverImage2.CrossFadeAlpha( 0, ft, true );
			fadeStarted = true;
		}

		// Blank
		else if( timer > ( 2*bt + 4*ft + 2*st ) && timer < ( 3*bt + 4*ft + 2*st ) && splash == SplashNumber.Utah )
		{
			eaeImage.enabled = false;
			utahImage.enabled = false;
			coverImage2.enabled = true;
			fadeStarted = false;
		}
		// Fade out Utah
		else if( timer > ( 2*bt + 3*ft + 2*st ) && timer < ( 2*bt + 4*ft + 2*st ) && !fadeStarted )
		{
			eaeImage.CrossFadeAlpha( 0, ft, true );
			utahImage.CrossFadeAlpha( 0, ft, true );
			fadeStarted = true;
		}
		// Show Utah
		else if( timer >= ( 2*bt + 3*ft + st ) && timer < ( 2*bt + 3*ft + 2*st ) )
		{
			eaeImage.enabled = true;
			utahImage.enabled = true;
			coverImage1.enabled = false;
			splash = SplashNumber.Utah;
			fadeStarted = false;
		}
		// Fade in Utah
		else if( timer > ( 2*bt + 2*ft + st ) && timer < ( 2*bt + 3*ft + st ) && !fadeStarted )
		{
			eaeImage.enabled = true;
			utahImage.enabled = true;
			coverImage1.CrossFadeAlpha( 0, ft, true );
			fadeStarted = true;
		}

		// Blank
		else if( timer > ( bt + 2*ft + st ) && timer < ( 2*bt + 2*ft + st ) && splash == SplashNumber.Unity )
		{
			unityImage.enabled = false;
			coverImage1.enabled = true;
			fadeStarted = false;
		}
		// Fade out Unity
		else if( timer > ( bt + ft + st ) && timer < ( bt + 2*ft + st ) && !fadeStarted )
		{
			unityImage.CrossFadeAlpha( 0, ft, true );
			fadeStarted = true;
		}
		// Show Unity
		else if( timer >= ( bt + ft ) && timer < ( bt + ft + st ) )
		{
			coverImage.enabled = false;
			splash = SplashNumber.Unity;
			fadeStarted = false;
		}
		// Fade in Unity
		else if( timer > ( bt ) && timer < ( bt + ft ) && !fadeStarted )
		{
			unityImage.enabled = true;
			coverImage.enabled = true;
			coverImage.CrossFadeAlpha( 0, ft, true );
			fadeStarted = true;
		}
	}

	private void GoToMenu()
	{
		Application.LoadLevel( Application.loadedLevel + 1 );
	}
}
