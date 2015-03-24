using UnityEngine;
using System.Collections;

public class AxeManKillActiveTree : MonoBehaviour
{
    private const float SwingFrameTime = 0.436f / 2f;
    private const float StruggleFrameTime = 0.1f;


    public GameObject Axe;
    public Sprite[] Sprites;
    public _Sounds Sounds;


    private GameObject targetTree;
    private PossessableTree tree;
    private GameObject actualAxeMan;

    private SpriteRenderer spriteRenderer;
    private uint phase;
    private float timer;

    // Phase 1 variables
    bool frame1;

    // Phase 2 variables
    bool played;

    // Phase 1001 variables
    int frame;

    // Phase 9001 variables
    int chopIndex, numChops;


    void Start()
    {
        spriteRenderer = GetComponent<SpriteRenderer>();
        phase = 0;
        timer = 0f;
        frame1 = true;
        played = false;

        spriteRenderer.sprite = Sprites[0];

        // Play taunt
        audio.clip = Sounds.Taunt;
        audio.Play();

        MessageCenter.Instance.RegisterListener(MessageType.AxeManMinigameAxeManChangePhase, HandleChangePhase);
    }

    public void Instantiate(GameObject target, GameObject actualAxeMan)
    {
        targetTree = target;
        this.actualAxeMan = actualAxeMan;
        tree = targetTree.GetComponent<PossessableTree>();
    }

    private void HandleChangePhase(Message m)
    {
        AxeManMinigameAxeManChangePhaseMessage message = m as AxeManMinigameAxeManChangePhaseMessage;

        phase = message.Phase;

        // Handle phase initialization cases
        if (phase == 1001)
        {
            timer = 0f;
            frame = 4;
            spriteRenderer.sprite = Sprites[frame];

            audio.clip = Sounds.Struggle[Random.Range(0, Sounds.Struggle.Length)];
            audio.Play();
        }
        if(phase == 9001)
        {
            timer = 0f;
            chopIndex = 0;
            numChops = 0;
            frame1 = true;
            spriteRenderer.sprite = Sprites[2];
            played = false;
        }
        // Phase 9875 is when the tree has won
        if(phase == 98765)
        {
            Destroy(actualAxeMan);
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
            case 1001: UpdatePhase1001(); break;
            case 9001: UpdatePhase9001(); break;
            case 9002: UpdatePhase9002(); break;
            case 9003: UpdatePhase9003(); break;
            case 9004: UpdatePhase9004(); break;
            case 9005: UpdatePhase9005(); break;
        }
    }

    private void UpdatePhase0()
    {
        if(!audio.isPlaying)
        {
            // Change to phase 1
            phase = 1;
            spriteRenderer.sprite = Sprites[1];

            return;
        }
    }

    private void UpdatePhase1()
    {
        timer += Time.deltaTime;

        if(timer > SwingFrameTime)
        {
            if(frame1)
            {
                timer = 0f;
                frame1 = false;
                audio.clip = Sounds.Chop[1];
                spriteRenderer.sprite = Sprites[2];

                audio.Play();
            }
            else
            {
                timer = 0f;
                phase = 2;
                spriteRenderer.sprite = Sprites[3];

                // Show tree's axe
                tree.BodyParts.Axe.SetActive(true);

                /*tree.audio.clip = Sounds.Music;
                tree.audio.Play();*/
            }
        }
    }

    private void UpdatePhase2()
    {
        /*timer += Time.deltaTime;

        if(timer > 1f)
        {
            timer = 0f;
            phase = 3;

            // Tell the tree to advance a phase
            MessageCenter.Instance.Broadcast(new AxeManMinigameTreeChangePhaseMessage("PanToAxe"));
        }*/

        phase = 3;
        MessageCenter.Instance.Broadcast(new AxeManMinigameTreeChangePhaseMessage(targetTree, "Groan"));
    }

    private void UpdatePhase3()
    {
    }

    private void UpdatePhase1001()
    {
        transform.localPosition = new Vector3(0.421f, -0.211f);
        timer += Time.deltaTime;

        if(timer > StruggleFrameTime)
        {
            frame++;

            if(frame > 7) frame = 4;

            spriteRenderer.sprite = Sprites[frame];
            timer = 0f;
        }
    }

    private void UpdatePhase9001()
    {
        timer += Time.deltaTime;

        if(timer > SwingFrameTime)
        {
            timer = 0f;
            audio.clip = Sounds.Chop[2];
            spriteRenderer.sprite = Sprites[0];
            phase = 9002;

            audio.Play();
        }
    }

    private void UpdatePhase9002()
    {
        if(!audio.isPlaying)
        {
            phase = 9003;
            audio.clip = Sounds.Taunt;

            audio.Play();
        }
    }

    private void UpdatePhase9003()
    {
        timer += Time.deltaTime;

        if(timer > 1f)
        {
            timer = 0f;
            phase = 9004;
            spriteRenderer.sprite = Sprites[1];
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
                spriteRenderer.sprite = Sprites[2];

                audio.Play();
            }
            else
            {
                if (numChops < 10)
                {
                    timer = 0f;
                    frame1 = true;
                    spriteRenderer.sprite = Sprites[1];
                }
                else
                {
                    // Change to phase 2 if number of chops is complete
                    timer = 0f;
                    phase = 9005;
                    spriteRenderer.sprite = Sprites[0];

                    // Destroy the tree
                    Destroy(targetTree);

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

                MessageCenter.Instance.Broadcast(new LevelFinishedMessage(LevelFinishedType.Loss, LevelFinishedReason.PlayerDied));
            }
        }
    }

    [System.Serializable]
    public class _Sounds
    {
        public AudioClip Taunt;
        public AudioClip[] Chop;
        public AudioClip[] Gloat;
        public AudioClip[] Struggle;
        public AudioClip Music;
    }
}
