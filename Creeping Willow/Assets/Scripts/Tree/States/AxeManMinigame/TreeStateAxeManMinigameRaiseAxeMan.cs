using UnityEngine;

public class TreeStateAxeManMinigameRaiseAxeMan : TreeState
{
    // arm constants
    private const float UpperArmStartAngle = 350.7832f;
    private const float LowerArmStartAngle = 255.4407f;
    private const float UpperArmEndAngle = 337.5777f;
    private const float LowerArmEndAngle = 245.239f;

    private const float MaxTime = 0.2f;

    private float timeElapsed;
    private float percentage;
    private GameObject axeMan;

    private Vector3 axeManFrom, axeManTo;
    private float axeManAngleFrom, axeManAngleTo;


    public override void Enter(object data)
    {
        MessageCenter.Instance.Broadcast(new CameraZoomAndFocusMessage(Tree.transform.position + new Vector3(0f, 0.7f), MaxTime, 1.8f, 0.8f));

        axeMan = (data as Data).AxeMan;

        // get axe man info
        axeMan.transform.SetParent(Tree.BodyParts.RightLowerForegroundArm.transform);

        axeManFrom = axeMan.transform.localPosition;
        //axeManTo = new Vector3(0.4297077f, -0.218f);
        axeManTo = new Vector3(0.383f, -0.238f);

        axeManAngleFrom = axeMan.transform.localEulerAngles.z;
        axeManAngleTo = 153.5345f;

        timeElapsed = 0f;
        percentage = 0f;

        UpdateArms(percentage);
        UpdateAxeMan(percentage);
    }

    public override void Update()
    {
        timeElapsed += Time.deltaTime;

        percentage = timeElapsed / MaxTime;

        if (percentage > 1f)
        {
            percentage = 1f;

            Tree.ChangeState("AxeManMinigameWrangleAxeMan", new TreeStateAxeManMinigameWrangleAxeMan.Data(axeMan));
        }

        UpdateArms(percentage);
        UpdateAxeMan(percentage);
    }

    protected void UpdateArms(float percentage)
    {
        float upperAngle = UpperArmStartAngle + ((UpperArmEndAngle - UpperArmStartAngle) * percentage);
        float lowerAngle = LowerArmStartAngle + ((LowerArmEndAngle - LowerArmStartAngle) * percentage);

        Tree.BodyParts.RightUpperArm.transform.localEulerAngles = new Vector3(0f, 0f, upperAngle);
        Tree.BodyParts.RightLowerForegroundArm.transform.localEulerAngles = new Vector3(0f, 0f, lowerAngle);
    }

    protected void UpdateAxeMan(float percentage)
    {
        Vector3 axePosition = Vector3.Lerp(axeManFrom, axeManTo, percentage);
        float axeAngle = Mathf.Lerp(axeManAngleFrom, axeManAngleTo, percentage);

        axeMan.transform.localPosition = axePosition;
        axeMan.transform.localEulerAngles = new Vector3(0f, 0f, axeAngle);
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
        //Tree.BodyParts.Axe.GetComponent<SpriteRenderer>().sortingOrder = i + 6;
        Tree.BodyParts.MinigameCircle.GetComponent<SpriteRenderer>().sortingOrder = i + 7;
        axeMan.GetComponent<SpriteRenderer>().sortingOrder = i + 3;
    }

    public override void Leave()
    {
        //Tree.BodyParts.RightLowerBackgroundArm.transform.rotation.lo
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
