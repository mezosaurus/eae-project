using UnityEngine;
using System.Collections.Generic;

[System.Serializable]

public class PlayerController : MonoBehaviour 
{
	public float Speed;
    public Transform LeftArm, RightArm;

    public int EatingBarWidth, EatingBarHeight;
    public Texture EatingBarBackground, EatingBarForeground;
    public float EatingDecay, EatingIncrease;

    // Normal variables
    private List<GameObject> npcsInRange;

    // Eating variables
    private GameObject grabbedNPC;
    private bool started;
    private float percentage;

    void Start()
    {
        npcsInRange = new List<GameObject>();
    }

    private GameObject FindNearestNPC()
    {
        GameObject nearest = null;
        float nearestDistance = float.MaxValue;
        
        foreach(GameObject npc in npcsInRange)
        {
            float distance = Vector3.Distance(transform.position, npc.transform.position);

            if(distance < nearestDistance)
            {
                nearestDistance = distance;
                nearest = npc;
            }
        }

        return nearest;
    }

	void Update()
	{
        switch(Globals.GameState)
        {
            case GameState.INLEVEL_DEFAULT: UpdateNormal(); break;
            case GameState.INLEVEL_EATING: UpdateEating();  break;
            case GameState.INLEVEL_EATING_CINEMATIC: break;
        }
	}

    private void UpdateNormal()
    {
        rigidbody.velocity = new Vector3(Input.GetAxis("LSX"), Input.GetAxis("LSY")) * Speed;

        if(Input.GetAxis("RT") > 0.5f)
        {            
            GameObject npc = FindNearestNPC();

            if (npc == null) return;

            ChangeStateToEating(npc);
        }
    }

    private void ChangeStateToEating(GameObject npc)
    {
        LeftArm.transform.Rotate(0f, 0f, 120f);
        RightArm.transform.Rotate(0f, 0f, -120f);

        rigidbody.isKinematic = true;
        grabbedNPC = npc;
        grabbedNPC.GetComponent<aiController>().grabbed = true;
        Globals.GameState = GameState.INLEVEL_EATING;

        started = false;
        percentage = 0.5f;
    }

    private void UpdateEating()
    {
        if (started)
        {
            grabbedNPC.transform.position = new Vector3(transform.position.x, transform.position.y - 0.4f, -1f);

            percentage -= (EatingDecay * Time.deltaTime);

            if (Input.GetButtonDown("A")) percentage += (EatingIncrease * Time.deltaTime);
        }
        else
        {
            if (Camera.main.orthographicSize == Camera.main.GetComponent<CameraController>().ZoomedInSize) started = true;
        }
    }

    void OnTriggerEnter(Collider collider)
    {
        if (Globals.GameState == GameState.INLEVEL_DEFAULT)
        {
            if (collider.gameObject.tag == "NPC") npcsInRange.Add(collider.gameObject);
        }
    }

    void OnTriggerLeave(Collider collider)
    {
        if (Globals.GameState == GameState.INLEVEL_DEFAULT)
        {
            if (collider.gameObject.tag == "NPC") npcsInRange.Remove(collider.gameObject);
        }
    }

    void OnGUI()
    {
        if(Globals.GameState == GameState.INLEVEL_EATING && started)
        {
            float x = (Screen.width - EatingBarBackground.width) / 2f;
            float y = 24f;

            Debug.Log(EatingBarBackground.height);

            GUI.DrawTexture(new Rect(x, y, EatingBarBackground.width, EatingBarBackground.height), EatingBarBackground);
            GUI.DrawTexture(new Rect(x + 5f, y + 5f, EatingBarForeground.width * percentage, EatingBarForeground.height), EatingBarForeground);
        }
    }
}
