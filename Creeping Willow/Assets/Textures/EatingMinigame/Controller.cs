using UnityEngine;
using System.Collections;
using XInputDotNetPure;

public class Controller : MonoBehaviour
{
    private enum State
    {
        NotStarted,
        Phase1,
        Phase2,
        Win,
        Lose
    }

    [System.Serializable]
    public class _Textures
    {
        public Texture[] Buttons;
        public Texture BarBackground, BarForeground;
    }

    [System.Serializable]
    public class _Eating
    {
        public float Decay, Increase;
    }


    public _Textures Textures;
    public GUISkin Skin, EndSkin;
    public GameObject Stick;
    public _Eating Eating;


    private int EmptyHash = Animator.StringToHash("Base Layer.Empty");
    private State state = State.NotStarted;
    private Animator animator;
    private int npcsGrabbed = 1;
    private int numberOfAttempts = 0;

    // Phase 1
    private readonly string[] buttons = { "A", "B", "X", "Y" };
    GameObject leftStick, rightStick;
    Vector3 leftStickOpposition, rightStickOpposition;

    // Phase 2
    float percentage;
    int qteButton;

    // End
    float timer;


    // Use this for initialization
    void Start()
    {
        animator = GetComponent<Animator>();
    }

    private void ChangeToNotStarted()
    {
        state = State.NotStarted;
        GamePad.SetVibration(PlayerIndex.One, 0f, 0f);
    }

    private void ChangeToPhase1()
    {
        state = State.Phase1;
        animator.SetTrigger("Countdown");
        float leftAngle = Random.Range(-120f, -240f);
        leftStick = (GameObject)GameObject.Instantiate(Stick, Quaternion.Euler(0f, 0f, leftAngle) * new Vector3(3, 0), Quaternion.identity);
        leftStickOpposition = (leftStick.transform.position - transform.position).normalized * 0.5f;
        
        if(npcsGrabbed > 1)
        {
            float rightAngle = Random.Range(-60f, 60f);
            rightStick = (GameObject)GameObject.Instantiate(Stick, Quaternion.Euler(0f, 0f, rightAngle) * new Vector3(3, 0), Quaternion.identity);
            rightStickOpposition = (rightStick.transform.position - transform.position).normalized * 0.5f;
        }

        GamePad.SetVibration(PlayerIndex.One, 0.5f, 0.5f);
    }

    private void ChangeToPhase2()
    {
        state = State.Phase2;
        animator.SetTrigger("Empty");
        GameObject.Destroy(leftStick);
        GameObject.Destroy(rightStick);
        leftStick = null;
        percentage = 0.5f;
        qteButton = Random.Range(0, 3);
        GamePad.SetVibration(PlayerIndex.One, 1f, 1f);
    }

    private void ChangeToWin()
    {
        state = State.Win;
        timer = 0f;
        GamePad.SetVibration(PlayerIndex.One, 0f, 0f);
        numberOfAttempts = 0;
    }

    private void ChangeToLose()
    {
        state = State.Lose;
        GameObject.Destroy(leftStick);
        GameObject.Destroy(rightStick);
        timer = 0f;
        GamePad.SetVibration(PlayerIndex.One, 0f, 0f);
        numberOfAttempts = 0;
    }

    private void UpdateNotStarted()
    {
        if (Input.GetButtonDown("A")) ChangeToPhase1();
        if(Input.GetButtonDown("B"))
        {
            npcsGrabbed++;

            if (npcsGrabbed == 3) npcsGrabbed = 1;
        }
    }

