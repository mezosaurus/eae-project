using UnityEngine;
using System.Collections;
using System.Linq;

public class CameraScript : MonoBehaviour
{
    public Transform ObjectToFollow;

    private bool zoomedIn = false;
    bool dum = true;

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
        if(ObjectToFollow != null) transform.position = new Vector3(ObjectToFollow.position.x, ObjectToFollow.position.y, transform.position.z);

        if(zoomedIn && camera.orthographicSize != 2)
        {
            camera.orthographicSize -= (20f * Time.deltaTime);

            if (camera.orthographicSize < 2) camera.orthographicSize = 2;
        }

        if (!zoomedIn && camera.orthographicSize != 5)
        {
            camera.orthographicSize += (8f * Time.deltaTime);

            if (camera.orthographicSize > 5) camera.orthographicSize = 5;
        }
    }

    void LateUpdate()
    {
        SpriteRenderer[] sprites = FindObjectsOfType<SpriteRenderer>();

        sprites = sprites.OrderByDescending(x => x.gameObject.transform.position.y).ToArray();

        for(int i = 0; i < sprites.Length; i++)
        {
            sprites[i].sortingOrder = i;
        }
    }
}
