/*
 * Intended for NPC's to receive this message so they know when the player has
 * changed movement types
 */
public class PlayerMovementTypeChangedMessage : Message
{
    public enum MovementType
    {
        Stationary,
        LowProfile,
        HighProfile,
        VeryHighProfile
    }

    public readonly MovementType NewMovementType;

    public PlayerMovementTypeChangedMessage(MovementType type) : base(MessageType.PlayerMovementTypeChanged)
    {
        NewMovementType = type;
    }
}
