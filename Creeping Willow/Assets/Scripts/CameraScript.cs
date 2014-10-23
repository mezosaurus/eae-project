using UnityEngine;
using System.Collections;

public class CameraScript : MonoBehaviour {

	// Use this for initialization
	void Start () {
	
	}
	
	// Update is called once per frame
	void Update ()
    {
        //transform.position = transform.position + (new Vector3(Input.GetAxis("LSX"), Input.GetAxis("LSY")) * 8f * Time.deltaTime);
        transform.position = Vector3.Lerp(transform.position, transform.position + (new Vector3(Input.GetAxis("LSX"), Input.GetAxis("LSY")) * 5f), Time.deltaTime);

        if (transform.position.x >  17f) transform.position = new Vector3( 17f, transform.position.y, transform.position.z);
        if (transform.position.x < -17f) transform.position = new Vector3(-17f, transform.position.y, transform.position.z);
        if (transform.position.y >  10f) transform.position = new Vector3(transform.position.x,  10f, transform.position.z);
        if (transform.position.y < -10f) transform.position = new Vector3(transform.position.x, -10f, transform.position.z);
	}
}
