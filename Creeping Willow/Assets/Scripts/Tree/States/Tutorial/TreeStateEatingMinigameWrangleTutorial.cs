using UnityEngine;

public class TreeStateEatingMinigameWrangleTutorial : TreeState
{
    private const float InRadius = 0.4f;
    private const float MaxThumbStickRadius = 1.5f;
    private const float ForwardForceValue = 0.65f;
    private const float OpposingForceValue = 0.25f;
    private const float MaxTime = 2f * (MaxThumbStickRadius / (ForwardForceValue - OpposingForceValue));


    private bool initialized;
    private GameObject LS, RS, LSArrow, RSArrow;
    private GameObject npc;
    private NPCData npcData;
    private bool grabbedTwo;
    private float timeElapsed;
    private int percentage;


    public override void Enter(object data)
    {
        GlobalGameStateManager.PosessionState = PosessionState.NON_EXORCISABLE;
        GlobalGameStateManager.SoulConsumedTimer = 0f;
        Tree.Eating = true;

        Tree.BodyParts.LeftArm.SetActive(false);
        Tree.BodyParts.RightArm.SetActive(false);
        Tree.BodyParts.RightUpperArm.SetActive(true);
        Tree.BodyParts.LeftUpperArm.SetActive(true);
        Tree.BodyParts.MinigameCircle.SetActive(true);

        Tree.BodyParts.Face.GetComponent<SpriteRenderer>().sprite = Tree.Sprites.Face.Crazy;
        Tree.BodyParts.MinigameCircle.GetComponent<SpriteRenderer>().color = Color.white;

        // Parse data
        Data parameters = data as Data;

        if(parameters.GrabbedNPCs.Length == 1)
        {
            grabbedTwo = false;
        }
        else
        {
            grabbedTwo = true;

            GameObject.Destroy(parameters.GrabbedNPCs[1]);
        }

        npc = parameters.GrabbedNPCs[0];
        npcData = GlobalGameStateManager.NPCData[parameters.GrabbedNPCs[0].GetComponent<AIController>().SkinType];

        Tree.BodyParts.RightGrabbedNPC.GetComponent<Animator>().SetTrigger(npcData.AnimationTrigger);

        MessageCenter.Instance.Broadcast(new CameraChangeFollowedMessage(Tree.BodyParts.MinigameCircle.transform, Vector3.zero));
        MessageCenter.Instance.Broadcast(new CameraZoomMessage(1.8f, 20f));

        // Play music if necessary
        if (parameters.PlayMusic)
        {
        	SoundManager soundManager = GameObject.FindObjectOfType<SoundManager>();
			soundManager.PauseMusic();

            Tree.audio.clip = Tree.Sounds.Music;
            Tree.audio.Play();
        }

        // Handle tutorial
        if (TutorialManager.Instance.Phase == 0 || TutorialManager.Instance.Phase == 6 || TutorialManager.Instance.Phase == 12 || TutorialManager.Instance.Phase == 16)
            TutorialManager.Instance.AdvancePhase();

        initialized = false;

        Initialize();
    }

    private void Initialize()
    {
        // Create thumbsticks
        Vector3 lsOffset = Quaternion.Euler(0f, 0f, Random.Range(45f, 135f)) * new Vector3(0f, MaxThumbStickRadius);
        Vector3 rsOffset = Quaternion.Euler(0f, 0f, Random.Range(45f, 135f)) * new Vector3(0f, -MaxThumbStickRadius);

        LS = (GameObject)GameObject.Instantiate(Tree.Prefabs.ThumbStick, Tree.BodyParts.MinigameCircle.transform.position + lsOffset, Quaternion.identity);
        RS = (GameObject)GameObject.Instantiate(Tree.Prefabs.ThumbStick, Tree.BodyParts.MinigameCircle.transform.position + rsOffset, Quaternion.identity);

        LS.GetComponent<SpriteRenderer>().sprite = Tree.Sprites.EatingMinigame.LS;
        RS.GetComponent<SpriteRenderer>().sprite = Tree.Sprites.EatingMinigame.RS;

        LSArrow = LS.transform.GetChild(0).gameObject;
        RSArrow = RS.transform.GetChild(0).gameObject;

        Tree.BodyParts.MinigameCircle.GetComponent<SpriteRenderer>().sprite = Tree.Sprites.EatingMinigame.Circle[0];

        timeElapsed = 0f;

        initialized = true;
    }

