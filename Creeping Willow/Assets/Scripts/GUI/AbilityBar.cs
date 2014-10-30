using UnityEngine;
using System.Collections;

public class AbilityBar : MonoBehaviour
{
	public Texture2D ability1Texture = null;
	public Texture2D ability2Texture = null;
	public Texture2D ability3Texture = null;
	public Texture2D ability4Texture = null;

	public Texture2D coverTexture = null;

	public Texture2D button1Texture = null;
	public Texture2D button2Texture = null;
	public Texture2D button3Texture = null;
	public Texture2D button4Texture = null;

	public int left = 10;
	public int top = 500;
	public int width = 200;
	public int height = 50;

	private AbilityType ability1Type;
	private AbilityType ability2Type;
	private AbilityType ability3Type;
	private AbilityType ability4Type;

	private float ability1CooldownPercent = 0.0f;
	private float ability2CooldownPercent = 0.0f;
	private float ability3CooldownPercent = 0.0f;
	private float ability4CooldownPercent = 0.0f;

	private float ability1CooldownTime = 0.0f;
	private float ability2CooldownTime = 0.0f;
	private float ability3CooldownTime = 0.0f;
	private float ability4CooldownTime = 0.0f;

	void Start()
	{
		RegisterListeners();
	}

	void OnDestroy()
	{
		UnregisterListeners();
	}

	void Update()
	{
	}

	protected void RegisterListeners()
	{
		MessageCenter.Instance.RegisterListener (MessageType.AbilityCoolDownMessage, HandleAbilityCooldownMessage);
	}
	
	protected void UnregisterListeners()
	{
		MessageCenter.Instance.UnregisterListener (MessageType.AbilityCoolDownMessage, HandleAbilityCooldownMessage);
	}

	protected void HandleAbilityCooldownMessage( Message message )
	{
		AbilityCoolDownMessage mess = message as AbilityCoolDownMessage;

		if( mess.AbilityType == ability1Type )
		{
			ability1CooldownTime = mess.CoolDown - mess.TimeElapsed;
			ability1CooldownPercent = mess.TimeElapsed / mess.CoolDown;
		}
		else if( mess.AbilityType == ability2Type )
		{
			ability2CooldownTime = mess.CoolDown - mess.TimeElapsed;
			ability2CooldownPercent = mess.TimeElapsed / mess.CoolDown;
		}
		else if( mess.AbilityType == ability3Type )
		{
			ability3CooldownTime = mess.CoolDown - mess.TimeElapsed;
			ability3CooldownPercent = mess.TimeElapsed / mess.CoolDown;
		}
		else if( mess.AbilityType == ability4Type )
		{
			ability4CooldownTime = mess.CoolDown - mess.TimeElapsed;
			ability4CooldownPercent = mess.TimeElapsed / mess.CoolDown;
		}
	}

	void OnGUI()
	{
		// draw the abilities
		if( ability1Texture != null )
			GUI.DrawTexture( new Rect( left, top, width * 0.25f, height ), ability1Texture );

		if( ability2Texture != null )
			GUI.DrawTexture( new Rect( left + width * 0.25f, top, width * 0.25f, height ), ability2Texture );

		if( ability3Texture != null )
			GUI.DrawTexture( new Rect( left + width * 0.5f, top, width * 0.25f, height ), ability3Texture );

		if( ability4Texture != null )
			GUI.DrawTexture( new Rect( left + width * 0.75f, top, width * 0.25f, height ), ability4Texture );

		// draw the cooldown boxes
		if( ability1CooldownPercent > 0 )
			GUI.Box( new Rect( left, top, width * 0.25f, height * ability1CooldownPercent ), GUIContent.none );

		if( ability2CooldownPercent > 0 )
			GUI.Box( new Rect( left + width * 0.25f, top, width * 0.25f, height * ability2CooldownPercent ), GUIContent.none );

		if( ability3CooldownPercent > 0 )
			GUI.Box( new Rect( left + width * 0.5f, top, width * 0.25f, height * ability3CooldownPercent ), GUIContent.none );

		if( ability4CooldownPercent > 0 )
			GUI.Box( new Rect( left + width * 0.75f, top, width * 0.25f, height * ability4CooldownPercent ), GUIContent.none );

		// draw the cover
		if( coverTexture != null )
			GUI.DrawTexture( new Rect( left, top, width * 1.05f, height ), coverTexture );

		// draw the buttons
		if( button1Texture != null )
			GUI.DrawTexture( new Rect( left + width * 0.075f, top + height * 0.75f, width * 0.1f, height * 0.4f ), button1Texture );

		if( button1Texture != null )
			GUI.DrawTexture( new Rect( left + width * 0.325f, top + height * 0.75f, width * 0.1f, height * 0.4f ), button2Texture );

		if( button1Texture != null )
			GUI.DrawTexture( new Rect( left + width * 0.575f, top + height * 0.75f, width * 0.1f, height * 0.4f ), button3Texture );

		if( button1Texture != null )
			GUI.DrawTexture( new Rect( left + width * 0.825f, top + height * 0.75f, width * 0.1f, height * 0.4f ), button4Texture );
	}
}
