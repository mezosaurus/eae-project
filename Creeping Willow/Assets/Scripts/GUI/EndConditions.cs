using UnityEngine;
using System.Collections;

public class EndConditions : MonoBehaviour
{
	// Wins
	public int WinConditionsRequired;
	private int WinConditionsAchieved;

	public GameObject TargetNPC;
	public bool UseTargetNPC;

	public int NumNPCsEaten;
	public bool UseNPCsEaten;
	private int NPCsEaten;

	// Losses
	public int LossConditionsRequired;
	private int LossConditionsAchieved;

	public bool UsePlayerDied;

	public int MaxNPCsPanicked;
	public bool UseMaxNPCsPanicked;
	private int NPCsPanicked;

	// Either
	public bool TimeOutIsLoss;
	public bool UseTimerOut;

	void Start()
	{
		WinConditionsAchieved = 0;
		LossConditionsAchieved = 0;

		NPCsEaten = 0;
		NPCsPanicked = 0;

		RegisterListeners();
	}

	void OnDestroy()
	{
		UnregisterListeners();
	}

	protected void RegisterListeners()
	{
		MessageCenter.Instance.RegisterListener( MessageType.NPCAlertLevel, HandleNPCAlertMessage );
		MessageCenter.Instance.RegisterListener( MessageType.TimerStatusChanged, HandleTimerStatusChangedMessage );
		MessageCenter.Instance.RegisterListener( MessageType.NPCEaten, HandleNPCEatenMessage );
	}
	
	protected void UnregisterListeners()
	{
		MessageCenter.Instance.UnregisterListener( MessageType.NPCAlertLevel, HandleNPCAlertMessage );
		MessageCenter.Instance.UnregisterListener( MessageType.TimerStatusChanged, HandleTimerStatusChangedMessage );
		MessageCenter.Instance.UnregisterListener( MessageType.NPCEaten, HandleNPCEatenMessage );
	}

	protected void HandleNPCEatenMessage( Message message )
	{
		NPCEatenMessage mess = message as NPCEatenMessage;
		NPCsEaten++;
		
		//if (mess.eaten)
			NPCsEaten++;
		
		if( UseTargetNPC && mess.NPC == TargetNPC )
			AddWin( LevelFinishedReason.TargetNPCEaten, null );
		else if( UseNPCsEaten && NPCsEaten == NumNPCsEaten )
			AddWin( LevelFinishedReason.NumNPCsEaten, null );
	}

	protected void HandleTimerStatusChangedMessage( Message message )
	{
		if( UseTimerOut )
		{
			TimerStatusChangedMessage mess = message as TimerStatusChangedMessage;

			switch( mess.g_timerStatus )
			{
			case TimerStatus.Completed:
				if( TimeOutIsLoss )
					AddFail( LevelFinishedReason.TimerOut, null );
				else
					AddWin( LevelFinishedReason.TimerOut, null );

				break;

			default:
				break;
			}
		}
	}

	private void AddFail( LevelFinishedReason i_reason, GameObject i_NPC )
	{
		LossConditionsAchieved++;

		if( LossConditionsAchieved >= LossConditionsRequired )
		{
			MessageCenter.Instance.Broadcast( new LevelFinishedMessage( LevelFinishedType.Loss, i_reason, i_NPC ) );
		}
	}

	private void AddWin( LevelFinishedReason i_reason, GameObject i_NPC )
	{
		WinConditionsAchieved++;
		
		if( WinConditionsAchieved >= WinConditionsRequired )
		{
			MessageCenter.Instance.Broadcast( new LevelFinishedMessage( LevelFinishedType.Win, i_reason, i_NPC ) );
		}
	}

	protected void HandleNPCAlertMessage( Message message )
	{
		if( UseMaxNPCsPanicked )
		{
			NPCAlertLevelMessage mess = message as NPCAlertLevelMessage;

			switch( mess.alertLevelType )
			{
			case AlertLevelType.Panic:
				NPCsPanicked++;

				if( NPCsPanicked == MaxNPCsPanicked )
				{
					AddFail( LevelFinishedReason.MaxNPCsPanicked, mess.NPC );
				}
				break;
				
			default:
				break;
			}
		}
	}

	private void update()
	{

	}

	void Update()
	{
	}
}
