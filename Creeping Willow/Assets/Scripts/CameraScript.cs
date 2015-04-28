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
    private Vector2 panFrom, panTo;
    private float panTimer, panSpeed;

    // ZF2
    private float zfTimer, zfTime;
    private Vector3 zfPointFrom, zfPointTo;
    private float zfZoomFrom, zfZoomTo;

    // TEMP?
    private const int SoulConsumedSize = 14;
    private int soulConsumed;
    private float scTimer;
    Texture2D[] SoulConsumed;

    // Use this for initialization
    void Start()
    {        
        MessageCenter.Instance.RegisterListener(MessageType.CameraZoom, HandleCameraZoomMessage);
        MessageCenter.Instance.RegisterListener(MessageType.CameraChangedObjectFollowed, HandleChangeObjectFollowed);
        MessageCenter.Instance.RegisterListener(MessageType.CameraZoomAndFocus, HandleZoomAndFocus);
        MessageCenter.Instance.RegisterListener(MessageType.CameraZoomAndFocus2, HandleZoomAndFocus2);
        //MessageCenter.Instance.RegisterListener(MessageType.CameraZoomOut, HandleCameraZoomOutMessage);
        TargetSize = camera.orthographicSize;
        locked = true;

        LoadSoulConsumedImages();

        GlobalGameStateManager.AxeManMinigameModifier1 = 1f;
        GlobalGameStateManager.AxeManMinigameModifier2 = 1f;
    }

    private void LoadSoulConsumedImages()
    {
        soulConsumed = 0;
        scTimer = 0f;
        SoulConsumed = new Texture2D[SoulConsumedSize];
        
        for(int i = 0; i < SoulConsumedSize; i++)
        {
            SoulConsumed[i] = Resources.Load<Texture2D>("Textures/SoulConsumed/SoulConsumed" + (i + 1));
        }
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

    void HandleZoomAndFocus(Message m)
    {
        CameraZoomAndFocusMessage message = m as CameraZoomAndFocusMessage;

        zoomSpeed = message.ZoomSpeed;
        TargetSize = message.ZoomSize;

        panFrom = transform.position;
        panTo = message.Point;
        panTimer = 0f;
        panSpeed = message.PanSpeed;

        ObjectToFollow = null;
    }

    void HandleZoomAndFocus2(Message m)
    {
        CameraZoomAndFocusMessage2 message = m as CameraZoomAndFocusMessage2;

        zfTimer = 0f;
        zfTime = message.Speed;

        zfPointFrom = transform.position;
        zfPointTo = message.Point;
        zfPointTo.z = transform.position.z;

        zfZoomFrom = camera.orthographicSize;
        zfZoomTo = message.Size;

        ObjectToFollow = null;
    }

    /*void HandleCameraZoomOutMessage(Message message)
    {
        zoomedIn = false;
    }*/

    // Update is called once per frame
    void Update()
    {        
        // Check for show mouse if it moves
		if( Input.GetAxis( "Mouse X" ) != 0 || Input.GetAxis( "Mouse Y" ) != 0 )
		{
			Screen.showCursor = true;
			Screen.lockCursor = false;
		}
		// Hide the mouse if you are using the controller
		else if( Input.GetAxis( "LSX" ) != 0 || Input.GetAxis( "LSY" ) != 0 )
		{
			Screen.showCursor = false;
			Screen.lockCursor = true;
		}
        
        if(ObjectToFollow != null)
        {
            Vector3 targetPosition = new Vector3(ObjectToFollow.position.x, ObjectToFollow.position.y, transform.position.z) + ObjectToFollowOffset;
            Vector3 distance = targetPosition - transform.position;
            float speed = 10f;

            /*if(locked)*/ transform.position = targetPosition;
            /*else
            {
                transform.position += (distance.normalized * speed * Time.deltaTime);

                if ((targetPosition - transform.position).magnitude < 0.1f) locked = true;
            }*/
        }

        if(panSpeed > 0f)
        {
            panTimer += Time.deltaTime;
            Vector2 position = Vector2.Lerp(panFrom, panTo, panTimer / panSpeed);

            transform.position = new Vector3(position.x, position.y, -10f);

            if (panTimer >= panSpeed) panSpeed = 0f;
        }

        if(zfTime > 0f)
        {
            zfTimer += Time.deltaTime;

            if (zfTimer > zfTime) zfTime = 0f;

            float ratio = zfTimer / zfTime;

            if (ratio > 1f) ratio = 1f;

            transform.position = Vector3.Lerp(zfPointFrom, zfPointTo, ratio);
            camera.orthographicSize = Mathf.Lerp(zfZoomFrom, zfZoomTo, ratio);
            TargetSize = Mathf.Lerp(zfZoomFrom, zfZoomTo, ratio);
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

        // TEMP?
        if (GlobalGameStateManager.SoulConsumedTimer > 0f)
        {
            GlobalGameStateManager.SoulConsumedTimer -= Time.deltaTime;
            
            scTimer += Time.deltaTime;

            if (scTimer > 0.075f)
            {
                scTimer = 0;

                ++soulConsumed;

                if (soulConsumed == SoulConsumedSize)
                {
                    soulConsumed = 0;
                }
            }
        }
    }

    private void OnDestroy()
    {
        MessageCenter.Instance.UnregisterListener(MessageType.CameraZoom, HandleCameraZoomMessage);
        MessageCenter.Instance.UnregisterListener(MessageType.CameraChangedObjectFollowed, HandleChangeObjectFollowed);
        MessageCenter.Instance.UnregisterListener(MessageType.CameraZoomAndFocus, HandleZoomAndFocus);
        MessageCenter.Instance.UnregisterListener(MessageType.CameraZoomAndFocus2, HandleZoomAndFocus2);
    }

    private void OnGUI()
    {
        if (GlobalGameStateManager.SoulConsumedTimer > 0f)
        {
            GUI.matrix = GlobalGameStateManager.PrepareMatrix();
            GUI.DrawTexture(new Rect(448f, 148f, 1024f, 194f), SoulConsumed[soulConsumed]);
            GUI.matrix = Matrix4x4.identity;
        }
    }
}
