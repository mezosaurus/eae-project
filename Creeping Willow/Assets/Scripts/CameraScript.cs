using UnityEngine;
using System.Collections;

public class CameraScript : MonoBehaviour
{
    public Transform ObjectToFollow;

    // Use this for initialization
    void Start()
    {

    }

    // Update is called once per frame
    void Update()
    {
        transform.position = new Vector3(ObjectToFollow.position.x, ObjectToFollow.position.y, transform.position.z);
    }
}
