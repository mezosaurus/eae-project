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

        SpriteRenderer[] sprites = FindObjectsOfType<SpriteRenderer>();

        sprites = sprites.Where(item => item.sortingLayerName == "Background").OrderByDescending(x => x.gameObject.transform.position.y).ThenBy(x => x.gameObject.transform.position.x).ToArray();

        for (int i = 0; i < sprites.Length; i++)
        {
            sprites[i].sortingOrder = i;
        }

        foreach(SpriteRenderer sprite in FindObjectsOfType<SpriteRenderer>())
        {
            Debug.Log(sprite.sortingLayerName);
        }
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

#if UNITY_EDITOR
        SpriteRenderer[] sprites = FindObjectsOfType<SpriteRenderer>();

        sprites = sprites.Where(item => item.sortingLayerName == "Background").OrderByDescending(x => x.gameObject.transform.position.y).ThenBy(x => x.gameObject.transform.position.x).ToArray();

        for (int i = 0; i < sprites.Length; i++)
        {
            sprites[i].sortingOrder = i;
        }

        foreach(SpriteRenderer sprite in FindObjectsOfType<SpriteRenderer>())
        {
            //Debug.Log(sprite.sortingLayerName);
        }
#endif
    }

    void LateUpdate()
    {
        SpriteRenderer[] sprites = FindObjectsOfType<SpriteRenderer>();

        sprites = sprites.Where(item => item.sortingLayerName == "").OrderByDescending(x => x.gameObject.transform.position.y).ToArray();

        int player = -1;
        int playerFace = -1;
        int playerLegs = -1;
        int playerLeftArm = -1;
        int playerRightArm = -1;
        int j = 0;

        for (int i = 0; i < sprites.Length; i++)
        {
            if (sprites[i].sortingLayerName == "GUI") continue;
            
            if (sprites[i].gameObject.tag == "Player") { j += 8; player = i; }
            if (sprites[i].gameObject.tag == "PlayerFace") playerFace = i;
            if (sprites[i].gameObject.tag == "PlayerLegs") playerLegs = i;
            if (sprites[i].gameObject.tag == "PlayerLeftArm") playerLeftArm = i;
            if (sprites[i].gameObject.tag == "PlayerRightArm") playerRightArm = i;

            sprites[i].sortingOrder = j++;

            if (sprites[i].gameObject.tag == "Player") j += 8;
        }

        /*sprites[playerFace].sortingOrder = sprites[player].sortingOrder + 2;
        sprites[playerLegs].sortingOrder = sprites[player].sortingOrder - 2;*/
        if(player >= 0) sprites[player].GetComponent<TreeController>().UpdateSorting();
        //sprites[playerLeftArm].sortingOrder = sprites[player].sortingOrder + 1;
        //sprites[playerRightArm].sortingOrder = sprites[player].sortingOrder + 1;
    }
}
