using UnityEngine;
using System.Collections;

public enum BountyState
{
	BOUNTY_HIDDEN,
	BOUNTY_HIDING,
	BOUNTY_SHOWN,
	BOUNTY_SHOWING
}

public class ScoreScript : GameBehavior {

	/*
	 * SCORING SYSTEM:
	 * 
	 * (TEMPORARY)
	 * Eating an npc - 50 pts
	 * Avoiding axeman detection - 100 pts
	 * Luring an npc - 25 pts
	 * Grabbing an npc - 25
	 * 
	 */
	public readonly int NPC_EATEN = 50;
	public readonly int AVOIDED_AXEMAN = 100;
	public readonly int LURED_NPC = 25;
	public readonly int NPC_GRABBED = 25;
	public readonly int BOUNTY_EATEN = 100;

	/**
	 * Probability that next npc will be the new targeted bounty.
	 **/
	public float frequency;



	// Score GUI variables
	float offsetX = 200;
	float offsetY = 50;
	float sizeX = 150;
	float sizeY = 50;

	float popupX = 100;
	float popupY = 30;
	float popupIncrement = 0;


	
	int _score;
	int chainLength;
	readonly int chainMax = 10;
	bool scoreDisplay;
	
	int displayScore;
	int displayMultiplier;


	// Bounty GUI variables
	float bountySizeX;
	float bountyLabelSizeY;
	float bountyRectSizeY;

	int bountyState;
	float bountyIncrement = 0;

	GameObject BountyNPC;
	Texture2D BountyNPCImage;


	// Use this for initialization
	void Start () {

		if( frequency > 1 || frequency < 0 )
			frequency = .5f;

		RegisterListeners ();
		_score = 0;
		chainLength = 1;

		scoreDisplay = false;
		displayMultiplier = chainLength;
		displayScore = 0;

		bountyState = (int)BountyState.BOUNTY_HIDDEN;
		BountyNPCImage = new Texture2D (1, 1);

		// bounty
		bountySizeX = (int)Screen.width * .2f;
		bountyLabelSizeY = 30;
		bountyRectSizeY = (int)Screen.height * .2f;
	}
	
	// Update is called once per frame
	protected override void GameUpdate () {
		if( GetComponent<TreeController>().state == Tree.State.Eating )
			return;

		if( Input.GetButtonDown("RB") )
		{
			if( bountyState == (int)BountyState.BOUNTY_SHOWING || bountyState == (int)BountyState.BOUNTY_HIDING )
			{
				// Do nothing
			}
			else
			{
				if( bountyState == (int)BountyState.BOUNTY_SHOWN )
				{
					bountyState = (int)BountyState.BOUNTY_HIDING;
				}
				else if( bountyState == (int)BountyState.BOUNTY_HIDDEN )
				{
					bountyState = (int)BountyState.BOUNTY_SHOWING;
				}
			}
		}
	}

