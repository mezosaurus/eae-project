using UnityEngine;
using System.Collections;

public class CycleTimer : GameBehavior
{
	public Texture2D cycleTexture;
	public Texture2D coverTexture;

	private static readonly float scaleFactor = 0.2f;
	private static readonly float topFactor = 0.01f;
	private static readonly float leftFactor = 0.45f;

	private int left = (int)(Screen.width * leftFactor);
	private int top = (int)(Screen.height * topFactor);
	private int width = (int)(Screen.height * scaleFactor);

	public bool limitTime = false;
	public float totalTime = 90.0f;
	private float timeLeft = 90.0f;

	public float dayLength = 15.0f;
	private float currentTime = 0.0f;

	public float startingAngle = -90.0f;
	public float endingAngle = 180.0f;

	void Start()
	{
		RegisterListeners();

		currentTime = startingAngle / 360 * dayLength;
	}

	void OnDestroy()
	{
		UnregisterListeners();
	}

	protected override void GameUpdate()
	{
		float deltaTime = g_currentTime - g_previousTime;

		currentTime += deltaTime;

		if( currentTime >= dayLength )
			currentTime -= dayLength;

		if( limitTime )
		{
			if( timeLeft > 0 )
			{
				timeLeft -= deltaTime;
			}
			else
			{
				TimerStatusChangedMessage message = new TimerStatusChangedMessage( TimerStatus.Completed );
				MessageCenter.Instance.Broadcast( message );

				timeLeft = 0;
			}
		}
	}

	protected void RegisterListeners() {}

	protected void UnregisterListeners() {}

	void OnGUI()
	{
		left = (int)(Screen.width * leftFactor);
		top = (int)(Screen.height * topFactor);
		width = (int)(Screen.height * scaleFactor);

		// draw the day/night cycle
		float currentAngle = 360 * currentTime / dayLength;
		GUIUtility.RotateAroundPivot( -currentAngle, new Vector2( left + width / 2, top + width / 2 ) );
		GUI.DrawTexture( new Rect( left, top, width, width ), cycleTexture );

		// draw the cover
		GUIUtility.RotateAroundPivot( currentAngle, new Vector2( left + width / 2, top + width / 2 ) );
		GUI.DrawTexture( new Rect( left, top, width, width ), coverTexture );

		Color tmpColor = GUI.color;
		float v = 1 - Mathf.Clamp( Mathf.Sin( ( 2 * Mathf.PI * currentTime ) / dayLength - Mathf.PI / 2 ) + 0.5f, 0, 1 );
		GUI.color = new Color( 1, 1, 1, v );
		GUI.Box( new Rect( 0, 0, Screen.width, Screen.height ), GUIContent.none );
		GUI.color = tmpColor;
		
		//GUI.Label( new Rect( 0, 0, 200, 50 ), currentTime.ToString() );
		//GUI.Label( new Rect( 0, 50, 200, 50 ), dayLength.ToString() );
		//GUI.Label( new Rect( 0, 100, 200, 50 ), GUI.color.ToString() );
		//GUI.Label( new Rect( 0, 150, 200, 50 ), ( currentTime % (dayLength / 2)).ToString() );
		//GUI.Label( new Rect( 0, 200, 200, 50 ), v.ToString() );
	}
}
