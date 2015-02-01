using UnityEngine;
using System.Collections;
using System.Linq;

[ExecuteInEditMode]
public class CameraScript : MonoBehaviour
{
    public Transform ObjectToFollow;
    public Vector3 ObjectToFollowOffset;

    public float TargetSize;

    private float zoomSpeed;
    private bool locked;

    // Use this for initialization
    void Start()
    {
        MessageCenter.Instance.RegisterListener(MessageType.CameraZoom, HandleCameraZoomMessage);
        MessageCenter.Instance.RegisterListener(MessageType.CameraChangedObjectFollowed, HandleChangeObjectFollowed);
        //MessageCenter.Instance.RegisterListener(MessageType.CameraZoomOut, HandleCameraZoomOutMessage);
        TargetSize = camera.orthographicSize;
        locked = true;
    }

    void HandleCameraZoomMessage(Message message)
    {
        CameraZoomMessage zoomMessage = message as CameraZoomMessage;

        zoomSpeed = zoomMessage.Speed;
        TargetSize = zoomMessage.Size;
    }

    void HandleChangeObjectFollowed(Message message)
    {
        CameraChangeFollowedMessage changeMessage = message as CameraChangeFollowedMessage;

        locked = false;
        ObjectToFollow = changeMessage.NewTarget;
        ObjectToFollowOffset = changeMessage.Offset;
    }

    /*void HandleCameraZoomOutMessage(Message message)
    {
        zoomedIn = false;
    }*/

    // Update is called once per frame
    void Update()
    {
        if(ObjectToFollow != null)
        {
            Vector3 targetPosition = new Vector3(ObjectToFollow.position.x, ObjectToFollow.position.y, transform.position.z) + ObjectToFollowOffset;
            Vector3 distance = targetPosition - transform.position;
            float speed = 10f;

            if(locked) transform.position = targetPosition;
            else
            {
                transform.position += (distance.normalized * speed * Time.deltaTime);

                if ((targetPosition - transform.position).magnitude < 0.1f) locked = true;
            }
        }

        /*if(zoomedIn && camera.orthographicSize != 1.5f)
        {
            camera.orthographicSize -= (20f * Time.deltaTime);

            if (camera.orthographicSize < 1.5f) camera.orthographicSize = 1.5f;
        }

        if (!zoomedIn && camera.orthographicSize != 5)
        {
            camera.orthographicSize += (8f * Time.deltaTime);

            if (camera.orthographicSize > 5) camera.orthographicSize = 5;
        }*/

        if (camera.orthographicSize != TargetSize)
        {
            if (camera.orthographicSize > TargetSize)
            {
                camera.orthographicSize -= (zoomSpeed * Time.deltaTime);

                if (camera.orthographicSize < TargetSize) camera.orthographicSize = TargetSize;
            }
            else
            {
                camera.orthographicSize += (zoomSpeed * Time.deltaTime);

                if (camera.orthographicSize > TargetSize) camera.orthographicSize = TargetSize;
            }
        }
    }
}
