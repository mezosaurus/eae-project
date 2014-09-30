using UnityEngine;
using System.Collections.Generic;
using XInputDotNetPure;

[System.Serializable]

public class PlayerController : MonoBehaviour 
{
	private class NPCOffset
    {
        public GameObject NPC;
        public Vector3 Offset;

        public NPCOffset(GameObject npc, Vector3 offset)
        {
            NPC = npc;
            Offset = offset;
        }
    }
    
    public float CurrentSpeed, MaxSpeed;
    public Transform LeftArm, RightArm, LeftForearm, RightForearm, Announcer;

    public int EatingBarWidth, EatingBarHeight;
    public Texture EatingBarBackground, EatingBarForeground;
    public Texture[] Buttons;
    public float EatingDecay, EatingIncrease;

    public AudioClip EatMusic, EatSound, Burp, SoulConsumed;
    public AudioClip DoubleKill, TripleKill, Overkill;
    public GUISkin SoulConsumedSkin, Skinx2, Skinx3, Skinx4;

    private string[] buttons = { "A", "B", "X", "Y" };

    // Normal variables
    private List<GameObject> npcsInRange;
    private Animator animator;
    private float upperArmChange, forearmChange;

    // Eating variables
    private List<NPCOffset> grabbedNPCs;
    private bool started;
    private float percentage;
    private float rumble, rumbleDirection;
    private int qteButton;

    // Eating cinematic variables
    /*private int eatHash, grinHash;
    private bool done1, done2;
    float timer, timer2;
    float ad1, ad2;*/
    private int hash;
    private bool leftArmUp, rightArmUp;
    private int multiplier;
    private string multiplierText;
    private float consumedTimer = 0f;

    void Start()
    {
        npcsInRange = new List<GameObject>();
        animator = GetComponent<Animator>();
        hash = Animator.StringToHash("Base Layer.EvilTree-MouthOpen");
        grabbedNPCs = new List<NPCOffset>();

        upperArmChange = 0f;
        forearmChange = 0f;
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
        Announcer.transform.position = transform.position;
        
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

        if (grabbedNPCs.Count != 0)
        {
            foreach(NPCOffset npcOffset in grabbedNPCs)
                npcOffset.NPC.GetComponent<aiController>().grabbed = false;

            grabbedNPCs.Clear();
        }

        GamePad.SetVibration(PlayerIndex.One, 0f, 0f);
        Globals.GameState = GameState.INLEVEL_DEFAULT;
    }

    private void ChangeStateToEating(IEnumerable<GameObject> npcs)
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

        foreach (GameObject npc in npcs)
        {
            aiController aic = npc.GetComponent<aiController>();
            
            aic.grabbed = true;
            aic.GetComponent<SpriteRenderer>().sprite = aic.normalTexture;
            Vector3 offset = Quaternion.EulerAngles(0f, 0f, Random.Range(0f, 360f)) * new Vector3(0.15f, 0f, 0f);

            grabbedNPCs.Add(new NPCOffset(npc, offset));
        }

        if (grabbedNPCs.Count == 1) grabbedNPCs[0].Offset = Vector3.zero;
        
        Globals.GameState = GameState.INLEVEL_EATING;

        started = false;
        percentage = 0.5f;
        rumble = 0.5f;
        consumedTimer = 0f;

        if (Random.Range(0f, 1f) > 0.5f) rumbleDirection = 1f;
        else rumbleDirection = -1f;

        qteButton = Random.Range(0, 4);

