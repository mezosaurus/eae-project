using UnityEngine;
using System.Collections;

public class ScoreScript : MonoBehaviour {

	/*
	 * SCORING SYSTEM:
	 * 
	 * (TEMPORARY)
	 * Eating an npc - 50 pts
	 * Avoiding axeman detection - 100 pts
	 * Luring an npc - 25 pts
	 * 
	 * 
	 */
	public static int NPC_EATEN = 50;
	public static int AVOIDED_AXEMAN = 100;
	public static int LURED_NPC = 25;
	public static int NPC_GRABBED = 25;


	// GUI variables
	float offsetX = 200;
	float offsetY = 50;
	float sizeX = 150;
	float sizeY = 50;

	float popupX = 100;
	float popupY = 30;
	float popupIncrement = 0;


	int _score;
	int chainLength;
	readonly int chainMax = 20;
	bool scoreDisplay;

	int displayScore;
	int displayMultiplier;


	// Use this for initialization
	void Start () {
		RegisterListeners ();
		_score = 0;
		chainLength = 1;

		scoreDisplay = false;
		displayMultiplier = chainLength;
		displayScore = 0;
	}
	
	// Update is called once per frame
	void Update () {
	
	}

	void OnGUI()
	{
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
		MessageCenter.Instance.RegisterListener (MessageType.NPCDestroyed, HandleNPCDestroyed);
	}

	void UnregisterListeners()
	{
		MessageCenter.Instance.UnregisterListener(MessageType.LureRadiusEntered, HandleLureEntered);
		MessageCenter.Instance.UnregisterListener(MessageType.PlayerGrabbedNPCs, HandleGrabbedNPCs);
		MessageCenter.Instance.UnregisterListener(MessageType.NPCAlertLevel, HandleNPCAlertLevel);
		MessageCenter.Instance.UnregisterListener(MessageType.LureReleased, HandleLureReleased);
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
			// add score
			addScore(addMultiplier(LURED_NPC,chainLength));
		}
	}

	void HandleGrabbedNPCs(Message message)
	{
		PlayerGrabbedNPCsMessage mess = message as PlayerGrabbedNPCsMessage;

		int num = mess.NPCs.Count;

		addScore (addMultiplier (NPC_GRABBED, chainLength));
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

	void HandleNPCDestroyed(Message message)
	{
		NPCDestroyedMessage mess = message as NPCDestroyedMessage;

		//addScore (addMultiplier (NPC_EATEN, chainLength));
	}
}
