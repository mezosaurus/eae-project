using UnityEngine;

public class TreeStateEating : TreeState
{
    private NPCData npcData;
    private float timeElapsed;


    public override void Enter(object data)
    {
        GlobalGameStateManager.PosessionState = PosessionState.NON_EXORCISABLE;
        
        Tree.BodyParts.Eyes.SetActive(true);
        Tree.BodyParts.RightGrabbedNPC.SetActive(false);
        Tree.BodyParts.MinigameCircle.SetActive(false);

        // Get parameters
        Data parameters = data as Data;

        npcData = GlobalGameStateManager.NPCData[parameters.SkinType];

        Tree.BodyParts.Face.GetComponent<Animator>().enabled = true;
        Tree.BodyParts.Face.GetComponent<SpriteRenderer>().sprite = Tree.Sprites.Face.Crazy;
        Tree.BodyParts.Face.GetComponent<Animator>().SetTrigger(npcData.AnimationTrigger);

        // Set arm angles
        Tree.BodyParts.RightUpperArm.transform.eulerAngles = new Vector3(0f, 0f, npcData.RightUpperArmEndAngle);
        Tree.BodyParts.RightLowerForegroundArm.transform.eulerAngles = new Vector3(0f, 0f, npcData.RightLowerArmEndAngle);

        // Play chew sound
        Tree.audio.clip = Tree.Sounds.Chew;

        Tree.audio.Play();

        timeElapsed = 0f;
    }

    public override void Update()
    {
        if(!Tree.audio.isPlaying)
        {
            MessageCenter.Instance.Broadcast(new CameraChangeFollowedMessage(Tree.transform, new Vector3(0f, 0.15f)));
            MessageCenter.Instance.Broadcast(new CameraZoomMessage(4f, 10f));

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

        Tree.BodyParts.Eyes.GetComponent<SpriteRenderer>().sortingOrder = i + 1;
        Tree.BodyParts.Face.GetComponent<SpriteRenderer>().sortingOrder = i + 3;
        //Tree.BodyParts.LeftArm.GetComponent<SpriteRenderer>().sortingOrder = i + 1;
        Tree.BodyParts.RightUpperArm.GetComponent<SpriteRenderer>().sortingOrder = i + 1;
        Tree.BodyParts.RightLowerForegroundArm.GetComponent<SpriteRenderer>().sortingOrder = i + 2;
        Tree.BodyParts.RightLowerBackgroundArm.GetComponent<SpriteRenderer>().sortingOrder = i + 4;
        Tree.BodyParts.Legs.GetComponent<SpriteRenderer>().sortingOrder = i - 1;
    }

    public override void Leave()
    {
        Tree.BodyParts.Eyes.SetActive(false);
        Tree.BodyParts.Face.GetComponent<Animator>().enabled = false;
        Tree.BodyParts.RightGrabbedNPC.SetActive(true);
        //Tree.BodyParts.Face.SetActive(true);
        //Tree.BodyParts.GrabbedNPC.SetActive(true);
    }

    public class Data
    {
        public NPCSkinType SkinType;


        public Data(NPCSkinType skinType)
        {
            SkinType = skinType;
        }
    }
}
