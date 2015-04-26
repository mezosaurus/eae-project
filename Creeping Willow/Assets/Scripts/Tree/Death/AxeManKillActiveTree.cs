using UnityEngine;
using System.Collections;

public class AxeManKillActiveTree : MonoBehaviour
{
    private const float SwingFrameTime = 0.436f / 2f;
    private const float StruggleFrameTime = 0.1f;
    private const float BigWinFrameTime1 = 0.1f;
    private const float BigWinFrameTime2 = 0.2f;


    public GameObject Axe;
    //public Sprite[] Sprites;
    public _Sprites Sprites;
    public _Sounds Sounds;


    private GameObject targetTree;
    private PossessableTree tree;
    private GameObject actualAxeMan;

    private SpriteRenderer spriteRenderer;
    private uint phase;
    private float timer;
    private Vector3 startPosition;

	private bool levelEndedMessageSent;

    // Phase 1 variables
    bool frame1;

    // Phase 2 variables
    bool played;

    // Phase 1001 variables
    int frame;

    // Phase 8001 variables
    GameObject axe;
    Vector3 from, peak;
    float rotation;

    // Phase 9001 variables
    int chopIndex, numChops;


    void Start()
    {
        spriteRenderer = GetComponent<SpriteRenderer>();
        phase = 0;
        timer = 0f;
        frame1 = true;
        played = false;

        //spriteRenderer.sprite = Sprites[0];
        spriteRenderer.sprite = Sprites.Stand[0];

        audio.rolloffMode = AudioRolloffMode.Linear;
        audio.volume = 0.5f;

        // Play taunt
        /*audio.clip = Sounds.Taunt[Random.Range(0, Sounds.Taunt.Length)];
        audio.Play();*/

        MessageCenter.Instance.RegisterListener(MessageType.AxeManMinigameAxeManChangePhase, HandleChangePhase);

		levelEndedMessageSent = false;
        startPosition = transform.position;
    }

    public void Instantiate(GameObject target, GameObject actualAxeMan)
    {
        targetTree = target;
        this.actualAxeMan = actualAxeMan;
        tree = targetTree.GetComponent<PossessableTree>();
    }

    private GameObject FindClosestAxe()
    {
        float closestDistance = float.MaxValue;
        GameObject closestAxe = null;

        foreach(GameObject axe in GameObject.FindGameObjectsWithTag("Axe"))
        {            
            float distance = Vector3.Distance(transform.position, axe.transform.position);

            if(distance < closestDistance)
            {
                closestDistance = distance;
                closestAxe = axe;
            }
        }

        return closestAxe;
    }

    private void HandleChangePhase(Message m)
    {
        AxeManMinigameAxeManChangePhaseMessage message = m as AxeManMinigameAxeManChangePhaseMessage;

        phase = message.Phase;

        // Handle phase initialization cases
        if (phase == 1001)
        {
            timer = 0f;
            frame = 0;
            //spriteRenderer.sprite = Sprites[frame];
            spriteRenderer.sprite = Sprites.Struggle[frame];
            //transform.SetParent(tree.BodyParts.RightLowerForegroundArm.transform);

            audio.clip = Sounds.Struggle[Random.Range(0, Sounds.Struggle.Length)];
            audio.Play();
        }
        if(phase == 8001)
        {
            timer = 0f;
            spriteRenderer.sprite = Sprites.Struggle[2];

            transform.SetParent(null);

            // Calculate x midpoint
            float xm = (startPosition.x - transform.position.x);
            from = transform.position;
            peak = new Vector3(transform.position.x + xm, transform.position.y + 0.1f);

            // Calculate rotation needed
            rotation = transform.eulerAngles.z;

            axe = FindClosestAxe();
        }
        if(phase == 9001)
        {
            timer = 0f;
            chopIndex = 0;
            numChops = 0;
            frame1 = true;
            //spriteRenderer.sprite = Sprites[2];
            spriteRenderer.sprite = Sprites.Swing[1];
            played = false;
        }
        // Phase 9875 is when the tree has won
        if(phase == 98765)
        {
            MessageCenter.Instance.Broadcast(new AxeManKilledMessage());
            //Destroy(actualAxeMan);
            Destroy(gameObject);
        }
    }

