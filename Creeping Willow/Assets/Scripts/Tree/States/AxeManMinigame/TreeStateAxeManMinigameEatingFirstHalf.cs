using UnityEngine;

public class TreeStateAxeManMinigameEatingFirstHalf : TreeState
{
    private GameObject axeMan;
    private float timeElapsed;
    private int frame;
    private float frameTimer;


    public override void Enter(object data)
    {        
        Tree.BodyParts.Eyes.SetActive(true);
        //Tree.BodyParts.RightGrabbedNPC.SetActive(false);
        Tree.BodyParts.MinigameCircle.SetActive(false);

        // Get parameters
        Data parameters = data as Data;

        axeMan = parameters.AxeMan;

        // Set arm angles
        /*Tree.BodyParts.RightUpperArm.transform.eulerAngles = new Vector3(0f, 0f, npcData.RightUpperArmEndAngle);
        Tree.BodyParts.RightLowerForegroundArm.transform.eulerAngles = new Vector3(0f, 0f, npcData.RightLowerArmEndAngle);

        // Play chew sound
        //if (Tree.audio.isPlaying) Tree.audio.Stop();

        Tree.audio.clip = Tree.Sounds.Chew;

        Tree.audio.Play();

        timeElapsed = 0f;
    }

    private void Eat()
    {        
        GlobalGameStateManager.SoulConsumedTimer = 3.5f;

        Tree.audio.Stop();
        Tree.audio.clip = Tree.Sounds.SoulConsumed;
        Tree.audio.Play();

        MessageCenter.Instance.Broadcast(new NPCEatenMessage(npc));
        MessageCenter.Instance.Broadcast(new CameraChangeFollowedMessage(Tree.transform, new Vector3(0f, 0.15f)));
        MessageCenter.Instance.Broadcast(new CameraZoomMessage(4f, 10f));

        if(npc.GetComponent<AIController>().isCritterType)
        {
            switch(npc.GetComponent<CritterController>().critterUpgradeType)
            {
                case CritterType.poisonous:
                    Tree.BonusPoisonTimer = Tree.MaxBonusTime;
                    break;

                default:
                    Tree.BonusSpeedTimer = Tree.MaxBonusTime;
                    break;
            }
        }

        GameObject.Destroy(npc);
    }

    public override void Update()
    {
        if(!Tree.audio.isPlaying)
        {
            Eat();
            Tree.ChangeState("Active");

            return;
        }
        
        // Update arm rotation
        if(timeElapsed < npcData.ArmEatTime)
            timeElapsed += Time.deltaTime;

        float percentage = Mathf.Clamp(timeElapsed / npcData.ArmEatTime, 0f, 1f);

        float upperAngle = npcData.RightUpperArmEndAngle + ((npcData.RightUpperArmFinalAngle - npcData.RightUpperArmEndAngle) * percentage);
        float lowerAngle = npcData.RightLowerArmEndAngle + ((npcData.RightLowerArmFinalAngle - npcData.RightLowerArmEndAngle) * percentage);

        Tree.BodyParts.RightUpperArm.transform.localEulerAngles = new Vector3(0f, 0f, upperAngle);
        Tree.BodyParts.RightLowerForegroundArm.transform.localEulerAngles = new Vector3(0f, 0f, lowerAngle);
    }

    public override void UpdateSorting()
    {
        Tree.BodyParts.Trunk.GetComponent<SpriteRenderer>().sortingOrder = 800;
        
        int i = Tree.BodyParts.Trunk.GetComponent<SpriteRenderer>().sortingOrder;

        //Tree.BodyParts.FlameEyes.particleSystem.renderer.sortingOrder = i + 1;
        Tree.BodyParts.Eyes.GetComponent<SpriteRenderer>().sortingOrder = i + 2;
        Tree.BodyParts.Face.GetComponent<SpriteRenderer>().sortingOrder = i + 4;
        //Tree.BodyParts.LeftArm.GetComponent<SpriteRenderer>().sortingOrder = i + 1;
        Tree.BodyParts.RightUpperArm.GetComponent<SpriteRenderer>().sortingOrder = i + 2;
        Tree.BodyParts.RightLowerForegroundArm.GetComponent<SpriteRenderer>().sortingOrder = i + 3;
        Tree.BodyParts.RightLowerBackgroundArm.GetComponent<SpriteRenderer>().sortingOrder = i + 5;
        Tree.BodyParts.Legs.GetComponent<SpriteRenderer>().sortingOrder = i - 1;
    }

    public override void Leave()
    {
        Tree.BodyParts.Eyes.SetActive(false);
        Tree.BodyParts.Face.GetComponent<Animator>().enabled = false;
        Tree.BodyParts.RightGrabbedNPC.SetActive(true);
        //Tree.BodyParts.Face.SetActive(true);
        //Tree.BodyParts.GrabbedNPC.SetActive(true);*/
    }

    public class Data
    {
        public GameObject AxeMan;


        public Data(GameObject axeMan)
        {
            AxeMan = axeMan;
        }
    }
}
