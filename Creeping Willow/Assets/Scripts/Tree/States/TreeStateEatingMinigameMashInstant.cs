using UnityEngine;

public class TreeStateEatingMinigameMashInstant : TreeState
{
    private static string[] Buttons = { "A", "B", "X", "Y" };
    private static Color[] Colors = { new Color(0.57f, 0.69f, 0.4f), new Color(0.75f, 0.42f, 0.28f), new Color(0.26f, 0.5f, 0.69f), new Color(0.86f, 0.73f, 0.24f) };
    private const float SampleRate = 3f;


    private GameObject npc;
    private NPCData npcData;
    private int button;
    private float percentage, timeElapsed, sampledTime, averagePercentage;
    private int intPercentage, ticks;
    private bool won;


    public override void Enter(object data)
    {
        GlobalGameStateManager.PosessionState = PosessionState.NON_EXORCISABLE;
        Tree.Eating = true;

        Tree.BodyParts.LeftArm.SetActive(false);
        Tree.BodyParts.RightArm.SetActive(false);
        Tree.BodyParts.RightUpperArm.SetActive(true);
        Tree.BodyParts.LeftUpperArm.SetActive(true);

        Tree.BodyParts.Face.GetComponent<SpriteRenderer>().sprite = Tree.Sprites.Face.Crazy;

        // Choose a random button
        float range = Random.Range(0f, 1f);

        if (range <= 0.25f) button = 1;
        else if (range > 0.25f && range <= 0.5f) button = 2;
        else if (range > 0.5f && range <= 0.75f) button = 0;
        else if (range > 0.75f && range <= 1f) button = 3;

        // Get data
        Data parameters = data as Data;

        npc = parameters.NPC;
        npcData = GlobalGameStateManager.NPCData[npc.GetComponent<AIController>().SkinType];

        Tree.BodyParts.RightGrabbedNPC.GetComponent<Animator>().SetTrigger(npcData.AnimationTrigger);

        MessageCenter.Instance.Broadcast(new CameraChangeFollowedMessage(Tree.BodyParts.MinigameCircle.transform, Vector3.zero));
        MessageCenter.Instance.Broadcast(new CameraZoomMessage(1.2f, 20f));

        UpdateArms(1f);

		SoundManager soundManager = GameObject.FindObjectOfType<SoundManager>();
		soundManager.PauseMusic();

        Tree.audio.clip = Tree.Sounds.Music;
        Tree.audio.Play();

        won = false;
    }

    public override void Update()
    {
        if (Camera.main.orthographicSize != Camera.main.GetComponent<CameraScript>().TargetSize) return;

        if (won)
        {
            TreeStateEating.Data data = new TreeStateEating.Data(npc);

            Tree.ChangeState("Eating", data);

            return;
        }

        if (Input.GetButtonDown(Buttons[button])) won = true;
    }
    protected void UpdateArms(float percentage)
    {
        float upperAngle = npcData.RightUpperArmMidpointAngle + ((npcData.RightUpperArmEndAngle - npcData.RightUpperArmMidpointAngle) * percentage);
        float lowerAngle = npcData.RightLowerArmMidpointAngle + ((npcData.RightLowerArmEndAngle - npcData.RightLowerArmMidpointAngle) * percentage);

        Tree.BodyParts.RightUpperArm.transform.localEulerAngles = new Vector3(0f, 0f, upperAngle);
        Tree.BodyParts.RightLowerForegroundArm.transform.localEulerAngles = new Vector3(0f, 0f, lowerAngle);
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
        Vector3 position = Camera.main.WorldToScreenPoint(Tree.BodyParts.MinigameCircle.transform.position + new Vector3(0f, 0.6f));

        GUI.DrawTexture(new Rect(position.x - (width / 2f), position.y - (height / 2f), width, height), Tree.Sprites.EatingMinigame.Buttons[button]);
    }

    public override void Leave()
    {

    }


    public class Data
    {
        public GameObject NPC;


        public Data(GameObject npc)
        {
            NPC = npc;
        }
    }
}
