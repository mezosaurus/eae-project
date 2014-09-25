using UnityEngine;
using System.Collections.Generic;
using XInputDotNetPure;

[System.Serializable]

public class PlayerController : MonoBehaviour 
{
	public float Speed, MaxSpeed;
    public Transform LeftArm, RightArm, LeftForearm, RightForearm;

    public int EatingBarWidth, EatingBarHeight;
    public Texture EatingBarBackground, EatingBarForeground;
    public float EatingDecay, EatingIncrease;

    public AudioClip EatMusic;
    public AudioClip[] Eat;
    public AudioClip Burp, SoulConsumed;
    public GUISkin SoulConsumedSkin;

    // Normal variables
    private List<GameObject> npcsInRange;
    private Animator animator;

    // Eating variables
    private GameObject grabbedNPC;
    private bool started;
    private float percentage;
    private float rumble, rumbleDirection;

    // Eating cinematic variables
    private int eatHash, grinHash;
    private bool done1, done2;
    float timer, timer2;
    float ad1, ad2;

    void Start()
    {
        npcsInRange = new List<GameObject>();
        animator = GetComponent<Animator>();
        eatHash = Animator.StringToHash("Base Layer.EvilTree-Eat");
        grinHash = Animator.StringToHash("Base Layer.EvilTree-Grin");
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
            case GameState.INLEVEL_EATING_CINEMATIC: UpdateEatingCinematic(); break;
        }

