using UnityEngine;
using System.Collections.Generic;

public class TreeController : GameBehavior
{
    static string[] buttons = { "A", "B", "X", "Y" };
    static Color[] colors = { new Color(0.57f, 0.69f, 0.4f), new Color(0.75f, 0.42f, 0.28f), new Color(0.26f, 0.5f, 0.69f), new Color(0.86f, 0.73f, 0.24f) };
    
    public Tree.Textures Textures;
    public Tree.Sounds Sounds;
    public Tree.Prefabs Prefabs;
    public float Speed; // Maximum speed in units/second


    // General variables
    GameObject face, rightArm, leftArm, sideArmClose, sideArmFar, legs;
    GameObject leftUpperArm, leftLowerBackgroundArm, leftLowerForegroundArm, rightUpperArm, rightLowerArm, theGrabbedNPC;
    ProgressBarController progressBar;
    SpriteRenderer spriteRenderer;
    public Tree.State state;
    float xScale;
    Vector2 velocity;
    Tree.Direction direction;
    Dictionary<int, GameObject> npcsInRange, invalidNpcsInRange;
    float zoomOutSize, zoomOutSize2;

    // Eating variables phase 1
    GameObject grabbedNPC = null;
    GameObject ls = null, rs = null;
    Vector3 lsForce, rsForce;
    float lsDistance, rsDistance;
    float startTimer, percentage;
    bool phase1;
    bool started;
    int button;

    // Eating variables phase 2
    float upperArmInitialRotation;
    float lowerArmInitialRotation;
    float sampleRate = 3f;
    float timer;
    float upperFrom, upperTo;
    float average;
    int ticks;

    private void Start()
    {
        face = GameObject.FindGameObjectWithTag("PlayerFace");
        leftArm = GameObject.FindGameObjectWithTag("PlayerLeftArm");
        rightArm = GameObject.FindGameObjectWithTag("PlayerRightArm");
        legs = GameObject.FindGameObjectWithTag("PlayerLegs");
        progressBar = GameObject.FindGameObjectWithTag("ProgressBar").GetComponent<ProgressBarController>();
        leftUpperArm = GameObject.FindGameObjectWithTag("PlayerLeftUpperArm");
        leftLowerBackgroundArm = GameObject.FindGameObjectWithTag("PlayerLeftLowerBackgroundArm");
        leftLowerForegroundArm = GameObject.FindGameObjectWithTag("PlayerLeftLowerForegroundArm");
        rightUpperArm = GameObject.FindGameObjectWithTag("PlayerRightUpperArm");
        rightLowerArm = GameObject.FindGameObjectWithTag("PlayerRightLowerArm");
        theGrabbedNPC = GameObject.FindGameObjectWithTag("GrabbedNPC");
        spriteRenderer = GetComponent<SpriteRenderer>();
        xScale = transform.localScale.x;
        npcsInRange = new Dictionary<int, GameObject>();
        invalidNpcsInRange = new Dictionary<int, GameObject>();
        zoomOutSize = Camera.main.orthographicSize;
        zoomOutSize2 = zoomOutSize * 2f;

        ChangeStateToNormal();
    }

    private void ChangeStateToNormal()
    {
        state = Tree.State.Normal;
        MessageCenter.Instance.Broadcast(new CameraZoomMessage(zoomOutSize, 5f));
        if(audio.clip == Sounds.Music) audio.Stop();
        face.GetComponent<Animator>().enabled = false;
        leftUpperArm.GetComponent<SpriteRenderer>().enabled = false;
        leftLowerForegroundArm.GetComponent<SpriteRenderer>().enabled = false;
        leftLowerBackgroundArm.GetComponent<SpriteRenderer>().enabled = false;
        theGrabbedNPC.GetComponent<SpriteRenderer>().enabled = false;
        rightLowerArm.GetComponent<SpriteRenderer>().enabled = false;
        rightUpperArm.GetComponent<SpriteRenderer>().enabled = false;
        rightArm.GetComponent<SpriteRenderer>().enabled = true;
        leftArm.GetComponent<SpriteRenderer>().enabled = true;

        if (grabbedNPC != null)
        {
            List<GameObject> theList = new List<GameObject>();

            theList.Add(grabbedNPC);
            MessageCenter.Instance.Broadcast(new PlayerReleasedNPCsMessage(theList));
            grabbedNPC.GetComponent<SpriteRenderer>().enabled = true;
        }
    }

