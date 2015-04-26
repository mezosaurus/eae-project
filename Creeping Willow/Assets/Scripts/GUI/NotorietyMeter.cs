using UnityEngine;
using System.Collections;

public class NotorietyMeter : MonoBehaviour
{
	public float notorietyMax = 75.0f;
	private float notoriety = 0.0f;

	private int axemanCount = 0;
	public Texture2D axemanHeadTexture;
	public Texture2D angryAxemanHeadTexture;

	private float width;
	private float height;
	private float top;
	private float left;

	public Texture2D bar1;
	public Texture2D bar2;
	public Texture2D bar3;
	public Texture2D bar4;
	public Texture2D bar5;
	public Texture2D bar6;
	public Texture2D bar7;
	public Texture2D bar8;
	public Texture2D bar9;
	public Texture2D bar10;
	public Texture2D bar11;
	public Texture2D bar12;
	public Texture2D bar13;
	public Texture2D bar14;
	public Texture2D bar15;
	public Texture2D bar16;
	public Texture2D bar17;
	public Texture2D bar18;
	public Texture2D bar19;
	public Texture2D bar20;
	public Texture2D bar21;
	public Texture2D bar22;

	int angerCount = 0;


	
	// gui variables
	float startX = 30;
	float startY = 10;
	float xWidth = 50;
	float yHeight = 50;


	void Start()
	{
		RegisterListeners();

		startX = Screen.width * startX / 1440;
		xWidth = Screen.width * xWidth / 1440;
		startY = Screen.height * startY / 742;
		yHeight = Screen.height * yHeight / 742;

		width = Screen.width / 4;
		height = Screen.height / 10;
		
		//top = GlobalGameStateManager.originalHeight / 2 - height / 2;
		top = 10;
		left = 170;
	}

	void OnDestroy()
	{
		UnregisterListeners();
	}

	protected void RegisterListeners()
	{
		MessageCenter.Instance.RegisterListener( MessageType.NPCPanickedOffMap, HandleNPCPanickedMessage );
		MessageCenter.Instance.RegisterListener( MessageType.AxeManAngerChanged, HandleAngerChanged );
	}
	
	protected void UnregisterListeners()
	{
		MessageCenter.Instance.UnregisterListener( MessageType.NPCPanickedOffMap, HandleNPCPanickedMessage);
		MessageCenter.Instance.UnregisterListener( MessageType.AxeManAngerChanged, HandleAngerChanged );
	}

	protected void HandleNPCPanickedMessage( Message message )
	{
		NPCPanickedOffMapMessage mess = message as NPCPanickedOffMapMessage;

		notoriety += 30.0f;
		
		if( notoriety > notorietyMax )
		{
			MessageCenter.Instance.Broadcast( new NotorietyMaxedMessage( mess.PanickedPosition ) );
			notoriety = notorietyMax;
		}
	}

	// pulsating axeman head variables
	bool pulsating = false;
	bool increasingPulse = true;
	float currentPulse = 0;
	float currentPulseTime = 0;
	float pulseMax = 20;
	float pulseIncrement = .5f;
	float pulseTime = 200;

	protected void HandleAngerChanged( Message message )
	{
		AxeManAngerChangedMessage mess = message as AxeManAngerChangedMessage;

		if( mess.Angry )
		{
			angerCount++;
			pulsating = true;
		}
		else
			angerCount--;
	}

	Texture2D getNotorietyBarImage(float num, float total)
	{
		//Debug.Log ("num image: " + num);
		
		switch((int)Mathf.Round((num/total)*21))
		{
		case 0:
			return bar22;
		case 1:
			return bar21;
		case 2:
			return bar20;
		case 3:
			return bar19;
		case 4:
			return bar18;
		case 5:
			return bar17;
		case 6:
			return bar16;
		case 7:
			return bar15;
		case 8:
			return bar14;
		case 9:
			return bar13;
		case 10:
			return bar12;
		case 11:
			return bar11;
		case 12:
			return bar10;
		case 13:
			return bar9;
		case 14:
			return bar8;
		case 15:
			return bar7;
		case 16:
			return bar6;
		case 17:
			return bar5;
		case 18:
			return bar4;
		case 19:
			return bar3;
		case 20:
			return bar2;
		case 21:
			return bar1;
		default:
			return bar22;
		}
		
	}
	
	void Update()
	{
	}

	void OnGUI ()
	{
		//FontConverter.instance.parseStringToTextures (20, top + height / 4, 15, Screen.height / 20, "notoriety");

		if( pulsating )
		{
			if( angerCount == 0 )//currentPulseTime >= pulseTime )
			{
				pulsating = false;
				currentPulseTime = 0;
				currentPulse = 0;
			}
			else
			{
				currentPulseTime += pulseIncrement; // update time left for pulse

				// if max range of pulse is reached
				if( currentPulse >= pulseMax && increasingPulse || currentPulse <= 0 && !increasingPulse )
					increasingPulse = !increasingPulse;

				// update current pulse
				if( increasingPulse )
					currentPulse += pulseIncrement;
				else
					currentPulse -= pulseIncrement;
				
			}
		}

		// axeman
		if( angerCount == 0 ) // if angry
			GUI.DrawTexture( new Rect(startX - .5f * currentPulse, startY - .5f * currentPulse, xWidth + currentPulse, yHeight + currentPulse), axemanHeadTexture );
		else
			GUI.DrawTexture( new Rect(startX - .5f * currentPulse, startY - .5f * currentPulse, xWidth + currentPulse, yHeight + currentPulse), angryAxemanHeadTexture );

		axemanCount = CountAxemen();

		FontConverter.instance.rightAnchorParseStringToTextures (startX + xWidth + startX * 2, startY * 1.5f, Screen.width * 30f / 1440f, Screen.height * 50f / 742f, axemanCount + "x");

		// count texture

		//GUI.matrix = GlobalGameStateManager.PrepareMatrix();


		//float percentFull = notoriety / notorietyMax;

		//GUI.Box( new Rect( left, top, width, height ), GUIContent.none );
		//GUI.Box( new Rect( left, top + height * ( 1 - percentFull ), width, height * percentFull ), GUIContent.none );
		//GUI.DrawTexture (new Rect (left, top, width, height), getNotorietyBarImage(notoriety,notorietyMax));


        // Matt changed this, feel free to change back
        /*float w = height / 2;
        float h = w;
		float t = top + (height / 2f) - ((axemanCount * h) / 2f);*/
		
		//GUI.matrix = Matrix4x4.identity;

		/*axemanCount = CountAxemen();
		float offset = 10;
		for( int i = 0; i < axemanCount; i++ )
		{
			//GUI.DrawTexture( new Rect( left + width, t + (i * h), w, h), axemanHeadTexture, ScaleMode.ScaleToFit );

			GUI.DrawTexture( new Rect(left + width + offset,10 + height / 4,w,h), axemanHeadTexture, ScaleMode.ScaleToFit );
			offset += w + 10;
		}*/
	}

	private int CountAxemen()
	{
		return GameObject.FindObjectsOfType<EnemyAIController>().Length;
	}
}