    void Update()
    {        
        switch(phase)
        {
            case 0: UpdatePhase0(); break;
            case 1: UpdatePhase1(); break;
            case 2: UpdatePhase2(); break;
            case 3: UpdatePhase3(); break;
            case 4: UpdatePhase4(); break;
            case 1001: UpdatePhase1001(); break;
            case 8001: UpdatePhase8001(); break;
            case 8002: UpdatePhase8002(); break;
            case 8003: UpdatePhase8003(); break;
            case 9001: UpdatePhase9001(); break;
            case 9002: UpdatePhase9002(); break;
            case 9003: UpdatePhase9003(); break;
            case 9004: UpdatePhase9004(); break;
            case 9005: UpdatePhase9005(); break;
        }
    }

    private void UpdatePhase0()
    {
        if(Camera.main.orthographicSize == Camera.main.GetComponent<CameraScript>().TargetSize)
        {
            timer += Time.deltaTime;

            if (timer > 1f)
            {
                // Play taunt
                phase = 1;
                timer = 0f;

                if(!GlobalGameStateManager.PlayedJohnny)
                {
                    audio.clip = Sounds.Found[1];
                    GlobalGameStateManager.PlayedJohnny = true;
                }
                else audio.clip = Sounds.Found[Random.Range(0, Sounds.Found.Length)];

                audio.Play();
            }
        }
    }

    private void UpdatePhase1()
    {
        if(!audio.isPlaying)
        {
            timer += Time.deltaTime;

            if (timer > 0.33f)
            {

                // Change to phase 2
                phase = 2;
                //spriteRenderer.sprite = Sprites[1];
                spriteRenderer.sprite = Sprites.Swing[0];
                audio.volume = 1f;
                audio.rolloffMode = AudioRolloffMode.Logarithmic;
            }

            return;
        }
    }

    private void UpdatePhase2()
    {
        timer += Time.deltaTime;

        if(timer > SwingFrameTime)
        {
            if(frame1)
            {
                timer = 0f;
                frame1 = false;
                audio.clip = Sounds.Chop[1];
                //spriteRenderer.sprite = Sprites[2];
                spriteRenderer.sprite = Sprites.Swing[1];

                audio.Play();
            }
            else
            {
                timer = 0f;
                phase = 3;
                //spriteRenderer.sprite = Sprites[3];
                spriteRenderer.sprite = Sprites.StandNoAxe[0];

                // Show tree's axe
                tree.BodyParts.Axe.SetActive(true);

                /*tree.audio.clip = Sounds.Music;
                tree.audio.Play();*/
            }
        }
    }

    private void UpdatePhase3()
    {
        /*timer += Time.deltaTime;

        if(timer > 1f)
        {
            timer = 0f;
            phase = 3;

            // Tell the tree to advance a phase
            MessageCenter.Instance.Broadcast(new AxeManMinigameTreeChangePhaseMessage("PanToAxe"));
        }*/

        phase = 4;
        MessageCenter.Instance.Broadcast(new AxeManMinigameTreeChangePhaseMessage(targetTree, "Groan"));
    }

    private void UpdatePhase4()
    {
    }

    private void UpdatePhase1001()
    {
        transform.localPosition = new Vector3(0.421f, -0.211f);
        timer += Time.deltaTime;

        if(timer > StruggleFrameTime)
        {
            frame++;

            if(frame > 3) frame = 0;

            //spriteRenderer.sprite = Sprites[frame];
            spriteRenderer.sprite = Sprites.Struggle[frame];
            timer = 0f;
        }
    }

    private void UpdatePhase8001()
    {
        timer += Time.deltaTime;

        if (timer > BigWinFrameTime1)
            timer = BigWinFrameTime1;

        transform.position = Vector3.Lerp(from, peak, timer / BigWinFrameTime1);
        transform.eulerAngles = new Vector3(0f, 0f, Mathf.Lerp(rotation, 360f, timer / (BigWinFrameTime1 + BigWinFrameTime2)));

        if (timer >= BigWinFrameTime1)
        {
            timer = 0f;
            phase = 8002;

            return;
        }
    }

