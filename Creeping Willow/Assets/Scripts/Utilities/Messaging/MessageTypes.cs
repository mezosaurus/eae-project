public enum MessageType
{
    PlayerMovementTypeChanged,
	AbilityStatusChanged,
	TimerStatusChanged,
}

public enum TimerStatus
{
	Resume,		// to start/resume
	Pause,		// to stop/pause
	Completed,	// signifies that the timer has run out
}
