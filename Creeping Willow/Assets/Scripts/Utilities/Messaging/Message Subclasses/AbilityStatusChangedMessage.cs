
/**
 * Let's the player know if there is an ability in use
 * and the player is unable to perform any other moves 
 * until that ability is over
 **/
public class AbilityStatusChangedMessage : Message {

	public readonly bool abilityInUseStatus;

	public AbilityStatusChangedMessage(bool abilityStatus) : base(MessageType.AbilityStatusChanged)
	{
		abilityInUseStatus = abilityStatus;
	}
}