    private int GetClosestNPC()
    {
        float distance = float.MaxValue;
        int index = -1;
        
        foreach(int id in npcsInRange.Keys)
        {
            if (invalidNpcsInRange.ContainsKey(id))
            {
                float d = (npcsInRange[id].transform.position - transform.position).magnitude;

                if (d < distance)
                {
                    distance = d;
                    index = id;
                }
            }
        }

        return index;
    }

    private void ChangeStateToEating()
    {
        rigidbody2D.velocity = Vector2.zero;
        legs.GetComponent<Animator>().enabled = false;
        
        Debug.Log(npcsInRange.Count);
        state = Tree.State.Eating;
        this.velocity = Vector2.zero;
        if (transform.localScale.x < 0f) transform.localScale = new Vector3(xScale, transform.localScale.y, transform.localScale.z);
        face.GetComponent<SpriteRenderer>().sprite = Textures.Face.Crazy;
        MessageCenter.Instance.Broadcast(new CameraZoomMessage(2f, 20f));
        startTimer = 0f;
        percentage = 0f;
        phase1 = true;
        started = false;

        float range = Random.Range(0f, 1f);

        if (range <= 0.25f) button = 1;
        else if (range > 0.25f && range <= 0.5f) button = 2;
        else if (range > 0.5f && range <= 0.75f) button = 0;
        else if (range > 0.75f && range <= 1f) button = 3;

        progressBar.GetComponent<SpriteRenderer>().color = Color.white;

        float leftAngle = Random.Range(-120f, -240f);
        float rightAngle = Random.Range(-60f, 60f);

        ls = (GameObject)Instantiate(Prefabs.LeftThumbStick, progressBar.transform.position + (Quaternion.Euler(0f, 0f, leftAngle) * new Vector3(1.5f, 0)), Quaternion.identity);
        rs = (GameObject)Instantiate(Prefabs.RightThumbStick, progressBar.transform.position + (Quaternion.Euler(0f, 0f, rightAngle) * new Vector3(1.5f, 0)), Quaternion.identity);

        lsForce = (ls.transform.position - progressBar.transform.position).normalized * 0.5f;
        rsForce = (rs.transform.position - progressBar.transform.position).normalized * 0.5f;

        ls.GetComponent<SpriteRenderer>().enabled = false;
        rs.GetComponent<SpriteRenderer>().enabled = false;

        lsDistance = (ls.transform.position - progressBar.transform.position).magnitude;
        rsDistance = (rs.transform.position - progressBar.transform.position).magnitude;

        audio.clip = Sounds.Music;
        audio.Play();

        leftUpperArm.transform.localEulerAngles = new Vector3(0f, 0f, 358.0023f);
        leftLowerForegroundArm.transform.localEulerAngles = new Vector3(0f, 0f, 172.2082f);

        leftUpperArm.GetComponent<SpriteRenderer>().enabled = true;
        leftLowerForegroundArm.GetComponent<SpriteRenderer>().enabled = true;
        leftLowerBackgroundArm.GetComponent<SpriteRenderer>().enabled = true;
        theGrabbedNPC.GetComponent<SpriteRenderer>().enabled = true;
        rightLowerArm.GetComponent<SpriteRenderer>().enabled = true;
        rightUpperArm.GetComponent<SpriteRenderer>().enabled = true;
        rightArm.GetComponent<SpriteRenderer>().enabled = false;
        leftArm.GetComponent<SpriteRenderer>().enabled = false;

        grabbedNPC = npcsInRange[GetClosestNPC()];

        List<GameObject> theList = new List<GameObject>();

        theList.Add(grabbedNPC);

        MessageCenter.Instance.Broadcast(new PlayerGrabbedNPCsMessage(theList));
        grabbedNPC.GetComponent<SpriteRenderer>().enabled = false;
        grabbedNPC.GetComponent<AIController>().alertTexture.GetComponent<SpriteRenderer>().enabled = false;
        grabbedNPC.GetComponent<AIController>().panicTexture.GetComponent<SpriteRenderer>().enabled = false;

        theGrabbedNPC.GetComponent<Animator>().enabled = (grabbedNPC.name.Contains("BenchNPC")) ? false : true;
        float angle = (grabbedNPC.name.Contains("BenchNPC")) ? 53f : 0f;

        //theGrabbedNPC.transform.eulerAngles = new Vector3(0f, 0f, angle);
        theGrabbedNPC.transform.localEulerAngles = new Vector3(0f, 0f, angle);

        if (grabbedNPC.name.Contains("BenchNPC")) theGrabbedNPC.GetComponent<SpriteRenderer>().sprite = Textures.GrabbedNPCs.OldMan;
    }

