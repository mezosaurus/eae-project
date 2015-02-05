using UnityEngine;

public class ParticlePreviewScript : MonoBehaviour
{
    public GameObject[] Particles;

    private int index = 0;

	// Use this for initialization
	void Start ()
    {
        
	}
	
	// Update is called once per frame
	void Update ()
    {

        //Particles[index].transform.position = new Vector3(Input.mousePosition.x, Input.mousePosition.y);

        if(Input.GetKeyDown(KeyCode.UpArrow))
        {
            index--;

            if (index < 0) index = Particles.Length - 1;
        }

        if (Input.GetKeyDown(KeyCode.DownArrow))
        {
            index++;

            if (index > Particles.Length - 1) index = 0;
        }

        if (Input.GetMouseButtonDown(0))
        {
            Particles[index].GetComponent<ParticleSystem>().Play();
        }
	}

    void OnGUI()
    {
        GUI.Label(new Rect(20, 20, 800, 100), Particles[index].name);
    }
}
