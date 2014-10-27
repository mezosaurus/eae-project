using UnityEngine;
using System.Collections;

public class PlayerScript : MonoBehaviour
{
    public float MaxSpeed;
    
    private PlayerMovementType movementType, lastMovementType;
    
    private void Start()
    {
        movementType = PlayerMovementType.Stationary;
        lastMovementType = PlayerMovementType.Stationary;

        MessageCenter.Instance.RegisterListener(MessageType.PlayerMovementTypeChanged, EventTest);
    }

    private void UpdateMovement()
    {
        Vector2 velocity = new Vector2(Input.GetAxis("LSX"), Input.GetAxis("LSY")) * MaxSpeed * Time.deltaTime;
        Vector2 zero = Vector2.zero;

        if (velocity.magnitude <= 0.01f) velocity = zero;

        // Update movement type
        if (velocity == zero) movementType = PlayerMovementType.Stationary;
        else movementType = PlayerMovementType.LowProfile;

        rigidbody2D.velocity = velocity;

        if (velocity.x < 0f) transform.localScale = new Vector3(-1f, transform.localScale.y, transform.localScale.z);
        else if (velocity.x > 0f) transform.localScale = new Vector3(1f, transform.localScale.y, transform.localScale.z);

        if (movementType != lastMovementType) MessageCenter.Instance.Broadcast(new PlayerMovementTypeChangedMessage(movementType));
    }

    private void EventTest(Message message)
    {
        PlayerMovementTypeChangedMessage theMessage = message as PlayerMovementTypeChangedMessage;

        Debug.Log("Player movement type changed to: " + theMessage.NewMovementType.ToString());
    }

    private void Update()
    {
        UpdateMovement();
    }

    private void LateUpdate()
    {
        lastMovementType = movementType;
    }

    private void OnDestroy()
    {
        MessageCenter.Instance.UnregisterListener(MessageType.PlayerMovementTypeChanged, EventTest);
    }
}

public enum PlayerMovementType
{
    Stationary,
    LowProfile,
    HighProfile,
    VeryHighProfile
}
