public class TimerStatusChangedMessage : Message
{
	public readonly TimerStatus g_timerStatus;

	public TimerStatusChangedMessage (TimerStatus i_timerStatus) : base(MessageType.TimerStatusChanged)
	{
		g_timerStatus = i_timerStatus;
	}
}
