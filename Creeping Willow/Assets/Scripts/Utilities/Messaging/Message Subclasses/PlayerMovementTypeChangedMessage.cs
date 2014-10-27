/*
 * Intended for NPC's to receive this message so they know
 * when the player has changed movement types
 */
public class PlayerMovementTypeChangedMessage : Message
{
    public readonly PlayerMovementType NewMovementType;

    public PlayerMovementTypeChangedMessage(PlayerMovementType type) : base(MessageType.PlayerMovementTypeChanged)
    {
        NewMovementType = type;
    }
}
