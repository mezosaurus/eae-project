using UnityEngine;

public class TreeStateAxeManMinigameLowerToAxeMan : TreeState
{
    // arm constants
    private const float UpperArmStartAngle = 337.5777f;
    private const float LowerArmStartAngle = 245.239f;
    private const float UpperArmEndAngle = 350.7832f;
    private const float LowerArmEndAngle = 255.4407f;

    private const float MaxTime = 0.2f;

    private float timeElapsed, waitTime;
    private float percentage;
    private GameObject axeMan;


    public override void Enter(object data)
    {
        MessageCenter.Instance.Broadcast(new CameraZoomAndFocusMessage(Tree.transform.position + new Vector3(-0.57f, 0.7f), MaxTime, 2f, 0.8f));

        axeMan = GameObject.FindGameObjectWithTag("AxeManKillActiveTree");

        timeElapsed = 0f;
        waitTime = 0f;
        percentage = 0f;

        UpdateArms(percentage);
    }

    public override void Update()
    {
        timeElapsed += Time.deltaTime;

        percentage = timeElapsed / MaxTime;

        if (percentage > 1f)
        {
            percentage = 1f;

            waitTime += Time.deltaTime;

            if(waitTime > 0.2f)
                Tree.ChangeState("AxeManMinigameRaiseAxeMan", new TreeStateAxeManMinigameRaiseAxeMan.Data(axeMan));
        }

        UpdateArms(percentage);
    }

    protected void UpdateArms(float percentage)
    {
        float upperAngle = UpperArmStartAngle + ((UpperArmEndAngle - UpperArmStartAngle) * percentage);
        float lowerAngle = LowerArmStartAngle + ((LowerArmEndAngle - LowerArmStartAngle) * percentage);

        Tree.BodyParts.RightUpperArm.transform.localEulerAngles = new Vector3(0f, 0f, upperAngle);
        Tree.BodyParts.RightLowerForegroundArm.transform.localEulerAngles = new Vector3(0f, 0f, lowerAngle);
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
        //Tree.BodyParts.Axe.GetComponent<SpriteRenderer>().sortingOrder = i + 6;
        Tree.BodyParts.MinigameCircle.GetComponent<SpriteRenderer>().sortingOrder = i + 7;
        Tree.AxeMan.GetComponent<SpriteRenderer>().sortingOrder = i + 3;*/

        Tree.BodyParts.Trunk.GetComponent<SpriteRenderer>().sortingOrder = 800;

        int i = Tree.BodyParts.Trunk.GetComponent<SpriteRenderer>().sortingOrder;

        Tree.BodyParts.Face.GetComponent<SpriteRenderer>().sortingOrder = i + 1;
        Tree.BodyParts.LeftUpperArm.GetComponent<SpriteRenderer>().sortingOrder = i + 1;
        Tree.BodyParts.RightUpperArm.GetComponent<SpriteRenderer>().sortingOrder = i + 1;
        Tree.BodyParts.RightLowerForegroundArm.GetComponent<SpriteRenderer>().sortingOrder = i + 4;
        Tree.BodyParts.RightLowerBackgroundArm.GetComponent<SpriteRenderer>().sortingOrder = i + 2;
        Tree.BodyParts.Legs.GetComponent<SpriteRenderer>().sortingOrder = i - 1;
        Tree.BodyParts.MinigameCircle.GetComponent<SpriteRenderer>().sortingOrder = i + 7;
        Tree.BodyParts.Axe.GetComponent<SpriteRenderer>().sortingOrder = i + 3;
        Tree.AxeMan.GetComponent<SpriteRenderer>().sortingOrder = i + 3;
    }

    public override void Leave()
    {
        //Tree.BodyParts.RightLowerBackgroundArm.transform.rotation.lo
    }

    public class Data
    {
        public GameObject Axe;

        public Data(GameObject axe)
        {
            Axe = axe;
        }
    }
}
