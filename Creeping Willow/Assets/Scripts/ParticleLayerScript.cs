using UnityEngine;
using System.Collections;

public class ParticleLayerScript : MonoBehaviour {

	// Use this for initialization
	void Start ()
    {
        particleSystem.renderer.sortingLayerName = "GUI";
	}
	
	// Update is called once per frame
	void Update () {
	
	}
}
