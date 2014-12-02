using UnityEngine;
using System.Collections;
using System.Collections.Generic;

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

	// multipliers
	public readonly int stealthMultiplier = 6;
	public readonly int droppingFliesMultiplier = 3;
	public readonly int lureMultiplier = 2;

	// output strings
	readonly string npcEatenString = "You ate a person";
	readonly string bountyEatenString = "You ate your target";
	readonly string npcGrabbedString = "Person grabbed";
	readonly string npcLuredString = "Person lured";

	readonly string stealthString = " using stealth";
	readonly string streakString = " within 15 seconds of last person eaten";
	readonly string luredString = " that was lured";

	float timeSinceLastKill = -1;
	float lastKillTime = -100;
	readonly float lastKillBonus = 30;

	/**
	 * Probability that next npc will be the new targeted bounty.
	 **/
	public float frequency;


	bool luresInUse = false;
	int luresInUseCount = 0;
	ArrayList luredNPCs = new ArrayList();

	// Dictionary of all npcs
	Dictionary<GameObject,bool> npcsAlerted = new Dictionary<GameObject,bool>();


	//Queue scoreQueue = new Queue();



	// Score GUI variables
	float offsetX = 200;
	float offsetY = 50;
	float sizeX = 150;
	float sizeY = 50;

	float popupX = 100;
	float popupY = 30;
	float popupIncrement = 0;
	readonly float popupIncrementMax = 100;
	float popupAlpha;
	
	int _score;
	int chainLength;
	readonly int chainMax = 20;
	bool scoreDisplay;
	
	int displayScore;
	int displayMultiplier;
	string displayScoreString;


	// Bounty GUI variables
	float bountySizeX;
	float bountyLabelSizeY;
	float bountyRectSizeY;

	int bountyState;
	float bountyIncrement = 0;

	GameObject BountyNPC;
	Texture2D BountyNPCImage;

	// when bounty is given
	bool bountyRaised = false;
	float bountyRaiseTime = -1;
	bool bountyRaiseSetup = true;


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
		timeSinceLastKill = Time.time - lastKillTime;

		// used for uncalled showing of target for player
		if( bountyRaiseSetup )
		{
			float bountyTime = Time.time;

			if( bountyRaised && bountyState == (int)BountyState.BOUNTY_SHOWN )
			{
				bountyRaised = false;
				bountyRaiseTime = Time.time;
			}
			
			if( bountyTime > bountyRaiseTime + 3 && bountyRaiseTime > 0 )
			{
				bountyState = (int)BountyState.BOUNTY_HIDING;
				bountyRaiseSetup = false;
			}
		}


		// ignore inputs
		if( GetComponent<TreeController>().state == Tree.State.Eating )
			return;

		// change state of bountyDisplay
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
		myStyle.alignment = TextAnchor.MiddleCenter;
		myStyle.fontSize = 50;

		Color guiColor = GUI.color;
		Color savedGuiColor = GUI.color;

		if( scoreDisplay )
		{
			// change alpha/transparency of score
			if( popupIncrement <= .4f * popupIncrementMax )
			{
				popupAlpha = GUI.color.a * popupIncrement / (.4f * popupIncrementMax );
				guiColor.a = popupAlpha;
				GUI.color = guiColor;
			}
			else if( popupIncrement >= .6f * popupIncrementMax )
			{
				popupAlpha = GUI.color.a * ( popupIncrementMax - popupIncrement ) / (.4f * popupIncrementMax );
				guiColor.a = popupAlpha;
				GUI.color = guiColor;
			}

			// score pop-up text
			myStyle.normal.textColor = Color.black;
			GUI.Label(new Rect(Screen.width/2 - popupX/2, Screen.height/2 - popupY/2 - popupIncrement, popupX, popupY), "" + displayScore, myStyle);
			
			// multiplier pop-up text
			myStyle.normal.textColor = Color.red;
			GUI.Label(new Rect(Screen.width/2 - popupX/2 + 25, Screen.height/2 - popupY/2 - popupIncrement + 75, popupX, popupY), "" + displayMultiplier + "x", myStyle);	


			// increment height
			popupIncrement += 1.5f;

			// check statement for pop-up statements
			if( popupIncrement > popupIncrementMax )
			{
				scoreDisplay = false;
				popupIncrement = 0;
			}

			
			GUI.color = savedGuiColor; // revert to previous alpha
			
			// score reason string
			myStyle.fontSize = 30;
			myStyle.normal.textColor = Color.white;
			GUI.Label(new Rect(Screen.width/2, Screen.height/2 - popupY/2 + 150, popupX, popupY), "" + displayScoreString, myStyle);

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

	// add a score
	void addScore(int score)
	{
		_score += score;
		//chainLength++;
		scoreDisplay = true;
		//MessageCenter.Instance.Broadcast (new ScoreChangedMessage (score, chainLength));
	}

	// add a multiplier to the score
	int addMultiplier(int score, int multi)
	{
		displayScore = score;
		displayMultiplier = multiplier(multi);


		return score * multiplier(multi);
	}

	// determine actual multiplier
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
		MessageCenter.Instance.RegisterListener (MessageType.NPCDestroyed, HandleNPCDestroyed);
	}

	void UnregisterListeners()
	{
		MessageCenter.Instance.UnregisterListener(MessageType.LureRadiusEntered, HandleLureEntered);
		MessageCenter.Instance.UnregisterListener(MessageType.PlayerGrabbedNPCs, HandleGrabbedNPCs);
		MessageCenter.Instance.UnregisterListener(MessageType.NPCAlertLevel, HandleNPCAlertLevel);
		MessageCenter.Instance.UnregisterListener(MessageType.LureReleased, HandleLureReleased);
		MessageCenter.Instance.UnregisterListener(MessageType.NPCEaten, HandleNPCEaten);
		MessageCenter.Instance.UnregisterListener(MessageType.NPCCreated, HandleNPCCreated);
		MessageCenter.Instance.UnregisterListener(MessageType.NPCDestroyed, HandleNPCDestroyed);

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
			luresInUseCount++;
			luresInUse = true;

			// add score
			addScore(addMultiplier(LURED_NPC,1));
			displayScoreString = npcLuredString;
		}

		luredNPCs.Add (NPC);
	}





	void HandleGrabbedNPCs(Message message)
	{
		PlayerGrabbedNPCsMessage mess = message as PlayerGrabbedNPCsMessage;

		int num = mess.NPCs.Count;

		addScore (addMultiplier (NPC_GRABBED, 1));
		displayScoreString = npcGrabbedString;

		bountyState = (int)BountyState.BOUNTY_HIDDEN;
	}





	void HandleNPCAlertLevel(Message message)
	{
		NPCAlertLevelMessage mess = message as NPCAlertLevelMessage;

		if(mess.alertLevelType == AlertLevelType.Alert)
		{
			chainLength = 1;
			npcsAlerted.Remove(mess.NPC);
			npcsAlerted.Add(mess.NPC,true);
			/*if( chainLength > 2 )
			{
				chainLength -= 2;
			}
			else
			{
				chainLength = 1;
			}*/
		}
		else if( mess.alertLevelType == AlertLevelType.Panic )
		{
			chainLength = 1;
			npcsAlerted.Remove(mess.NPC);
			npcsAlerted.Add(mess.NPC,true);
		}
	}






	void HandleLureReleased(Message message)
	{
		LureReleasedMessage mess = message as LureReleasedMessage;

		GameObject NPC = mess.NPC;

		luredNPCs.Remove (NPC);

		luresInUseCount--;

		if( luresInUseCount == 0 )
			luresInUse = false;
	}





	void HandleNPCDestroyed(Message message)
	{
		NPCDestroyedMessage mess = message as NPCDestroyedMessage;

		if( npcsAlerted.ContainsKey(mess.NPC) )
			npcsAlerted.Remove(mess.NPC);
	}





	void HandleNPCEaten(Message message)
	{
		NPCEatenMessage mess = message as NPCEatenMessage;

		if( BountyNPC == null || mess.NPC == null )
			return;

		if( BountyNPC.Equals(mess.NPC) )
		{
			if( timeSinceLastKill < 15 )
			{
				addScore (addMultiplier (BOUNTY_EATEN, droppingFliesMultiplier * chainLength));
				displayScoreString = bountyEatenString + streakString;
			}
			else
			{
				addScore (addMultiplier (BOUNTY_EATEN, chainLength));
				displayScoreString = bountyEatenString;
			}

			BountyNPC = null;
			BountyNPCImage = new Texture2D(1,1);
		}
		else
		{
			if( timeSinceLastKill < 15 )
			{
				addScore (addMultiplier (NPC_EATEN, droppingFliesMultiplier * chainLength));
				displayScoreString = npcEatenString + streakString;
			}
			else
			{
				addScore (addMultiplier (NPC_EATEN, chainLength));
				displayScoreString = npcEatenString;
			}
		}

		if( luredNPCs.Contains(mess.NPC) )
		{
			luredNPCs.Remove(mess.NPC);
			addScore(addMultiplier(LURED_NPC, lureMultiplier));
			displayScoreString += luredString;
		}

		lastKillTime = Time.time;
		chainLength++;

		if( !npcsAlerted.ContainsValue(true) )
		{
			npcsAlerted.Remove(mess.NPC);
			displayScoreString += stealthString;
		}
		if( npcsAlerted.ContainsKey(mess.NPC) )
		{
			npcsAlerted.Remove(mess.NPC);
		}
	}





	void HandleNPCCreated(Message message)
	{
		NPCCreatedMessage mess = message as NPCCreatedMessage;

		if( BountyNPC != null || GetComponent<TreeController>().state == Tree.State.Eating)
			return;

		npcsAlerted.Add (mess.NPC, false);

		// bounty/target
		if( Random.value > frequency )
			return;

		BountyNPC = mess.NPC;

		// used to convert from sprite sheet to current sprite
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
		bountyRaised = true;
	}
}