	void OnGUI()
	{
		/***** Scoring GUI *****/
		GUIStyle myStyle = new GUIStyle ();
		myStyle.fontSize = 50;

		if( scoreDisplay )
		{

			// score pop-up text
			myStyle.normal.textColor = Color.black;
			GUI.Label(new Rect(Screen.width/2 - popupX/2, Screen.height/2 - popupY/2 - popupIncrement, popupX, popupY), "" + displayScore, myStyle);
			
			// multiplier pop-up text
			myStyle.normal.textColor = Color.red;
			GUI.Label(new Rect(Screen.width/2 - popupX/2 + 25, Screen.height/2 - popupY/2 - popupIncrement + 75, popupX, popupY), "" + displayMultiplier + "x", myStyle);	


			// increment height
			popupIncrement += 1.5f;

			// check statement for pop-up statements
			if( popupIncrement > 100 )
			{
				scoreDisplay = false;
				popupIncrement = 0;
			}

  		}

		// score
		myStyle.fontSize = 30;
		myStyle.normal.textColor = Color.white;
		GUI.Box (new Rect (Screen.width-offsetX, Screen.height-offsetY, sizeX, sizeY), "SCORE: " + _score, myStyle);




		/***** Bounty GUI *****/

		// probably should use switch-case, but meh
		/*if( BountyNPC == null )
		{
			GUI.Box (new Rect(Screen.width/2-bountySizeX/2,Screen.height-bountyLabelSizeY,bountySizeX,bountyLabelSizeY), "Current Bounty (None)");
		}
		else*/ if( bountyState == (int)BountyState.BOUNTY_HIDDEN )
		{
			GUI.Box (new Rect(Screen.width/2-bountySizeX/2,Screen.height-bountyLabelSizeY,bountySizeX,bountyLabelSizeY), "Current Bounty (Right Bumper)");
			GUI.Box (new Rect(Screen.width/2-bountySizeX/2,Screen.height,bountySizeX,bountyRectSizeY), BountyNPCImage);
		}
		else if( bountyState == (int)BountyState.BOUNTY_SHOWN )
		{
			GUI.Box (new Rect(Screen.width/2-bountySizeX/2,Screen.height-bountyLabelSizeY-bountyRectSizeY,bountySizeX,bountyLabelSizeY), "Current Bounty (Right Bumper)");
			GUI.Box (new Rect(Screen.width/2-bountySizeX/2,Screen.height-bountyRectSizeY,bountySizeX,bountyRectSizeY), BountyNPCImage);
		}
		else if( bountyState == (int)BountyState.BOUNTY_SHOWING )
		{
			if( bountyIncrement >= bountyRectSizeY )
			{
				bountyState = (int)BountyState.BOUNTY_SHOWN;
				bountyIncrement = bountyRectSizeY;
			}
			else
			{
				bountyIncrement += 2f;
				GUI.Box (new Rect(Screen.width/2-bountySizeX/2,Screen.height-bountyLabelSizeY-bountyIncrement,bountySizeX,bountyLabelSizeY), "Current Bounty (Right Bumper)");
				GUI.Box (new Rect(Screen.width/2-bountySizeX/2,Screen.height-bountyIncrement,bountySizeX,bountyRectSizeY), BountyNPCImage);
			}


		}
		else if( bountyState == (int)BountyState.BOUNTY_HIDING )
		{
			if( bountyIncrement <= 0 )
			{
				bountyState = (int)BountyState.BOUNTY_HIDDEN;
				bountyIncrement = 0;
			}
			else
			{
				bountyIncrement -= 2f;
				GUI.Box (new Rect(Screen.width/2-bountySizeX/2,Screen.height-bountyLabelSizeY-bountyIncrement,bountySizeX,bountyLabelSizeY), "Current Bounty (Right Bumper)");
				GUI.Box (new Rect(Screen.width/2-bountySizeX/2,Screen.height-bountyIncrement,bountySizeX,bountyRectSizeY), BountyNPCImage);
			}

		}
		else // default - BOUNTY_HIDDEN
		{
			GUI.Box (new Rect(Screen.width/2-bountySizeX/2,Screen.height-bountyLabelSizeY,bountySizeX,bountyLabelSizeY), "Current Bounty (Right Bumper)");
			GUI.Box (new Rect(Screen.width/2-bountySizeX/2,Screen.height,bountySizeX,bountyRectSizeY), BountyNPCImage);
		}

	}

	void addScore(int score)
	{
		_score += score;
		chainLength++;
		scoreDisplay = true;
		//MessageCenter.Instance.Broadcast (new ScoreChangedMessage (score, chainLength));
	}

	int addMultiplier(int score, int multi)
	{
		displayScore = score;
		displayMultiplier = multi;

		return score * multiplier(multi);
	}

	int multiplier(int multi)
	{
		return Mathf.Min (multi, chainMax);
	}

	void invokeAudio()
	{

	}

	void OnDestroy()
	{
		UnregisterListeners ();
	}

	void RegisterListeners()
	{
		MessageCenter.Instance.RegisterListener (MessageType.LureRadiusEntered, HandleLureEntered);
		MessageCenter.Instance.RegisterListener (MessageType.PlayerGrabbedNPCs, HandleGrabbedNPCs);
		MessageCenter.Instance.RegisterListener (MessageType.NPCAlertLevel, HandleNPCAlertLevel);
		MessageCenter.Instance.RegisterListener (MessageType.LureReleased, HandleLureReleased);
		MessageCenter.Instance.RegisterListener (MessageType.NPCEaten, HandleNPCEaten);
		MessageCenter.Instance.RegisterListener (MessageType.NPCCreated, HandleNPCCreated);
	}

