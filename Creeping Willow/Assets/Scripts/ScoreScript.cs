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


	int _score;
	bool chain;
	int chainLength;
	readonly int chainMax = 20;

	// Use this for initialization
	void Start () {
		RegisterListeners ();
		_score = 0;
		chain = false;
		chainLength = 1;
	}
	
	// Update is called once per frame
	void Update () {
	
	}

	void addScore(int score)
	{
		_score += score;
		chain = true;
		chainLength++;
		//MessageCenter.Instance.Broadcast (new ScoreChangedMessage (score, chainLength));
	}

	int addMultiplier(int score, int multi)
	{
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
	}

	void UnregisterListeners()
	{
		MessageCenter.Instance.UnregisterListener(MessageType.LureRadiusEntered, HandleLureEntered);
		MessageCenter.Instance.UnregisterListener(MessageType.PlayerGrabbedNPCs, HandleGrabbedNPCs);
		MessageCenter.Instance.UnregisterListener(MessageType.NPCAlertLevel, HandleNPCAlertLevel);
		MessageCenter.Instance.UnregisterListener(MessageType.LureReleased, HandleLureReleased);
	}

	/*
	 * Handler Methods
	 */

	void HandleLureEntered(Message message)
	{
		LureEnteredMessage mess = message as LureEnteredMessage;

		GameObject NPC = mess.NPC;

		if( NPC.GetComponent<AIController>().getLastLure() != null && NPC.GetComponent<AIController>().getLastLure().Equals(mess.Lure) )
		{
			return;
		}

		if( mess.Lure.lurePower >= NPC.GetComponent<AIController>().lurePower )
		{
			// add score
			addScore(addMultiplier(LURED_NPC,chainLength));
			chainLength++;
		}
	}

	void HandleGrabbedNPCs(Message message)
	{
		PlayerGrabbedNPCsMessage mess = message as PlayerGrabbedNPCsMessage;

		int num = mess.NPCs.Count;

		addScore (addMultiplier (NPC_GRABBED, num));
	}

	void HandleNPCAlertLevel(Message message)
	{
		NPCAlertLevelMessage mess = message as NPCAlertLevelMessage;

		if(mess.alertLevelType == AlertLevelType.Alert)
		{
			if( chainLength > 2 )
			{
				chainLength -= 2;
				if( chainLength == 1 )
					chain = false;
			}
			else
			{
				chainLength = 1;
				chain = false;
			}
		}
		else if( mess.alertLevelType == AlertLevelType.Panic )
		{
			chainLength = 1;
			chain = false;
		}
	}

	void HandleLureReleased(Message message)
	{
		LureReleasedMessage mess = message as LureReleasedMessage;
	}
}
