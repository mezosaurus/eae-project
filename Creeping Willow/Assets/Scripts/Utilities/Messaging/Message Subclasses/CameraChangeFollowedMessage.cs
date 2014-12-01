using UnityEngine;

public class CameraChangeFollowedMessage : Message
{
    public Transform NewTarget;
    public Vector3 Offset;

    public CameraChangeFollowedMessage(Transform target, Vector3 offset) : base(MessageType.CameraChangedObjectFollowed)
    {
        NewTarget = target;
        Offset = offset;
    }
}