    private void UpdatePhase1()
    {
        bool leftInside = InsideCircle(leftStick.transform.position);
        bool rightInside = false;

        if (rightStick != null) rightInside = InsideCircle(rightStick.transform.position);

        if ((npcsGrabbed == 1 && leftInside) || (rightInside && leftInside)) { ChangeToPhase2(); return; }
        if (animator.GetCurrentAnimatorStateInfo(0).nameHash == EmptyHash) { ChangeToLose(); return; }

        Vector3 ls = new Vector3(Input.GetAxis("LSX"), Input.GetAxis("LSY")) * 2f * Time.deltaTime;
        Vector3 lo = Vector3.zero;

        /*if (!leftInside)*/ lo = leftStickOpposition * Time.deltaTime;

        Vector3 ld = ls + lo;

        leftStick.transform.position += ld;

        if (npcsGrabbed > 1)
        {
            Vector3 rs = new Vector3(Input.GetAxis("RSX"), Input.GetAxis("RSY")) * 2f * Time.deltaTime;
            Vector3 ro = Vector3.zero;

            /*if (!rightInside)*/ ro = rightStickOpposition * Time.deltaTime;

            Vector3 rd = rs + ro;

            rightStick.transform.position += rd;
        }
    }

    private void UpdatePhase2()
    {
        if (percentage <= 0f)
        {
            if (numberOfAttempts > 0) ChangeToLose();
            else { numberOfAttempts++; ChangeToPhase1(); }
        }
        if (percentage >= 1f) ChangeToWin();

        percentage -= (Eating.Decay * Time.deltaTime);

        if(Input.GetButtonDown(buttons[qteButton]))
        {
            percentage += (Eating.Increase * Time.deltaTime);

            if (percentage > 1f) percentage = 1f;
        }
    }

    private void UpdateEnd()
    {
        timer += Time.deltaTime;

        if (timer > 3f) ChangeToNotStarted();
    }

    private bool InsideCircle(Vector3 position)
    {
        return (position - transform.position).magnitude <= 0.44f;
    }

    // Update is called once per frame
    void Update()
    {
        switch(state)
        {
            case State.NotStarted:
                UpdateNotStarted();
                break;

            case State.Phase1:
                UpdatePhase1();
                break;

            case State.Phase2:
                UpdatePhase2();
                break;

            case State.Win:
            case State.Lose:
                UpdateEnd();
                break;

            default:
                break;
        }
    }

    void OnGUI()
    {
        if(state == State.NotStarted)
        {
            GUI.skin = Skin;
            GUI.DrawTexture(new Rect(24f, 24f, 48f, 48f), Textures.Buttons[0]);
            GUI.Label(new Rect(80f, 24f, 200f, 48f), "Start Minigame");
            GUI.DrawTexture(new Rect(24f, 80f, 48f, 48f), Textures.Buttons[1]);
            GUI.Label(new Rect(80f, 80f, 200f, 48f), "Number of NPC's Grabbed: " + npcsGrabbed);
        }
        else if(state == State.Phase2)
        {
            Texture button = Textures.Buttons[qteButton];
            float x = (Screen.width - Textures.BarBackground.width - button.width) / 2f;
            float y = 180f;

            GUI.DrawTexture(new Rect(x, y, Textures.BarBackground.width, Textures.BarBackground.height), Textures.BarBackground);
            GUI.DrawTexture(new Rect(x + 5f, y + 5f, Textures.BarForeground.width * percentage, Textures.BarForeground.height), Textures.BarForeground);
            GUI.DrawTexture(new Rect(x + Textures.BarBackground.width, y, button.width, button.height), button);
        }
        else if(state == State.Win)
        {
            string text = (npcsGrabbed > 1) ? "Souls Consumed" : "Soul Consumed";
            GUI.skin = EndSkin;
            float x = 0f;
            float y = 92f;
            GUI.Label(new Rect(x, y, Screen.width, 96f), text);
        }
        else if(state == State.Lose)
        {
            string text = (npcsGrabbed > 1) ? "NPCs Escaped" : "NPC Escaped";
            GUI.skin = EndSkin;
            float x = 0f;
            float y = 92f;
            GUI.Label(new Rect(x, y, Screen.width, 96f), text);
        }
    }

    private void OnDestroy()
    {
        GamePad.SetVibration(PlayerIndex.One, 0f, 0f);
    }
}
