using UnityEngine;

public class TreeStateEatingMinigameMash : TreeState
{
    private static string[] Buttons = { "A", "B", "X", "Y" };
    private static Color[] Colors = { new Color(0.57f, 0.69f, 0.4f), new Color(0.75f, 0.42f, 0.28f), new Color(0.26f, 0.5f, 0.69f), new Color(0.86f, 0.73f, 0.24f) };
    private const float SampleRate = 3f;


    private NPCData npcData;
    private int button;
    private float percentage, timeElapsed, sampledTime, averagePercentage;
    private int intPercentage, ticks;
    private bool won;


    public override void Enter(object data)
    {
        GlobalGameStateManager.PosessionState = PosessionState.NON_EXORCISABLE;

        // Choose a random button
        float range = Random.Range(0f, 1f);

        if (range <= 0.25f) button = 1;
        else if (range > 0.25f && range <= 0.5f) button = 2;
        else if (range > 0.5f && range <= 0.75f) button = 0;
        else if (range > 0.75f && range <= 1f) button = 3;

        //Tree.BodyParts.MinigameCircle.GetComponent<SpriteRenderer>().sprite = Tree.Sprites.EatingMinigame.Circle[Percentage];
        Tree.BodyParts.MinigameCircle.GetComponent<SpriteRenderer>().color = Colors[button];

        MessageCenter.Instance.Broadcast(new CameraZoomMessage(1.2f, 20f));

        // Get data
        Data parameters = data as Data;

        npcData = GlobalGameStateManager.NPCData[parameters.SkinType];

        percentage = (float)parameters.Percentage / 100f;
        timeElapsed = 0f;

        UpdateArms(1f - percentage);

        sampledTime = 0f;
        averagePercentage = 0f;
        ticks = 0;

        won = false;
    }

    public override void Update()
    {
        if (Camera.main.orthographicSize != Camera.main.GetComponent<CameraScript>().TargetSize) return;

        if(won)
        {
            TreeStateEating.Data data = new TreeStateEating.Data(npcData.SkinType);

            Tree.ChangeState("Eating", data);

            return;
        }
        
        float decrease = (percentage > 0.9f) ? 0.08f * Time.deltaTime : 0.48f * Time.deltaTime;
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
    }
    protected void UpdateArms(float percentage)
    {
        float upperAngle = npcData.RightUpperArmMidpointAngle + ((npcData.RightUpperArmEndAngle - npcData.RightUpperArmMidpointAngle) * percentage);
        float lowerAngle = npcData.RightLowerArmMidpointAngle + ((npcData.RightLowerArmEndAngle - npcData.RightLowerArmMidpointAngle) * percentage);

        Tree.BodyParts.RightUpperArm.transform.localEulerAngles = new Vector3(0f, 0f, upperAngle);
        Tree.BodyParts.RightLowerForegroundArm.transform.localEulerAngles = new Vector3(0f, 0f, lowerAngle);
    }

    protected void Lose()
    {
        MessageCenter.Instance.Broadcast(new CameraZoomMessage(4f, 20f));

        Tree.ChangeState("Active");
    }

    public override void UpdateSorting()
    {
        Tree.BodyParts.Trunk.GetComponent<SpriteRenderer>().sortingOrder = 800;
        
        int i = Tree.BodyParts.Trunk.GetComponent<SpriteRenderer>().sortingOrder;

        Tree.BodyParts.Face.GetComponent<SpriteRenderer>().sortingOrder = i + 1;
        //Tree.BodyParts.LeftArm.GetComponent<SpriteRenderer>().sortingOrder = i + 1;
        Tree.BodyParts.RightUpperArm.GetComponent<SpriteRenderer>().sortingOrder = i + 1;
        Tree.BodyParts.RightLowerForegroundArm.GetComponent<SpriteRenderer>().sortingOrder = i + 4;
        Tree.BodyParts.RightLowerBackgroundArm.GetComponent<SpriteRenderer>().sortingOrder = i + 2;
        Tree.BodyParts.Legs.GetComponent<SpriteRenderer>().sortingOrder = i - 1;
        Tree.BodyParts.MinigameCircle.GetComponent<SpriteRenderer>().sortingOrder = i + 7;
        Tree.BodyParts.RightGrabbedNPC.GetComponent<SpriteRenderer>().sortingOrder = i + 3;
    }

    public override void OnGUI()
    {
        int width = Tree.Sprites.EatingMinigame.Buttons[0].width;
        int height = Tree.Sprites.EatingMinigame.Buttons[0].height;
        Vector3 position = Camera.main.WorldToScreenPoint(Tree.BodyParts.MinigameCircle.transform.position + new Vector3(0f, 0.3f));

        GUI.DrawTexture(new Rect(position.x - (width / 2f), position.y - (height / 2f), width, height), Tree.Sprites.EatingMinigame.Buttons[button]);
    }

    public override void Leave()
    {

    }


    public class Data
    {
        public NPCSkinType SkinType;
        public int Percentage;


        public Data(NPCSkinType skinType, int percentage)
        {
            SkinType = skinType;
            Percentage = percentage;
        }
    }
}
