using UnityEngine;

public class CameraZoomAndFocusMessage : Message
{
    public Vector3 Point;
    public float PanSpeed;
    public float ZoomSize;
    public float ZoomSpeed;
    
    public CameraZoomAndFocusMessage(Vector3 point, float panSpeed, float zoomSize, float zoomSpeed) : base(MessageType.CameraZoomAndFocus)
    {
        Point = point;
        PanSpeed = panSpeed;
        ZoomSize = zoomSize;
        ZoomSpeed = zoomSpeed;
    }
}