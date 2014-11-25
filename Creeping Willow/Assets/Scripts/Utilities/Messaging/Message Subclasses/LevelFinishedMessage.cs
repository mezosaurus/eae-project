using UnityEngine;

public enum LevelFinishedType
{
	Win,
	Loss,
	Tie,
}

public enum LevelFinishedReason
{
	// Wins
	TargetNPCEaten,
	NumNPCsEaten,

	// Losses
	PlayerDied,
	MaxNPCsPanicked,

	// Either
	TimerOut,
}

public class LevelFinishedMessage : Message
{
	public LevelFinishedType Type;
	public LevelFinishedReason Reason;
	public GameObject NPC;
	
	public LevelFinishedMessage( LevelFinishedType i_levelFinishedType, LevelFinishedReason i_levelFinishedReason ) : base( MessageType.LevelFinished )
	{
		Type = i_levelFinishedType;
		Reason = i_levelFinishedReason;
		NPC = null;
	}

	public LevelFinishedMessage( LevelFinishedType i_levelFinishedType, LevelFinishedReason i_levelFinishedReason, GameObject i_NPC ) : base( MessageType.LevelFinished )
	{
		Type = i_levelFinishedType;
		Reason = i_levelFinishedReason;
		NPC = i_NPC;
	}
}
