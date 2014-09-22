using UnityEngine;
using System.Collections;

public class CameraController : MonoBehaviour {
	public GameObject player;
	private Vector3 offset;

    public float ZoomedOutSize, ZoomedInSize, ZoomSpeed;
    public Vector3 ZoomOffset;

    private float zoomDifference;

	// Use this for initialization
	void Start () {
        camera.orthographicSize = ZoomedOutSize;
        zoomDifference = ZoomedOutSize - ZoomedInSize;
	}
	
	void Update()
    {
        switch(Globals.GameState)
        {
            case GameState.INLEVEL_DEFAULT:
                // Zoom out if we were zoomed in
                if(camera.orthographicSize != ZoomedOutSize)
                {
                    camera.orthographicSize += ZoomSpeed;

                    if (camera.orthographicSize > ZoomedOutSize) camera.orthographicSize = ZoomedOutSize;
                }

                transform.position = new Vector3(player.transform.position.x, player.transform.position.y, -10f);
                break;

            case GameState.INLEVEL_EATING:
            case GameState.INLEVEL_EATING_CINEMATIC:
                // Zoom in if we were zoomed out
                if (camera.orthographicSize != ZoomedInSize)
                {
                    camera.orthographicSize -= ZoomSpeed;

                    if (camera.orthographicSize < ZoomedInSize) camera.orthographicSize = ZoomedInSize;
                }

                transform.position = new Vector3(player.transform.position.x, player.transform.position.y, -10f);
                break;
        }
    }
}