    private void ChangeStateToEatingCinematic()
    {
        state = Tree.State.EatingCinematic;
        theGrabbedNPC.GetComponent<SpriteRenderer>().enabled = false;
        timer = 0f;
        face.GetComponent<Animator>().enabled = true;
        audio.Stop();
        audio.clip = Sounds.Chew;
        audio.Play();

        npcsInRange.Remove(grabbedNPC.GetInstanceID());

		// Broadcast NPC being eaten
		MessageCenter.Instance.Broadcast (new NPCEatenMessage(grabbedNPC));

        Destroy(grabbedNPC);

        grabbedNPC = null;

        leftUpperArm.transform.localEulerAngles = new Vector3(0f, 0f, 380.8806f);
        leftLowerForegroundArm.transform.localEulerAngles = new Vector3(0f, 0f, 203.9803f);
    }

    private void LeaveEatingState()
    {
        if(ls != null)
        {
            Destroy(ls);
            Destroy(rs);

            ls = null;
            rs = null;
        }

        progressBar.GetComponent<SpriteRenderer>().sprite = progressBar.Sprites[0];
    }

    private void OnTriggerEnter2D(Collider2D collider)
    {
		if (collider.tag == "NPC" && !npcsInRange.ContainsKey(collider.gameObject.GetInstanceID())) npcsInRange.Add(collider.gameObject.GetInstanceID(), collider.gameObject);
		//if (collider.tag == "NPC") npcsInRange.Add(collider.gameObject.GetInstanceID(), collider.gameObject);
    }

    private void OnTriggerExit2D(Collider2D collider)
    {
        if (collider.tag == "NPC")
        {
            npcsInRange.Remove(collider.gameObject.GetInstanceID());
            invalidNpcsInRange.Remove(collider.gameObject.GetInstanceID());
        }
    }

    protected override void GameUpdate()
    {
        switch (state)
        {
            case Tree.State.Normal: UpdateNormal(); break;
            case Tree.State.Eating: UpdateEating(); break;
            case Tree.State.EatingCinematic: UpdateEatingCinematic(); break;
        }
    }

    private void UpdateNormal()
    {
        if (Camera.main.orthographicSize > zoomOutSize)
        {
            if (Input.GetAxis("DY") > 0.5f)  MessageCenter.Instance.Broadcast(new CameraZoomMessage(zoomOutSize, 10f));

            return;
        }
        
        if (!GetComponent<PlayerAbilityScript_v2>().abilityInUse)
        {
            // Check to see if the player is grabbing
            if (Input.GetAxis("LT") > 0.5f && npcsInRange.Count > 0)
            {
                ChangeStateToEating();

                return;
            }

            velocity = new Vector2(Input.GetAxis("LSX"), Input.GetAxis("LSY"));

            if (Input.GetAxis("DY") < -0.5f)
            {
                MessageCenter.Instance.Broadcast(new CameraZoomMessage(zoomOutSize2, 10f));

                return;
            }

            // Keyboard Input
            if (Input.GetKey(KeyCode.LeftArrow) || Input.GetKey(KeyCode.RightArrow)) velocity.x = (Input.GetKey(KeyCode.LeftArrow)) ? -1f : 1f;
            if (Input.GetKey(KeyCode.UpArrow) || Input.GetKey(KeyCode.DownArrow)) velocity.y = (Input.GetKey(KeyCode.UpArrow)) ? 1f : -1f;

            rigidbody2D.velocity = velocity * Mathf.Lerp(0f, Speed, Time.deltaTime);
        }
        else rigidbody2D.velocity = Vector2.zero;

        UpdateDirection();
        UpdateSprites();
    }

