public class AbilityPlacedMessage:Message
{
	public float X;
	public float Y;
	public AbilityType AType;
	public AbilityPlacedMessage (float x, float y, AbilityType atype):base(MessageType.AbilityPlaced)
	{
		X = x;
		Y = y;
		AType = atype;
	}
}