    public override void Update()
    {
        if (Camera.main.orthographicSize != Camera.main.GetComponent<CameraScript>().TargetSize) return;
        //if (!initialized) Initialize();

        Transform circle = Tree.BodyParts.MinigameCircle.transform;

        // Determine whether thumbsticks are in the circle
        float lsDistance = Vector3.Distance(circle.position, LS.transform.position);
        float rsDistance = Vector3.Distance(circle.position, RS.transform.position);

        bool lsIn = (lsDistance <= InRadius);
        bool rsIn = (rsDistance <= InRadius);

        // If both sticks are in, change to next phase of minigame
        if (lsIn && rsIn)
        {
            if (TutorialManager.Instance.Phase == 1) percentage = 75;
            
            TreeStateEatingMinigameMashTutorial.Data data = new TreeStateEatingMinigameMashTutorial.Data(npc, percentage);
            
            Tree.ChangeState("EatingMinigameMash", data);

            return;
        }

        Vector3 lsDifference = circle.position - LS.transform.position;
        Vector3 rsDifference = circle.position - RS.transform.position;

        // Apply forces if necessary
        if(!lsIn || !rsIn)
        {
            if (lsDistance < MaxThumbStickRadius)
            {
                LS.transform.position -= (lsDifference.normalized * OpposingForceValue * Time.deltaTime);
            }

            if (rsDistance < MaxThumbStickRadius)
            {
                RS.transform.position -= (rsDifference.normalized * OpposingForceValue * Time.deltaTime);
            }
        }

        // Move the user toward their goal
        LS.transform.position += (new Vector3(Input.GetAxis("LSX"), Input.GetAxis("LSY")).normalized * ForwardForceValue * Time.deltaTime);
        RS.transform.position += (new Vector3(Input.GetAxis("RSX"), Input.GetAxis("RSY")).normalized * ForwardForceValue * Time.deltaTime);

        // Make sure the user can't go too far and make things even more difficult
        if (Vector3.Distance(circle.position, LS.transform.position) > MaxThumbStickRadius)
        {
            LS.transform.position = circle.position - ((circle.position - LS.transform.position).normalized * MaxThumbStickRadius);
        }

        if (Vector3.Distance(circle.position, RS.transform.position) > MaxThumbStickRadius)
        {
            RS.transform.position = circle.position - ((circle.position - RS.transform.position).normalized * MaxThumbStickRadius);
        }

        // Update arrows
        LSArrow.GetComponent<SpriteRenderer>().enabled = (lsIn) ? false : true;
        RSArrow.GetComponent<SpriteRenderer>().enabled = (rsIn) ? false : true;

        float lsAngle = Mathf.Atan2(lsDifference.y, lsDifference.x) * Mathf.Rad2Deg;
        float rsAngle = Mathf.Atan2(rsDifference.y, rsDifference.x) * Mathf.Rad2Deg;

        LSArrow.transform.eulerAngles = new Vector3(0f, 0f, lsAngle);
        RSArrow.transform.eulerAngles = new Vector3(0f, 0f, rsAngle);

        // Update arms based on user progress
        float lsProgress = Mathf.Clamp(1f - ((Vector3.Distance(circle.position, LS.transform.position) - InRadius) / (MaxThumbStickRadius - InRadius)), 0f, 1f);
        float rsProgress = Mathf.Clamp(1f - ((Vector3.Distance(circle.position, RS.transform.position) - InRadius) / (MaxThumbStickRadius - InRadius)), 0f, 1f);
        float progress = (lsProgress + rsProgress) / 2f;

        UpdateArms(progress);

        // Update timer
        if(TutorialManager.Instance.Phase > 1 && percentage < 90)
            timeElapsed += Time.deltaTime;

        float timePercentage = Mathf.Clamp(timeElapsed / MaxTime, 0f, 1f);
        percentage = Mathf.RoundToInt(timePercentage * 100f);

        Tree.BodyParts.MinigameCircle.GetComponent<SpriteRenderer>().sprite = Tree.Sprites.EatingMinigame.Circle[percentage];

        // To-do
        if (timePercentage >= 1f)
        {
            Lose();

            return;
        }
    }

    protected void UpdateArms(float percentage)
    {
        float upperAngle = npcData.RightUpperArmStartAngle + ((npcData.RightUpperArmMidpointAngle - npcData.RightUpperArmStartAngle) * percentage);
        float lowerAngle = npcData.RightLowerArmStartAngle + ((npcData.RightLowerArmMidpointAngle - npcData.RightLowerArmStartAngle) * percentage);

        Tree.BodyParts.RightUpperArm.transform.localEulerAngles = new Vector3(0f, 0f, upperAngle);
        Tree.BodyParts.RightLowerForegroundArm.transform.localEulerAngles = new Vector3(0f, 0f, lowerAngle);
    }

    protected void Lose()
    {
        npc.SetActive(true);
        npc.GetComponent<AIController>().IsTaggedByTree = true;

        MessageCenter.Instance.Broadcast(new CameraZoomMessage(4f, 20f));
        MessageCenter.Instance.Broadcast(new PlayerReleasedNPCsMessage(new System.Collections.Generic.List<GameObject>() { npc }));

        npc.transform.position = Tree.transform.position + new Vector3(-1f, -0.25f);

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

    public override void Leave()
    {
        GameObject.Destroy(LS);
        GameObject.Destroy(RS);

        Tree.BodyParts.MinigameCircle.GetComponent<SpriteRenderer>().sprite = Tree.Sprites.EatingMinigame.Circle[100];
    }

    
    public class Data
    {
        public GameObject[] GrabbedNPCs;
        public bool PlayMusic;

        public Data(GameObject[] grabbedNPCs, bool playMusic)
        {
            GrabbedNPCs = grabbedNPCs;
            PlayMusic = playMusic;
        }
    }
}