    private void UpdateEating()
    {
        if (Camera.main.orthographicSize != Camera.main.GetComponent<CameraScript>().TargetSize) return;

        if (phase1) UpdateEatingPhase1();
        else UpdateEatingPhase2();
    }

    private void UpdateEatingPhase1()
    {

        ls.GetComponent<SpriteRenderer>().enabled = true;
        rs.GetComponent<SpriteRenderer>().enabled = true;
        
        percentage += 0.32f * Time.deltaTime;

        if(percentage >= 1f)
        {
            invalidNpcsInRange.ContainsKey(collider.gameObject.GetInstanceID());
            LeaveEatingState();
            ChangeStateToNormal();

            return;
        }

        if (InsideProgressBar(ls.transform.position) && InsideProgressBar(rs.transform.position)) { ChangeToEatingPhase2(); return; }

        progressBar.GetComponent<SpriteRenderer>().sprite = progressBar.Sprites[Mathf.RoundToInt(percentage * 100f)];

        Vector3 lsInput = new Vector3(Input.GetAxis("LSX"), Input.GetAxis("LSY")) * 1.35f * Time.deltaTime;
        Vector3 rsInput = new Vector3(Input.GetAxis("RSX"), Input.GetAxis("RSY")) * 1.35f * Time.deltaTime;

        ls.transform.position += lsInput + (lsForce * Time.deltaTime);
        rs.transform.position += rsInput + (rsForce * Time.deltaTime);

        float lsPercentage = (ls.transform.position - progressBar.transform.position).magnitude / lsDistance;
        float rsPercentage = (rs.transform.position - progressBar.transform.position).magnitude / rsDistance;
        float average = (lsPercentage + rsPercentage) / 2f;

        leftUpperArm.transform.localEulerAngles = new Vector3(0f, 0f, Mathf.Lerp(391.59891f, 358.0023f, average));
        leftLowerForegroundArm.transform.localEulerAngles = new Vector3(0f, 0f, Mathf.Lerp(199.8239f, 172.2082f, average));
    }

    private void UpdateEatingPhase2()
    {        
        float decrease = (percentage > 0.9f) ? 0.08f * Time.deltaTime : 0.48f * Time.deltaTime;
        float increase = 0f;

        if (Input.GetButtonDown(buttons[button])) increase = 8f * Time.deltaTime;

        progressBar.GetComponent<SpriteRenderer>().color = colors[button];

        percentage += (decrease - increase);

        if (percentage >= 1f)
        {
            LeaveEatingState();
            ChangeStateToNormal();

            return;
        }

        if(percentage <= 0f)
        {
            LeaveEatingState();
            ChangeStateToEatingCinematic();

            return;
        }

        //if (percentage <= 0f) UnityEditor.EditorApplication.ExecuteMenuItem("Edit/Pause");

        progressBar.GetComponent<SpriteRenderer>().sprite = progressBar.Sprites[Mathf.RoundToInt(percentage * 100f)];

        /*if(sampleIndex == 0)
        {
            sample = 0f;

            for (int i = 0; i < sampleRate; i++) sample += samples[i];

            sample /= (float)sampleRate;
        }

        samples[sampleIndex++] = percentage;

        if (sampleIndex == sampleRate) sampleIndex = 0;

        sample = 1f - percentage;*/

        timer += sampleRate * Time.deltaTime;

        leftUpperArm.transform.localEulerAngles = new Vector3(0f, 0f, Mathf.SmoothStep(upperFrom, upperTo, timer));

        if (timer >= 1f)
        {
            timer = 0f;
            average /= (float)(ticks + 1);
            ticks = 0;

            upperFrom = upperTo;
            upperTo = Mathf.Lerp(upperArmInitialRotation, 53.97528f, 1f - average);
        }

        average += percentage;
        ticks++;

        //leftLowerForegroundArm.transform.localEulerAngles = new Vector3(0f, 0f, Mathf.SmoothStep(lowerArmInitialRotation, 204.8439f, sample));

        /*if (InsideProgressBar(ls.transform.position) && InsideProgressBar(rs.transform.position))

            

        Vector3 lsInput = new Vector3(Input.GetAxis("LSX"), Input.GetAxis("LSY")) * 1.25f * Time.deltaTime;
        Vector3 rsInput = new Vector3(Input.GetAxis("RSX"), Input.GetAxis("RSY")) * 1.25f * Time.deltaTime;

        ls.transform.position += lsInput + (lsForce * Time.deltaTime);
        rs.transform.position += rsInput + (rsForce * Time.deltaTime);*/
    }

