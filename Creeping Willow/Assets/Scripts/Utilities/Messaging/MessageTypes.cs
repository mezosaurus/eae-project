public enum MessageType
{
    AbilityStatusChanged,
    PlayerMovementTypeChanged,
    PlayerGrabbedNPCs,
    PlayerReleasedNPCs,
	TimerStatusChanged,
	LureRadiusEntered
}

public enum TimerStatus
{
	Resume,		// to start/resume
	Pause,		// to stop/pause
	Completed,	// signifies that the timer has run out
}
