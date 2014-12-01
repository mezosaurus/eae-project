using UnityEngine;

public class NotorietyMaxedMessage : Message
{
	public Vector3 panickedPosition;
	
	public NotorietyMaxedMessage( Vector3 position ) : base( MessageType.NotorietyMaxed )
	{
		panickedPosition = position;
	}
}
