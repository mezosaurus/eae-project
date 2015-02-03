using UnityEngine;

public enum LevelStartType {
	Start
}
public class LevelStartMessage : Message
{
	public LevelStartType Type;
	
	public LevelStartMessage(LevelStartType i_levelStartType) : base( MessageType.LevelStart )
	{
		Type = i_levelStartType;
	}
}
