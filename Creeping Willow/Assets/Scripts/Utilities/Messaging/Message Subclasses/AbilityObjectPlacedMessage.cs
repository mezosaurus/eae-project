using UnityEngine;

public class AbilityObjectPlacedMessage : Message {

	public int NumPlaced;
	public AbilityType Atype;

	public AbilityObjectPlacedMessage(int numPlaced, AbilityType atype):base(MessageType.AbilityObjectPlaced){
		NumPlaced = numPlaced;
		Atype = atype;
	}
}
