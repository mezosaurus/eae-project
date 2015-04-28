using UnityEngine;

public class TreeStateAxeManMinigameMash : TreeState
{
    private const float UpperArmStartAngle = 357.470825f;
    private const float LowerArmStartAngle = 273.74145f;
    private const float UpperArmEndAngle = 377.36395f;
    private const float LowerArmEndAngle = 302.2439f;

    private static string[] Buttons = { "A", "B", "X", "Y" };
    private static Color[] Colors = { new Color(0.57f, 0.69f, 0.4f), new Color(0.75f, 0.42f, 0.28f), new Color(0.26f, 0.5f, 0.69f), new Color(0.86f, 0.73f, 0.24f) };
    private const float SampleRate = 3f;


    private GameObject axeMan;
    private GameObject buttonIcon;
    private int button;
    private float percentage, timeElapsed, sampledTime, averagePercentage;
    private int intPercentage, ticks;
    private bool won;
    private bool started;
    private float buttonScale, buttonScaleDirection;


    public override void Enter(object data)
    {
        // Choose a random button
        float range = Random.Range(0f, 1f);

        if (range <= 0.25f) button = 1;
        else if (range > 0.25f && range <= 0.5f) button = 2;
        else if (range > 0.5f && range <= 0.75f) button = 0;
        else if (range > 0.75f && range <= 1f) button = 3;

        /*buttonIcon = (GameObject)GameObject.Instantiate(Tree.Prefabs.A, Tree.BodyParts.MinigameCircle.transform.position + new Vector3(0f, -0.64f), Quaternion.identity);

        buttonIcon.GetComponent<SpriteRenderer>().sprite = Tree.Sprites.EatingMinigame.ButtonSprites[button];
        buttonIcon.GetComponent<SpriteRenderer>().color = new Color(1f, 1f, 1f, 1f);
        buttonIcon.transform.localScale = new Vector3(0.5f, 0.5f, 1f);*/

        //Tree.BodyParts.MinigameCircle.GetComponent<SpriteRenderer>().sprite = Tree.Sprites.EatingMinigame.Circle[Percentage];
        Tree.BodyParts.MinigameCircle.GetComponent<SpriteRenderer>().color = Colors[button];
        MessageCenter.Instance.Broadcast(new CameraZoomAndFocusMessage(Tree.BodyParts.MinigameCircle.transform.position, 0.25f, 1.2f, 15f));

        // Get data
        Data parameters = data as Data;

        axeMan = parameters.AxeMan;
        percentage = (float)parameters.Percentage / 100f;
        timeElapsed = 0f;

        // Adjust the percentage to be based off how well the player did, but harder
        // (the percentage from the previous phase will always be low, so boost it a bit to make it more fair)
        percentage = 0.6f + ((1f - percentage) * 0.35f);

        UpdateArms(percentage);

        sampledTime = 0f;
        averagePercentage = 0f;
        ticks = 0;
        started = false;

        won = false;
        buttonScale = 1f;
        buttonScaleDirection = 1f;
    }

    /*public override void Update()
    {
        if (Camera.main.orthographicSize != Camera.main.GetComponent<CameraScript>().TargetSize) return;

        if(won)
        {
           TreeStateEating.Data data = new TreeStateEating.Data(npc);
            
           // Tree.ChangeState("Eating", data);

            return;
        }

        float decrease = 0.52f * Time.deltaTime;// (percentage > 0.9f) ? 0.08f * Time.deltaTime : 0.48f * Time.deltaTime;
        float increase = 0f;

        if (Input.GetButtonDown(Buttons[button])) increase = 8f * Time.deltaTime;

        percentage += (decrease - increase);

        if (percentage >= 1f)
        {
            Lose();

            return;
        }

        if (percentage <= 0f)
        {
            percentage = 0f;
            won = true;
        }

        // Update circle
        intPercentage = Mathf.RoundToInt(percentage * 100f);

        Tree.BodyParts.MinigameCircle.GetComponent<SpriteRenderer>().sprite = Tree.Sprites.EatingMinigame.Circle[intPercentage];

        // Update arms
        sampledTime += Time.deltaTime;
        averagePercentage += percentage;
        ticks++;

        if(sampledTime > (1f / 60f))
        {
            averagePercentage /= ticks;

            UpdateArms(1f - averagePercentage);

            averagePercentage = 0f;
            sampledTime = 0f;
            ticks = 0;
        }
    }*/

    public override void Update()
    {
        timeElapsed += Time.deltaTime;

        buttonScale += (Time.deltaTime * buttonScaleDirection * 2f);

        if (buttonScale > 1.25f)
        {
            buttonScale = 1.25f;
            buttonScaleDirection = -1f;
        }

        if (buttonScale < 0.75f)
        {
            buttonScale = 0.75f;
            buttonScaleDirection = 1f;
        }
    }