        animator.SetTrigger("MouthOpen");
    }

    private void ChangeStateToEatingCinematic()
    {
        /*GameObject.Destroy(grabbedNPC);
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

        audio.Play();*/

        Globals.GameState = GameState.INLEVEL_EATING_CINEMATIC;

        GamePad.SetVibration(PlayerIndex.One, 0f, 0f);

        leftArmUp = System.Convert.ToBoolean(Random.Range(0, 2));
        rightArmUp = !leftArmUp;
        audio.clip = EatSound;
        multiplier = 0;
        multiplierText = "";
    }

    private void UpdateNormal()
    {
        rigidbody2D.velocity = new Vector3(Input.GetAxis("LSX"), Input.GetAxis("LSY")) * MaxSpeed;
		CurrentSpeed = rigidbody2D.velocity.magnitude;

        if (Input.GetAxis("RT") > 0.5f) CurrentSpeed *= 0.5f;

        if (consumedTimer > 0f && consumedTimer < 2.2f)
        {
            consumedTimer += Time.deltaTime;

            if(consumedTimer >= 2.2f) consumedTimer = 0f;
        }

        /*if(ad1 > 0f)
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
        }*/

        if(upperArmChange > 0f)
        {
            float upperChange = upperArmChange * Time.deltaTime;
            float lowerChange = forearmChange * Time.deltaTime;

            if (LeftArm.rotation.eulerAngles.z > 160f)
            {
                LeftArm.Rotate(0f, 0f, -upperChange);
                LeftForearm.RotateAround(LeftArm.transform.position, new Vector3(0f, 0f, 1f), -upperChange);
                LeftForearm.Rotate(0f, 0f, -lowerChange);
                RightArm.Rotate(0f, 0f, upperChange);
                RightForearm.RotateAround(RightArm.transform.position, new Vector3(0f, 0f, 1f), upperChange);
                RightForearm.Rotate(0f, 0f, lowerChange);
            }
        }

        if(Input.GetAxis("LT") > 0.5f)
        {
            if (npcsInRange.Count == 0) return;

            ChangeStateToEating(npcsInRange);
        }
    }

    private void UpdateEating()
    {
        if (started)
        {
            if (Input.GetAxis("LT") < 0.5f || percentage <= 0f)
            {
                upperArmChange = (LeftArm.rotation.eulerAngles.z - 160f);
                forearmChange = (RightArm.rotation.eulerAngles.z - RightForearm.eulerAngles.z + 8f);
                
                ChangeStateToNormal();

                return;
            }

            if(percentage >= 1f)
            {
                ChangeStateToEatingCinematic();

                return;
            }

            Vector3 offset = new Vector3(transform.position.x, transform.position.y - 0.35f, -1f);

            foreach(NPCOffset npcOffset in grabbedNPCs) npcOffset.NPC.transform.position = offset + npcOffset.Offset;

            percentage -= (EatingDecay * Time.deltaTime);

            if (Input.GetButtonDown(buttons[qteButton]))
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
        if(animator.GetCurrentAnimatorStateInfo(0).nameHash == hash)
        {            
            switch(multiplier)
            {
                case 2:
                    multiplierText = "x2";
                    Announcer.audio.clip = DoubleKill;
                    Announcer.audio.Play();
                    break;

                case 3:
                    multiplierText = "x3";
                    Announcer.audio.clip = TripleKill;
                    Announcer.audio.Play();
                    break;

                case 4:
                    multiplierText = "x4";
                    Announcer.audio.clip = Overkill;
                    Announcer.audio.Play();
                    break;
                /*default:
                    if (multiplier > 1)
                        multiplierText = "x" + multiplier;
                    break;*/
            }
            
            if(grabbedNPCs.Count == 0)
            {
                multiplier = 0;
                
                if (audio.isPlaying)
                {
                    if (audio.clip != Burp)
                    {
                        Globals.gameTimer.AddTime( 15 );
                        Globals.healthController.AddHealth( 25 );

                        if (Globals.healthController.playerHealth >= Globals.healthController.maxHealth)
                            Globals.GameState = GameState.ENDOFLEVEL_VICTORY;
                        
                        audio.clip = Burp;
                        audio.Play();
                    }
                    else return;
                }
                else
                {
                    animator.SetTrigger("Grin");

                    //timer2 += Time.deltaTime;

                    audio.clip = SoulConsumed;
                    audio.Play();

                    consumedTimer += Time.deltaTime;

                    upperArmChange = (LeftArm.rotation.eulerAngles.z - 160f) / 1.96f;
                    forearmChange = (RightArm.rotation.eulerAngles.z - RightForearm.eulerAngles.z + 8f) / 1.96f;

                    ChangeStateToNormal();
                }
            }
            else if(grabbedNPCs.Count == 1)
            {
                // Eat the NPC
                NPCOffset npcOffset = grabbedNPCs[0];

                grabbedNPCs.Remove(npcOffset);
                GameObject.Destroy(npcOffset.NPC);
                
                leftArmUp = true;
                rightArmUp = true;

                animator.SetTrigger("Eat");
                audio.Play();
            }
            else
            {
                // Eat one NPC
                NPCOffset npcOffset = grabbedNPCs[0];

                grabbedNPCs.Remove(npcOffset);
                GameObject.Destroy(npcOffset.NPC);
                
                leftArmUp = !leftArmUp;
                rightArmUp = !leftArmUp;

                animator.SetTrigger("Eat");
                audio.Play();
            }


            ++multiplier;
        }

        Vector3 offset = new Vector3(transform.position.x, transform.position.y - 0.35f, 0f);

        foreach (NPCOffset npcOffset in grabbedNPCs) npcOffset.NPC.transform.position = offset + npcOffset.Offset;

        float upperChange = 5.291005291f * Time.deltaTime;
        float lowerChange = 18.51851852f * Time.deltaTime;

        if(leftArmUp && LeftArm.transform.rotation.eulerAngles.z < 249f)
        {
            LeftArm.Rotate(0f, 0f, upperChange);
            LeftForearm.RotateAround(LeftArm.transform.position, new Vector3(0f, 0f, 1f), upperChange);
            LeftForearm.Rotate(0f, 0f, lowerChange);
        }

        if (leftArmUp == false && LeftArm.transform.rotation.eulerAngles.z > 240f)
        {            
            LeftArm.Rotate(0f, 0f, -upperChange);
            LeftForearm.RotateAround(LeftArm.transform.position, new Vector3(0f, 0f, 1f), -upperChange);
            LeftForearm.Rotate(0f, 0f, -lowerChange);
        }

        if(rightArmUp && RightArm.transform.rotation.eulerAngles.z > 291f)
        {
            RightArm.Rotate(0f, 0f, -upperChange);
            RightForearm.RotateAround(RightArm.transform.position, new Vector3(0f, 0f, 1f), -upperChange);
            RightForearm.Rotate(0f, 0f, -lowerChange);
        }

        if (rightArmUp == false && RightArm.transform.rotation.eulerAngles.z < 300f)
        {
            RightArm.Rotate(0f, 0f, upperChange);
            RightForearm.RotateAround(RightArm.transform.position, new Vector3(0f, 0f, 1f), upperChange);
            RightForearm.Rotate(0f, 0f, lowerChange);
        }
        
        /*if (!done1 && animator.GetCurrentAnimatorStateInfo(0).nameHash != eatHash) done1 = true;

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
        }*/
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
            float x = (Screen.width - EatingBarBackground.width - Buttons[qteButton].width) / 2f;
			//float y = 172f;
			float y = ((Camera.main.WorldToScreenPoint(transform.position - new Vector3(0f, 1f, 0)).y - EatingBarBackground.height) / 2f);

            GUI.DrawTexture(new Rect(x, y, EatingBarBackground.width, EatingBarBackground.height), EatingBarBackground);
            GUI.DrawTexture(new Rect(x + 5f, y + 5f, EatingBarForeground.width * percentage, EatingBarForeground.height), EatingBarForeground);
            GUI.DrawTexture(new Rect(x + EatingBarBackground.width, y, Buttons[qteButton].width, Buttons[qteButton].height), Buttons[qteButton]);
        }

        if(consumedTimer > 0f && consumedTimer < 2.2f)
        {
            GUI.skin = SoulConsumedSkin;
            GUI.Label(new Rect(0f, 0f, Screen.width, (Screen.height / 4)), "Soul Consumed");
        }

        if(Globals.GameState == GameState.INLEVEL_EATING_CINEMATIC)
        {            
            if(multiplierText == "x2")       GUI.skin = Skinx2;
            else if (multiplierText == "x3") GUI.skin = Skinx3;
            else                      GUI.skin = Skinx4;

            Vector3 position = Camera.main.WorldToScreenPoint(transform.position + new Vector3(1f, -1f, 0));

            GUI.Label(new Rect(position.x, position.y, 200f, 200f), multiplierText);
        }
    }
}