    private void UpdatePhase8002()
    {
        timer += Time.deltaTime;

        if (timer > BigWinFrameTime2)
            timer = BigWinFrameTime2;

        transform.position = Vector3.Lerp(peak, startPosition, timer / BigWinFrameTime2);
        transform.eulerAngles = new Vector3(0f, 0f, Mathf.Lerp(rotation, 360f, (timer + BigWinFrameTime1) / (BigWinFrameTime1 + BigWinFrameTime2)));

        if (timer >= BigWinFrameTime2)
        {
            timer = 0f;
            phase = 9002;
            spriteRenderer.sprite = Sprites.Stand[0];
            transform.eulerAngles = Vector3.zero;
            Destroy(axe);

            return;
        }
    }

    private void UpdatePhase8003()
    {
        Debug.Log(rotation);
        //transform.Rotate(new Vector3(0f, 0f, rotation * Time.deltaTime));
    }

    private void UpdatePhase9001()
    {
        timer += Time.deltaTime;

        if(timer > SwingFrameTime)
        {
            timer = 0f;
            audio.clip = Sounds.Chop[2];
            //spriteRenderer.sprite = Sprites[0];
            spriteRenderer.sprite = Sprites.Stand[0];
            phase = 9002;

            audio.Play();
        }
    }

    private void UpdatePhase9002()
    {
        if(!audio.isPlaying)
        {
            //timer += Time.deltaTime;

            /*if (timer > 0.25f)
            {*/
                phase = 9003;
                audio.clip = Sounds.Taunt[Random.Range(0, Sounds.Taunt.Length)];

                audio.Play();
            //}
        }
    }

    private void UpdatePhase9003()
    {
        if (!audio.isPlaying)
        {
            timer = 0f;
            phase = 9004;
            //spriteRenderer.sprite = Sprites[1];
            spriteRenderer.sprite = Sprites.Swing[0];
        }
    }

    private void UpdatePhase9004()
    {
        timer += Time.deltaTime;

        if (timer > SwingFrameTime)
        {
            if (frame1)
            {
                timer = 0f;
                frame1 = false;
                audio.clip = (numChops == 9) ? Sounds.Chop[2] : Sounds.Chop[chopIndex % 2];
                chopIndex++;
                numChops++;
                //spriteRenderer.sprite = Sprites[2];
                spriteRenderer.sprite = Sprites.Swing[1];

                audio.Play();
            }
            else
            {
                if (numChops < 10)
                {
                    timer = 0f;
                    frame1 = true;
                    //spriteRenderer.sprite = Sprites[1];
                    spriteRenderer.sprite = Sprites.Swing[0];
                }
                else
                {
                    // Change to phase 2 if number of chops is complete
                    timer = 0f;
                    phase = 9005;
                    //spriteRenderer.sprite = Sprites[0];
                    spriteRenderer.sprite = Sprites.Stand[0];

                    // Destroy the tree
                    Destroy(targetTree);
                    GlobalGameStateManager.PanicTree = null;

                    // TODO: spawn tree explosion
                    // TODO: send GUI message
                }
            }
        }
    }

    private void UpdatePhase9005()
    {
        if (!played)
        {
            timer += Time.deltaTime;

            if (timer > SwingFrameTime)
            {
                played = true;

                // Choose a gloat to say
                audio.clip = Sounds.Gloat[Random.Range(0, Sounds.Gloat.Length)];
                audio.Play();
            }
        }
        else
        {
            if (!audio.isPlaying)
            {
                /*actualAxeMan.SetActive(true);
                Destroy(gameObject);*/

				if( !levelEndedMessageSent )
				{
					levelEndedMessageSent = true;
                	MessageCenter.Instance.Broadcast(new LevelFinishedMessage(LevelFinishedType.Loss, LevelFinishedReason.PlayerDied));
                    Debug.Log("Sent loss message");
				}
            }
        }
    }

    void OnDestroy()
    {
        MessageCenter.Instance.UnregisterListener(MessageType.AxeManMinigameAxeManChangePhase, HandleChangePhase);
    }

    [System.Serializable]
    public class _Sprites
    {
        public Sprite[] Stand;
        public Sprite[] StandNoAxe;
        public Sprite[] Swing;
        public Sprite[] Struggle;
    }

    [System.Serializable]
    public class _Sounds
    {
        public AudioClip[] Found;
        public AudioClip[] Taunt;
        public AudioClip[] Chop;
        public AudioClip[] Gloat;
        public AudioClip[] Struggle;
        public AudioClip Music;
    }
}
