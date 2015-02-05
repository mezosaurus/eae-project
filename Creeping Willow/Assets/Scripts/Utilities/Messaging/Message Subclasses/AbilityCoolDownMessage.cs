public enum AbilityType{
	Minion,
	Ranged,
	Lure,
	PossessionScare,
	PossessionLure,
	Trap,
	Movement
}
public class AbilityCoolDownMessage:Message
{
	public AbilityType AbilityType;
	public float CoolDown;
	public float TimeElapsed;

	public AbilityCoolDownMessage (AbilityType type, float coolDown, float timeElapsed):base(MessageType.AbilityCoolDownMessage)
	{
		AbilityType = type;
		CoolDown = coolDown;
		TimeElapsed = timeElapsed;
	}
}

