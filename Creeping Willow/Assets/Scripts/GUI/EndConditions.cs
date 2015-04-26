using UnityEngine;
using System.Collections;

public enum GameMode
{
	Survival,
	Marked,
	Feast,
	Timed,
}

public class EndConditions : GameBehavior
{
	private LevelLoader levelLoader;
	private ScoreScript scoreScript;
	private GameMode gameMode;

	// Survival
	private int treesLeft;

	// Feast
	public int maxNPCsEatenFeast = 10;
	private int NPCsEaten = 0;

	// Marked
	public int maxBountiesDestroyed = 5;
	private int bountiesDestroyed = 0;

	void Start()
	{
		levelLoader = GameObject.FindObjectOfType<LevelLoader>();
		scoreScript = GameObject.FindObjectOfType<ScoreScript>();

		if( levelLoader.modeName == "Survival" )
			gameMode = GameMode.Survival;
		if( levelLoader.modeName == "Marked" )
			gameMode = GameMode.Marked;
		else if( levelLoader.modeName == "Feast" )
			gameMode = GameMode.Feast;
		else if( levelLoader.modeName == "Timed" )
			gameMode = GameMode.Timed;

		RegisterListeners();
	}

	void OnDestroy()
	{
		UnregisterListeners();
	}

	protected void RegisterListeners()
	{
		MessageCenter.Instance.RegisterListener( MessageType.NPCEaten, HandleNPCEatenMessage );
		MessageCenter.Instance.RegisterListener( MessageType.PlayerKilled, HandlePlayerKilledMessage );
		MessageCenter.Instance.RegisterListener( MessageType.MarkedBountyDestroyed, HandleMarkedBountyDestroyedMessage );
	}
	
	protected void UnregisterListeners()
	{
		MessageCenter.Instance.UnregisterListener( MessageType.NPCEaten, HandleNPCEatenMessage );
		MessageCenter.Instance.UnregisterListener( MessageType.PlayerKilled, HandlePlayerKilledMessage );
		MessageCenter.Instance.UnregisterListener( MessageType.MarkedBountyDestroyed, HandleMarkedBountyDestroyedMessage );
	}

	protected void HandleMarkedBountyDestroyedMessage( Message message )
	{
		MarkedBountyDestroyedMessage mess = message as MarkedBountyDestroyedMessage;

		bountiesDestroyed++;

		if( gameMode == GameMode.Marked && bountiesDestroyed >= maxBountiesDestroyed )
			MessageCenter.Instance.Broadcast( new LevelFinishedMessage( LevelFinishedType.Win, LevelFinishedReason.NumNPCsEaten ) );
	}

	protected void HandlePlayerKilledMessage( Message message )
	{
		PlayerKilledMessage mess = message as PlayerKilledMessage;

		treesLeft = CountTrees();

		if( treesLeft <= 0 )
			MessageCenter.Instance.Broadcast( new LevelFinishedMessage( LevelFinishedType.Loss, LevelFinishedReason.PlayerDied, mess.NPC ) );
	}

	protected void HandleNPCEatenMessage( Message message )
	{
		NPCEatenMessage mess = message as NPCEatenMessage;

		// ignore critters
		if( mess.NPC.GetComponent<CritterController>() != null || mess.NPC == null )
			return;

		NPCsEaten++;

		if( gameMode == GameMode.Feast && NPCsEaten >= maxNPCsEatenFeast )
			MessageCenter.Instance.Broadcast( new LevelFinishedMessage( LevelFinishedType.Win, LevelFinishedReason.NumNPCsEaten, mess.NPC ) );
	}

	int CountTrees()
	{
		int count = 0;
		return GameObject.FindObjectsOfType<PossessableTree>().Length;
	}

	public GameMode GameMode
	{
		get
		{
			return gameMode;
		}
	}
}
