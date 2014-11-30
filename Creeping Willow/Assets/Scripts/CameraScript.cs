using UnityEngine;
using System.Collections;
using System.Linq;

#if UNITY_EDITOR
using UnityEngine;
#endif

[ExecuteInEditMode]
public class CameraScript : MonoBehaviour
{
    public Transform ObjectToFollow;
    public Vector3 ObjectToFollowOffset;

    private bool zoomedIn = false;

    // Use this for initialization
    void Start()
    {
        MessageCenter.Instance.RegisterListener(MessageType.CameraZoomIn, HandleCameraZoomInMessage);
        MessageCenter.Instance.RegisterListener(MessageType.CameraZoomOut, HandleCameraZoomOutMessage);
    }

    void HandleCameraZoomInMessage(Message message)
    {
        zoomedIn = true;
    }

    void HandleCameraZoomOutMessage(Message message)
    {
        zoomedIn = false;
    }

    // Update is called once per frame
    void Update()
    {
        if(ObjectToFollow != null) transform.position = new Vector3(ObjectToFollow.position.x, ObjectToFollow.position.y, transform.position.z) + ObjectToFollowOffset;

        if(zoomedIn && camera.orthographicSize != 1.5f)
        {
            camera.orthographicSize -= (20f * Time.deltaTime);

            if (camera.orthographicSize < 1.5f) camera.orthographicSize = 1.5f;
        }

        if (!zoomedIn && camera.orthographicSize != 5)
        {
            camera.orthographicSize += (8f * Time.deltaTime);

            if (camera.orthographicSize > 5) camera.orthographicSize = 5;
        }
    }
}
