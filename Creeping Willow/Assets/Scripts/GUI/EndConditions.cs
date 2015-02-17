using UnityEngine;
using System.Collections;

public enum GameMode
{
	Survival,
	Feast,
	Timed,
}

public class EndConditions : GameBehavior
{
	private LevelLoader levelLoader;
	public GameMode gameMode;

	// Survival
	private int treesLeft;

	// Feast
	public int maxNPCsEaten;
	private int NPCsEaten;

	// Timed
	public float timeLimit;
	private float currentTime;

	void Start()
	{
		levelLoader = GameObject.FindObjectOfType<LevelLoader>();
		if( levelLoader.modeName == "Survival" )
			gameMode = GameMode.Survival;
		else if( levelLoader.modeName == "Feast" )
			gameMode = GameMode.Feast;
		else if( levelLoader.modeName == "Timed" )
			gameMode = GameMode.Timed;

		NPCsEaten = 0;
		currentTime = 0.0f;//todo

		RegisterListeners();
	}

	void OnDestroy()
	{
		UnregisterListeners();
	}

	protected void RegisterListeners()
	{
		MessageCenter.Instance.RegisterListener( MessageType.TimerStatusChanged, HandleTimerStatusChangedMessage );
		MessageCenter.Instance.RegisterListener( MessageType.NPCEaten, HandleNPCEatenMessage );
		MessageCenter.Instance.RegisterListener( MessageType.PlayerKilled, HandlePlayerKilledMessage );
	}
	
	protected void UnregisterListeners()
	{
		MessageCenter.Instance.UnregisterListener( MessageType.TimerStatusChanged, HandleTimerStatusChangedMessage );
		MessageCenter.Instance.UnregisterListener( MessageType.NPCEaten, HandleNPCEatenMessage );
		MessageCenter.Instance.UnregisterListener( MessageType.PlayerKilled, HandlePlayerKilledMessage );
	}

	protected void HandlePlayerKilledMessage( Message message )
	{
		PlayerKilledMessage mess = message as PlayerKilledMessage;

		treesLeft = CountTrees();
		if( treesLeft == 0 )
			MessageCenter.Instance.Broadcast( new LevelFinishedMessage( LevelFinishedType.Loss, LevelFinishedReason.PlayerDied, mess.NPC ) );
	}

	protected void HandleNPCEatenMessage( Message message )
	{
		NPCEatenMessage mess = message as NPCEatenMessage;

		NPCsEaten++;

		if( gameMode == GameMode.Feast && NPCsEaten >= maxNPCsEaten )
			MessageCenter.Instance.Broadcast( new LevelFinishedMessage( LevelFinishedType.Win, LevelFinishedReason.NumNPCsEaten, mess.NPC ) );
	}

	protected void HandleTimerStatusChangedMessage( Message message )
	{
		TimerStatusChangedMessage mess = message as TimerStatusChangedMessage;

		switch( mess.g_timerStatus )
		{
		case TimerStatus.Completed:
			if( gameMode == GameMode.Timed )
				MessageCenter.Instance.Broadcast( new LevelFinishedMessage( LevelFinishedType.Tie, LevelFinishedReason.TimerOut ) );

			break;

		default:
			break;
		}
	}

	int CountTrees()
	{
		int count = 0;
		return GameObject.FindObjectsOfType<PossessableTree>().Length;
	}
}
