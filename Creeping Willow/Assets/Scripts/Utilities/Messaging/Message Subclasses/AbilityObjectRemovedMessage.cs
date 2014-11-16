using UnityEngine;

public class AbilityObjectRemovedMessage : Message {

	public AbilityType Atype;
	public AbilityObjectRemovedMessage(AbilityType atype):base(MessageType.AbilityObjectRemoved){
		Atype = atype;
	}
}