    private void UpdateEatingCinematic()
    {
        if (!audio.isPlaying)
        {
            audio.Stop();
            audio.clip = Sounds.SoulConsumed;
            audio.Play();

            ChangeStateToNormal();

            return;
        }
    }

    private void ChangeToEatingPhase2()
    {
        phase1 = false;
        //percentage = 0.0f;
        Destroy(ls);
        Destroy(rs);

        ls = null;
        rs = null;

        upperArmInitialRotation = leftUpperArm.transform.localEulerAngles.z;

        if (upperArmInitialRotation > 360f) upperArmInitialRotation -= 360f;

        lowerArmInitialRotation = leftLowerForegroundArm.transform.localEulerAngles.z;

        upperFrom = Mathf.Lerp(upperArmInitialRotation, 53.97528f, 1f - percentage);
        upperTo = upperFrom;
        timer = 0f;
        average = percentage;
        ticks = 0;
    }

    private bool InsideProgressBar(Vector3 position)
    {
        return (position - progressBar.transform.position).magnitude <= 0.25f;
    }

    public void UpdateSorting()
    {        
        if (state == Tree.State.Normal)
        {
            leftArm.GetComponent<SpriteRenderer>().sortingOrder = spriteRenderer.sortingOrder + 1;
            rightArm.GetComponent<SpriteRenderer>().sortingOrder = spriteRenderer.sortingOrder + 1;
            face.GetComponent<SpriteRenderer>().sortingOrder = spriteRenderer.sortingOrder + 2;
            legs.GetComponent<SpriteRenderer>().sortingOrder = spriteRenderer.sortingOrder - 3;
            //progressBar.GetComponent<SpriteRenderer>().sortingOrder = spriteRenderer.sortingOrder + 4;
        }
        else if(state == Tree.State.Eating || state == Tree.State.EatingCinematic)
        {
            face.GetComponent<SpriteRenderer>().sortingOrder = spriteRenderer.sortingOrder + 1;
            leftUpperArm.GetComponent<SpriteRenderer>().sortingOrder = spriteRenderer.sortingOrder + 2;
            rightUpperArm.GetComponent<SpriteRenderer>().sortingOrder = spriteRenderer.sortingOrder + 2;
            leftLowerBackgroundArm.GetComponent<SpriteRenderer>().sortingOrder = spriteRenderer.sortingOrder + 3;
            rightLowerArm.GetComponent<SpriteRenderer>().sortingOrder = spriteRenderer.sortingOrder + 3;
            theGrabbedNPC.GetComponent<SpriteRenderer>().sortingOrder = spriteRenderer.sortingOrder + 4;
            leftLowerForegroundArm.GetComponent<SpriteRenderer>().sortingOrder = spriteRenderer.sortingOrder + 5;
            legs.GetComponent<SpriteRenderer>().sortingOrder = spriteRenderer.sortingOrder - 3;
            progressBar.GetComponent<SpriteRenderer>().sortingOrder = spriteRenderer.sortingOrder + 6;
        }
    }

    private void UpdateDirection()
    {
        if (velocity == Vector2.zero) return;
        
        float angle = Mathf.Atan2(velocity.y, velocity.x) * Mathf.Rad2Deg;

        if      (angle >  -67.5f && angle <=  -22.5f) direction = Tree.Direction.FrontRight;
        else if (angle >  -22.5f && angle <=   22.5f) direction = Tree.Direction.Right;
        else if (angle >   22.5f && angle <=   67.5f) direction = Tree.Direction.BackRight;
        else if (angle >   67.5f && angle <=  112.5f) direction = Tree.Direction.Back;
        else if (angle >  112.5f && angle <=  157.5f) direction = Tree.Direction.BackLeft;
        else if (angle >  157.5f || angle <= -157.5f) direction = Tree.Direction.Left;
        else if (angle > -157.5f && angle <= -112.5f) direction = Tree.Direction.FrontLeft;
        else direction = Tree.Direction.Front;
    }

