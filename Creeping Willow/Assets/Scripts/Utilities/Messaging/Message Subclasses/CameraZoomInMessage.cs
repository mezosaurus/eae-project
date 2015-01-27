using UnityEngine;

public class CameraZoomMessage : Message
{
    public float Size;
    public Vector3 Offset;
    public float Speed;

    public CameraZoomMessage(float size, float speed) : base(MessageType.CameraZoom) { Size = size; Offset = Vector3.zero; Speed = speed; }

    public CameraZoomMessage(float size, Vector3 offset, float speed) : base(MessageType.CameraZoom) { Size = size; Offset = offset; Speed = speed; }
}
