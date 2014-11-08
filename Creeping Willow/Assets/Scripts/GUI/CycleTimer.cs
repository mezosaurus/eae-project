using UnityEngine;
using System.Collections;

public class CycleTimer : GameBehavior
{
	public Texture2D back;
	public Texture2D cover;

	public int left = 10;
	public int top = 10;
	public int width = 50;
	public int height = 50;

	private float prevTime;
	private float currentTime;
	public float totalTime = 90.0f;

	public float startingAngle = -90.0f;
	public float endingAngle = 180.0f;

	void Start()
	{
		currentTime = totalTime;
		RegisterListeners();
	}

	void OnDestroy()
	{
		UnregisterListeners();
	}

	void Update()
	{
		if (paused)
		{
			prevTime = Time.time;
			return;
		}

		if( currentTime > 0 )
		{
			currentTime -= Time.time - prevTime;
			prevTime = Time.time;
		}
		else
		{
			TimerStatusChangedMessage message = new TimerStatusChangedMessage( TimerStatus.Completed );
			MessageCenter.Instance.Broadcast( message );

			currentTime = 0;
		}
	}

	protected void RegisterListeners()
	{
		MessageCenter.Instance.RegisterListener( MessageType.TimerStatusChanged, HandleTimerStatusChanged );
	}

	protected void UnregisterListeners()
	{
		MessageCenter.Instance.UnregisterListener( MessageType.TimerStatusChanged, HandleTimerStatusChanged );
	}

	protected void HandleTimerStatusChanged( Message message )
	{
		TimerStatusChangedMessage mess = message as TimerStatusChangedMessage;

		switch( mess.g_timerStatus )
		{
		case TimerStatus.Resume:
			prevTime = Time.time;
			break;

		case TimerStatus.Pause:
		case TimerStatus.Completed:
			break;
		}
	}

	void OnGUI()
	{
		// draw the day/night cycle
		Matrix4x4 oldMatrix = GUI.matrix;
		float currentAngle = 360 - ( startingAngle + ( totalTime - currentTime ) / totalTime * ( endingAngle - startingAngle ) );
		GUIUtility.RotateAroundPivot( currentAngle, new Vector2( left + width / 2, top + height / 2 ) );
		GUI.DrawTexture( new Rect( left, top, width, height ), back );
		GUI.matrix = oldMatrix;

		// draw the cover
		GUI.DrawTexture( new Rect( left, top, width, height ), cover );
	}
}