    private void UpdateSprites()
    {
        int index = (velocity == Vector2.zero) ? 0 : 3;

        Animator legsAnimator = legs.GetComponent<Animator>();

        legsAnimator.enabled = (velocity != Vector2.zero);
        legsAnimator.speed = Mathf.Clamp(velocity.magnitude, 0f, 1f);

        switch (direction)
        {
            case Tree.Direction.Front:
                spriteRenderer.sprite = Textures.Body.Front;
                face.GetComponent<SpriteRenderer>().sprite = Textures.Face.Front[index];
                break;

            case Tree.Direction.FrontRight:
                spriteRenderer.sprite = Textures.Body.FrontRight;
                face.GetComponent<SpriteRenderer>().sprite = Textures.Face.FrontRight[index];
                break;

            case Tree.Direction.Right:
                spriteRenderer.sprite = Textures.Body.Right;

                if (index == 3) index = 1;

                face.GetComponent<SpriteRenderer>().sprite = Textures.Face.Right[index];
                break;

            case Tree.Direction.BackRight:
                spriteRenderer.sprite = Textures.Body.BackRight;
                face.GetComponent<SpriteRenderer>().sprite = Textures.Face.Blank;
                break;

            case Tree.Direction.Back:
                spriteRenderer.sprite = Textures.Body.Back;
                face.GetComponent<SpriteRenderer>().sprite = Textures.Face.Blank;
                break;

            case Tree.Direction.BackLeft:
                spriteRenderer.sprite = Textures.Body.BackRight;
                face.GetComponent<SpriteRenderer>().sprite = Textures.Face.Blank;
                break;

            case Tree.Direction.Left:
                spriteRenderer.sprite = Textures.Body.Right;

                if (index == 3) index = 1;

                face.GetComponent<SpriteRenderer>().sprite = Textures.Face.Right[index];
                break;

            case Tree.Direction.FrontLeft:
                spriteRenderer.sprite = Textures.Body.FrontRight;
                face.GetComponent<SpriteRenderer>().sprite = Textures.Face.FrontRight[index];
                break;
        }

        // Flip sides if necessary
        if (direction == Tree.Direction.FrontLeft || direction == Tree.Direction.Left || direction == Tree.Direction.BackLeft) transform.localScale = new Vector3(-xScale, transform.localScale.y, transform.localScale.z);
        else transform.localScale = new Vector3(xScale, transform.localScale.y, transform.localScale.z);
    }

    public void OnGUI()
    {
        if(state == Tree.State.Eating && !phase1)
        {
            int width = Textures.Buttons[0].width;
            int height = Textures.Buttons[0].height;
            Vector3 position = Camera.main.WorldToScreenPoint(progressBar.transform.position);

            GUI.DrawTexture(new Rect(position.x - (width / 2f), position.y - (256f * transform.localScale.y), width, height), Textures.Buttons[button]);
        }
    }
}

namespace Tree
{
    [System.Serializable]
    public class Textures
    {
        [System.Serializable]
        public class _Body
        {
            public Sprite Front, FrontRight, Right, BackRight, Back;
        }

        [System.Serializable]
        public class _Face
        {
            public Sprite Blank, Crazy;
            public Sprite[] Front, FrontRight, Right;
        }

        [System.Serializable]
        public class _GrabbedNPCs
        {
            public Sprite OldMan, MowerMan;
        }

        public _Body Body;
        public _Face Face;
        public _GrabbedNPCs GrabbedNPCs;
        public Texture2D[] Buttons;
    }

    [System.Serializable]
    public class Sounds
    {
        public AudioClip Music, Chew, SoulConsumed;
    }

    [System.Serializable]
    public class Prefabs
    {
        public GameObject LeftThumbStick, RightThumbStick;
    }

    public enum Direction
    {
        Front, FrontRight, Right, BackRight, Back, BackLeft, Left, FrontLeft
    }

    public enum State
    {
        Normal,
        Eating,
        EatingCinematic
    }
}
