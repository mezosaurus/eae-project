public class CameraZoomMessage : Message
{
    public float Size;
    public float Speed;

    public CameraZoomMessage(float size, float speed) : base(MessageType.CameraZoom) { Size = size; Speed = speed; }
}
