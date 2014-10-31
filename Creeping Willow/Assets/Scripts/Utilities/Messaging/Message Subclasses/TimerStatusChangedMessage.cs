public enum TimerStatus
{
	Resume,		// to start/resume
	Pause,		// to stop/pause
	Completed,	// signifies that the timer has run out
}

public class TimerStatusChangedMessage : Message
{
	public readonly TimerStatus g_timerStatus;

	public TimerStatusChangedMessage( TimerStatus i_timerStatus ) : base(MessageType.TimerStatusChanged)
	{
		g_timerStatus = i_timerStatus;
	}
}
