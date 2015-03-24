using UnityEngine;
using System.Collections;

public class AxeManKillInactiveTree : MonoBehaviour
{
    private const float SwingFrameTime = 0.436f / 2f;


    public delegate void Finished(GameObject cinematic, GameObject actual);


    public Sprite[] Sprites;
    public _Sounds Sounds;


    private GameObject targetTree;
    private GameObject actualAxeMan;
    private Finished finishedCallback;

    private SpriteRenderer spriteRenderer;
    private int phase;
    private float timer;

    // Phase 1 variables
    bool frame1;
    int chopIndex, numChops;

    // Phase 2 variables
    bool played;


    void Start()
    {
        spriteRenderer = GetComponent<SpriteRenderer>();
        phase = 0;
        timer = 0f;
        frame1 = true;
        chopIndex = 0;
        numChops = 0;
        played = false;

        spriteRenderer.sprite = Sprites[0];

        // Play taunt
        audio.clip = Sounds.Taunt;
        audio.Play();
    }

    public void Instantiate(GameObject target, GameObject actualAxeMan, Finished callback)
    {
        targetTree = target;
        this.actualAxeMan = actualAxeMan;
        finishedCallback = callback;
    }

    void Update()
    {
        switch(phase)
        {
            case 0: UpdatePhase0(); break;
            case 1: UpdatePhase1(); break;
            case 2: UpdatePhase2(); break;
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
                audio.clip = (numChops == 9) ? Sounds.Chop[2] : Sounds.Chop[chopIndex % 2];
                chopIndex++;
                numChops++;
                spriteRenderer.sprite = Sprites[2];

                audio.Play();
            }
            else
            {
                if(numChops < 10)
                {
                    timer = 0f;
                    frame1 = true;
                    spriteRenderer.sprite = Sprites[1];
                }
                else
                {
                    // Change to phase 2 if number of chops is complete
                    timer = 0f;
                    phase = 2;
                    spriteRenderer.sprite = Sprites[0];

                    // Destroy the tree
                    Destroy(targetTree);
                    GlobalGameStateManager.PanicTree = null;

                    // TODO: spawn tree explosion
                    // TODO: send GUI message
                }
            }
        }
    }

    private void UpdatePhase2()
    {
        if(!played)
        {
            timer += Time.deltaTime;

            if(timer > SwingFrameTime)
            {
                played = true;

                // Choose a gloat to say
                audio.clip = Sounds.Gloat[Random.Range(0, Sounds.Gloat.Length)];
                audio.Play();
            }
        }
        else
        {
            if(!audio.isPlaying)
                finishedCallback(gameObject, actualAxeMan);
        }
    }

    [System.Serializable]
    public class _Sounds
    {
        public AudioClip Taunt;
        public AudioClip[] Chop;
        public AudioClip[] Gloat;
    }
}
