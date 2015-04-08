using UnityEngine;
using System.Collections;

public class NotorietyMeter : MonoBehaviour
{
	public float notorietyMax = 75.0f;
	private float notoriety = 0.0f;

	private int axemanCount = 0;
	public Texture2D axemanHeadTexture;

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


	void Start()
	{
		RegisterListeners();

		width = 160;
		height = 800;
		
		top = GlobalGameStateManager.originalHeight / 2 - height / 2;
		left = 80;
	}

	void OnDestroy()
	{
		UnregisterListeners();
	}

	protected void RegisterListeners()
	{
		MessageCenter.Instance.RegisterListener( MessageType.NPCPanickedOffMap, HandleNPCPanickedMessage );
	}
	
	protected void UnregisterListeners()
	{
		MessageCenter.Instance.UnregisterListener( MessageType.NPCPanickedOffMap, HandleNPCPanickedMessage);
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
		GUI.matrix = GlobalGameStateManager.PrepareMatrix();

		float percentFull = notoriety / notorietyMax;

		//GUI.Box( new Rect( left, top, width, height ), GUIContent.none );
		//GUI.Box( new Rect( left, top + height * ( 1 - percentFull ), width, height * percentFull ), GUIContent.none );
		GUI.DrawTexture (new Rect (left, top, width, height), getNotorietyBarImage(notoriety,notorietyMax));

        // Matt changed this, feel free to change back
        float w = 80f;
        float h = w;
        float t = top + (height / 2f) - ((axemanCount * h) / 2f);

		axemanCount = CountAxemen();

		for( int i = 0; i < axemanCount; i++ )
		{
			GUI.DrawTexture( new Rect( left + width, t + (i * h), w, h), axemanHeadTexture, ScaleMode.ScaleToFit );
		}

		GUI.matrix = Matrix4x4.identity;
	}

	private int CountAxemen()
	{
		return GameObject.FindObjectsOfType<EnemyAIController>().Length;
	}
}
