public class AxeManAngerChangedMessage : Message
{

	public readonly bool Angry;

	public AxeManAngerChangedMessage(bool angry) : base(MessageType.AxeManAngerChanged)
	{
		Angry = angry;
	}
}