        if (Input.GetButtonDown("Back")) Application.LoadLevel(0);
	}

    private void ChangeStateToNormal()
    {
        npcsInRange = new List<GameObject>();

        //LeftArm.transform.Rotate(0f, 0f, -140f);
        //RightArm.transform.Rotate(0f, 0f, 140f);

        rigidbody2D.isKinematic = false;

        if (grabbedNPC != null)
        {
            grabbedNPC.GetComponent<aiController>().grabbed = false;
            grabbedNPC = null;
        }

        GamePad.SetVibration(PlayerIndex.One, 0f, 0f);
        Globals.GameState = GameState.INLEVEL_DEFAULT;
    }

    private void ChangeStateToEating(GameObject npc)
    {
        /*LeftArm.transform.Rotate(0f, 0f, 80f);
        LeftForearm.transform.Rotate(0f, 0f, 8f);
        LeftForearm.transform.RotateAround(LeftArm.transform.position, new Vector3(0f, 0f, 1f), 80f);
        LeftForearm.transform.Rotate(0f, 0f, 110f);
        RightArm.transform.Rotate(0f, 0f, -80f);
        RightForearm.transform.Rotate(0f, 0f, -8f);
        RightForearm.transform.RotateAround(RightArm.transform.position, new Vector3(0f, 0f, 1f), -80f);
        RightForearm.transform.Rotate(0f, 0f, -110f);*/

        // Stop all other audio
        foreach (AudioSource audioSource in FindObjectsOfType<AudioSource>()) audioSource.Stop();

        audio.clip = EatMusic;

        audio.Play();

        LeftArm.transform.position = transform.position + new Vector3(-0.409021f, 0.2674716f, 0f);
        LeftArm.eulerAngles = new Vector3(0f, 0f, 240f);
        LeftForearm.transform.position = transform.position + new Vector3(-0.699663f, -0.2375197f, 0f);
        LeftForearm.eulerAngles = new Vector3(0f, 0f, 350f);
        RightArm.transform.position = transform.position + new Vector3(0.409021f, 0.2674716f, 0f);
        RightArm.eulerAngles = new Vector3(0f, 0f, 300f);
        RightForearm.transform.position = transform.position + new Vector3(0.699663f, -0.2375197f, 0f);
        RightForearm.eulerAngles = new Vector3(0f, 0f, 190f);

        rigidbody2D.isKinematic = true;
        grabbedNPC = npc;
        grabbedNPC.GetComponent<aiController>().grabbed = true;
        Globals.GameState = GameState.INLEVEL_EATING;

        started = false;
        percentage = 0.5f;
        rumble = 0.5f;

        if (Random.Range(0f, 1f) > 0.5f) rumbleDirection = 1f;
        else rumbleDirection = -1f;
    }

    private void ChangeStateToEatingCinematic()
    {
        GameObject.Destroy(grabbedNPC);
        animator.SetTrigger("Eat");
        GamePad.SetVibration(PlayerIndex.One, 0f, 0f);

        grabbedNPC = null;
        Globals.GameState = GameState.INLEVEL_EATING_CINEMATIC;
        done1 = false;
        done2 = false;
        timer = 0f;
        timer2 = 0f;
        ad1 = 0f;
        ad2 = 0f;

        float random = Random.Range(0f, 1f);
        int clipId = Mathf.RoundToInt((float)(Eat.Length - 1) * random);
        audio.clip = Eat[clipId];

        audio.Play();
    }

    private void UpdateNormal()
    {
        rigidbody2D.velocity = new Vector3(Input.GetAxis("LSX"), Input.GetAxis("LSY")) * MaxSpeed;
		Speed = rigidbody2D.velocity.magnitude;

        if (timer2 > 0f && timer2 < 2.2f) timer2 += Time.deltaTime;

        if(ad1 > 0f)
        {
            float a1 = ad1 * Time.deltaTime;
            float a2 = ad2 * Time.deltaTime;
            
            if(LeftArm.rotation.eulerAngles.z > 160f)
            {
                LeftArm.Rotate(0f, 0f, -a1);
                LeftForearm.RotateAround(LeftArm.transform.position, new Vector3(0f, 0f, 1f), -a1);
                LeftForearm.Rotate(0f, 0f, -a2);
                RightArm.Rotate(0f, 0f, a1);
                RightForearm.RotateAround(RightArm.transform.position, new Vector3(0f, 0f, 1f), a1);
                RightForearm.Rotate(0f, 0f, a2);
            }
        }

        if(Input.GetAxis("LT") > 0.5f)
        {            
            GameObject npc = FindNearestNPC();

            if (npc == null) return;

            ChangeStateToEating(npc);
        }
    }

    private void UpdateEating()
    {
        if (started)
        {
            if (Input.GetAxis("LT") < 0.5f || percentage <= 0f)
            {
                ad1 = (LeftArm.rotation.eulerAngles.z - 160f);
                ad2 = (RightArm.rotation.eulerAngles.z - RightForearm.eulerAngles.z + 8f);
                
                ChangeStateToNormal();

                return;
            }

            if(percentage >= 1f)
            {
                ChangeStateToEatingCinematic();

                return;
            }
            
            grabbedNPC.transform.position = new Vector3(transform.position.x, transform.position.y - 0.22f, -1f);

            percentage -= (EatingDecay * Time.deltaTime);

            if (Input.GetButtonDown("A"))
            {
                percentage += (EatingIncrease * Time.deltaTime);

                if (percentage > 1f) percentage = 1f;
            }

            rumble += (Time.deltaTime * rumbleDirection);

            if(rumble > 1f)
            {
                rumble = 1f;
                rumbleDirection *= -1f;
            }

            if(rumble < 0f)
            {
                rumble = 0f;
                rumbleDirection *= -1f;
            }

            GamePad.SetVibration(PlayerIndex.One, Random.Range(0f, 0.75f), Random.Range(0f, 0.75f));
        }
        else
        {
            if (Camera.main.orthographicSize == Camera.main.GetComponent<CameraController>().ZoomedInSize) started = true;
        }
    }

    private void UpdateEatingCinematic()
    {
        if (!done1 && animator.GetCurrentAnimatorStateInfo(0).nameHash != eatHash) done1 = true;

        if(!audio.isPlaying && !done2)
        {
            done2 = true;
            audio.clip = Burp;
			Globals.gameTimer.time += 15;
			Globals.healthController.playerHealth += 25;

			if( Globals.healthController.playerHealth >= Globals.healthController.maxHealth )
				Globals.GameState = GameState.ENDOFLEVEL_VICTORY;

            audio.Play();
        }

        if (!audio.isPlaying && done2) animator.SetTrigger("Grin");

        if (done2 && timer < 1.5f) timer += Time.deltaTime;

        if(timer >= 1.5f)
        {
            animator.ResetTrigger("Grin");
            animator.SetTrigger("Normal");

            timer2 += Time.deltaTime;
            audio.clip = SoulConsumed;

            audio.Play();

            ad1 = (LeftArm.rotation.eulerAngles.z - 160f) / 1.96f;
            ad2 = (RightArm.rotation.eulerAngles.z - RightForearm.eulerAngles.z + 8f) / 1.96f;

            ChangeStateToNormal();
        }

        if (!done1)
        {
            float a1 = 5.291005291f * Time.deltaTime;
            float a2 = 18.51851852f * Time.deltaTime;
            
            LeftArm.Rotate(0f, 0f, a1);
            LeftForearm.RotateAround(LeftArm.transform.position, new Vector3(0f, 0f, 1f), a1);
            LeftForearm.Rotate(0f, 0f, a2);
            RightArm.Rotate(0f, 0f, -a1);
            RightForearm.RotateAround(RightArm.transform.position, new Vector3(0f, 0f, 1f), -a1);
            RightForearm.Rotate(0f, 0f, -a2);
        }
        else 
        {
            if (RightArm.transform.rotation.eulerAngles.z < 305f)
            {
                Debug.Log(RightArm.transform.rotation.z);
                
                float a1 = 15f * Time.deltaTime;
                float a2 = 135f * Time.deltaTime;

                LeftArm.Rotate(0f, 0f, -a1); LeftForearm.RotateAround(LeftArm.transform.position, new Vector3(0f, 0f, 1f), -a1);
                LeftForearm.Rotate(0f, 0f, -a2);
                RightArm.Rotate(0f, 0f, a1);
                RightForearm.RotateAround(RightArm.transform.position, new Vector3(0f, 0f, 1f), a1);
                RightForearm.Rotate(0f, 0f, a2);
            }
        }
    }

    void OnTriggerEnter2D(Collider2D collider)
    {
        if (Globals.GameState == GameState.INLEVEL_DEFAULT)
        {
            if (collider.gameObject.tag == "NPC") 
            {
                var npc = collider.gameObject;
                float distance = Vector2.Distance(transform.position, npc.transform.position);
                if (distance <= 3 && !npcsInRange.Contains(collider.gameObject))
                {
                    npcsInRange.Add (collider.gameObject);
                }
            }
        }
    }

    void OnTriggerExit2D(Collider2D collider)
    {
        if (Globals.GameState == GameState.INLEVEL_DEFAULT)
        {
            if (collider.gameObject.tag == "NPC") 
            {
                var npc = collider.gameObject;
                float distance = Vector2.Distance(transform.position, npc.transform.position);
                if (distance > 3)
                {
                    npcsInRange.Remove(collider.gameObject);
                }
            }
        }
    }

    void OnGUI()
    {
        if(Globals.GameState == GameState.INLEVEL_EATING && started)
        {
            float x = (Screen.width - EatingBarBackground.width) / 2f;
			float y = 172f;
			//float y = ((Camera.main.WorldToScreenPoint(transform.position - new Vector3(0f, 1f, 0)).y - EatingBarBackground.height) / 2f);

            GUI.DrawTexture(new Rect(x, y, EatingBarBackground.width, EatingBarBackground.height), EatingBarBackground);
            GUI.DrawTexture(new Rect(x + 5f, y + 5f, EatingBarForeground.width * percentage, EatingBarForeground.height), EatingBarForeground);
        }

        if(timer2 > 0f && timer2 < 2.2f)
        {
            GUI.skin = SoulConsumedSkin;
            GUI.Label(new Rect(0f, 156f, Screen.width, 200f), "Soul Consumed");
        }
    }
}