    public override void FixedUpdate()
    {
        if (Camera.main.orthographicSize != Camera.main.GetComponent<CameraScript>().TargetSize) return;
        if (!started) started = true;

        if(won)
        {
            /*TreeStateEating.Data data = new TreeStateEating.Data(npc);*/
            
            Tree.ChangeState("AxeManMinigameEatingFirstHalf");

            return;
        }

        float decrease = 0.3f * Time.fixedDeltaTime; // *Time.deltaTime;// (percentage > 0.9f) ? 0.08f * Time.deltaTime : 0.48f * Time.deltaTime;
        float increase = 0f;

        if (Input.GetButtonDown(Buttons[button])) increase = 6.4f * Time.fixedDeltaTime; // *Time.deltaTime;

        percentage += (decrease - increase);

        if (percentage >= 1f)
        {
            Lose();

            return;
        }

        if (percentage <= 0f)
        {
            percentage = 0f;
            won = true;
        }

        // Update circle
        intPercentage = Mathf.RoundToInt(percentage * 100f);

        Tree.BodyParts.MinigameCircle.GetComponent<SpriteRenderer>().sprite = Tree.Sprites.EatingMinigame.Circle[intPercentage];

        // Update arms
        sampledTime += Time.deltaTime;
        averagePercentage += percentage;
        ticks++;

        if(sampledTime > (1f / 60f))
        {
            averagePercentage /= ticks;

            UpdateArms(1f - averagePercentage);

            averagePercentage = 0f;
            sampledTime = 0f;
            ticks = 0;
        }
    }

    protected void UpdateArms(float percentage)
    {
        float upperAngle = UpperArmStartAngle + ((UpperArmEndAngle - UpperArmStartAngle) * percentage);
        float lowerAngle = LowerArmStartAngle + ((LowerArmEndAngle - LowerArmStartAngle) * percentage);

        Tree.BodyParts.RightUpperArm.transform.localEulerAngles = new Vector3(0f, 0f, upperAngle);
        Tree.BodyParts.RightLowerForegroundArm.transform.localEulerAngles = new Vector3(0f, 0f, lowerAngle);
    }

    protected void Lose()
    {
        Tree.BodyParts.RightLowerBackgroundArm.transform.localEulerAngles = new Vector3(0f, 0f, 0f);
        Tree.BodyParts.Axe.SetActive(false);

        Tree.audio.Stop();

        MessageCenter.Instance.Broadcast(new AxeManMinigameAxeManChangePhaseMessage(8001));

        Tree.ChangeState("AxeManMinigameDead");
    }

    public override void UpdateSorting()
    {
        /*Tree.BodyParts.Trunk.GetComponent<SpriteRenderer>().sortingOrder = 800;
        
        int i = Tree.BodyParts.Trunk.GetComponent<SpriteRenderer>().sortingOrder;

        Tree.BodyParts.Face.GetComponent<SpriteRenderer>().sortingOrder = i + 1;
        //Tree.BodyParts.LeftArm.GetComponent<SpriteRenderer>().sortingOrder = i + 1;
        Tree.BodyParts.RightUpperArm.GetComponent<SpriteRenderer>().sortingOrder = i + 1;
        Tree.BodyParts.RightLowerForegroundArm.GetComponent<SpriteRenderer>().sortingOrder = i + 4;
        Tree.BodyParts.RightLowerBackgroundArm.GetComponent<SpriteRenderer>().sortingOrder = i + 2;
        Tree.BodyParts.Legs.GetComponent<SpriteRenderer>().sortingOrder = i - 1;
        Tree.BodyParts.MinigameCircle.GetComponent<SpriteRenderer>().sortingOrder = i + 7;
        axeMan.GetComponent<SpriteRenderer>().sortingOrder = i + 3;*/

        Tree.BodyParts.Trunk.GetComponent<SpriteRenderer>().sortingOrder = 800;

        int i = Tree.BodyParts.Trunk.GetComponent<SpriteRenderer>().sortingOrder;

        Tree.BodyParts.Face.GetComponent<SpriteRenderer>().sortingOrder = i + 1;
        Tree.BodyParts.LeftUpperArm.GetComponent<SpriteRenderer>().sortingOrder = i + 1;
        Tree.BodyParts.LeftLowerForegroundArm.GetComponent<SpriteRenderer>().sortingOrder = i + 2;
        Tree.BodyParts.LeftLowerBackgroundArm.GetComponent<SpriteRenderer>().sortingOrder = i + 1;
        Tree.BodyParts.RightUpperArm.GetComponent<SpriteRenderer>().sortingOrder = i + 1;
        Tree.BodyParts.RightLowerForegroundArm.GetComponent<SpriteRenderer>().sortingOrder = i + 4;
        Tree.BodyParts.RightLowerBackgroundArm.GetComponent<SpriteRenderer>().sortingOrder = i + 2;
        Tree.BodyParts.Legs.GetComponent<SpriteRenderer>().sortingOrder = i - 1;
        Tree.BodyParts.MinigameCircle.GetComponent<SpriteRenderer>().sortingOrder = i + 7;
        Tree.BodyParts.Axe.GetComponent<SpriteRenderer>().sortingOrder = i + 3;
        Tree.AxeMan.GetComponent<SpriteRenderer>().sortingOrder = i + 3;
    }

    public override void OnGUI()
    {
        float width = Tree.Sprites.EatingMinigame.Buttons[0].width * buttonScale;
        float height = Tree.Sprites.EatingMinigame.Buttons[0].height * buttonScale;
        Vector3 position = Camera.main.WorldToScreenPoint(Tree.BodyParts.MinigameCircle.transform.position + new Vector3(0f, 0.6f));

        GUI.DrawTexture(new Rect(position.x - (width / 2f), position.y - (height / 2f), width, height), Tree.Sprites.EatingMinigame.Buttons[button]);
    }

    public override void Leave()
    {
        //GameObject.Destroy(buttonIcon);
        //Tree.audio.Stop();
    }


    public class Data
    {
        public GameObject AxeMan;
        public int Percentage;


        public Data(GameObject axeMan, int percentage)
        {
            AxeMan = axeMan;
            Percentage = percentage;
        }
    }
}