	void UnregisterListeners()
	{
		MessageCenter.Instance.UnregisterListener(MessageType.LureRadiusEntered, HandleLureEntered);
		MessageCenter.Instance.UnregisterListener(MessageType.PlayerGrabbedNPCs, HandleGrabbedNPCs);
		MessageCenter.Instance.UnregisterListener(MessageType.NPCAlertLevel, HandleNPCAlertLevel);
		MessageCenter.Instance.UnregisterListener(MessageType.LureReleased, HandleLureReleased);
		MessageCenter.Instance.UnregisterListener(MessageType.NPCEaten, HandleNPCEaten);
		MessageCenter.Instance.UnregisterListener (MessageType.NPCCreated, HandleNPCCreated);

	}

	/*
	 * Handler Methods
	 */

	void HandleLureEntered(Message message)
	{
		LureEnteredMessage mess = message as LureEnteredMessage;

		GameObject NPC = mess.NPC;

		if( NPC.GetComponent<AIController>() as AIController == null )
			return;

		if( NPC.GetComponent<AIController>().getLastLure() != null && NPC.GetComponent<AIController>().getLastLure().Equals(mess.Lure) )
		{
			if( NPC.GetComponent<AIController>().getLastLure().Equals(mess.Lure) )
				return;
		}

		if( mess.Lure.lurePower >= NPC.GetComponent<AIController>().lurePower )
		{
			// add score
			addScore(addMultiplier(LURED_NPC,chainLength));
		}
	}

	void HandleGrabbedNPCs(Message message)
	{
		PlayerGrabbedNPCsMessage mess = message as PlayerGrabbedNPCsMessage;

		int num = mess.NPCs.Count;

		addScore (addMultiplier (NPC_GRABBED, chainLength));

		bountyState = (int)BountyState.BOUNTY_HIDDEN;
	}

	void HandleNPCAlertLevel(Message message)
	{
		NPCAlertLevelMessage mess = message as NPCAlertLevelMessage;

		if(mess.alertLevelType == AlertLevelType.Alert)
		{
			if( chainLength > 2 )
			{
				chainLength -= 2;
			}
			else
			{
				chainLength = 1;
			}
		}
		else if( mess.alertLevelType == AlertLevelType.Panic )
		{
			chainLength = 1;
		}
	}

	void HandleLureReleased(Message message)
	{
		LureReleasedMessage mess = message as LureReleasedMessage;
	}

	void HandleNPCEaten(Message message)
	{
		NPCEatenMessage mess = message as NPCEatenMessage;

		if( BountyNPC.Equals(mess.NPC) )
		{
			addScore (addMultiplier (BOUNTY_EATEN, chainLength));
			BountyNPC = null;
			BountyNPCImage = new Texture2D(1,1);
		}
		else
			addScore (addMultiplier (NPC_EATEN, chainLength));
	}

	void HandleNPCCreated(Message message)
	{
		if( BountyNPC != null || GetComponent<TreeController>().state == Tree.State.Eating)
			return;

		if( Random.value > frequency )
			return;

		NPCCreatedMessage mess = message as NPCCreatedMessage;

		BountyNPC = mess.NPC;

		Sprite sprite = mess.NPC.GetComponent<SpriteRenderer> ().sprite;
		Color[] pixels = sprite.texture.GetPixels (
			(int)sprite.textureRect.x, 
			(int)sprite.textureRect.y, 
			(int)sprite.textureRect.width, 
			(int)sprite.textureRect.height
			);

		BountyNPCImage = new Texture2D ((int)sprite.rect.width, (int)sprite.rect.height);

		BountyNPCImage.SetPixels (pixels);
		BountyNPCImage.Apply ();
		
		//BountyNPCImage = mess.NPC.GetComponent<SpriteRenderer> ().sprite.texture;
		
		bountyState = (int)BountyState.BOUNTY_SHOWING;
	}
}
