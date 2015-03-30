using UnityEngine;

public class CameraZoomAndFocusMessage2 : Message
{
    public Vector3 Point;
    public float Size;
    public float Speed;
    
    public CameraZoomAndFocusMessage2(Vector3 point, float size, float speed) : base(MessageType.CameraZoomAndFocus2)
    {
        Point = point;
        Size = size;
        Speed = speed;
    }
}