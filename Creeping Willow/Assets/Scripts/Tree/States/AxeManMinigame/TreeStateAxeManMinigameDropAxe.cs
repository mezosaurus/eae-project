using UnityEngine;

public class TreeStateAxeManMinigameDropAxe : TreeState
{
    private const float MaxTime = 0.1f;

    private float timeElapsed, waitTimer;
    private float percentage;
    private GameObject axe;
    private Vector3 axeFrom, axeTo;
    private float axeAngleFrom, axeAngleTo;


    public override void Enter(object data)
    {
        //MessageCenter.Instance.Broadcast(new CameraZoomAndFocusMessage(Tree.BodyParts.Axe.transform.position, 0.25f, 1.8f, 20f));

        axe = (data as Data).Axe;

        axe.transform.parent = null;

        timeElapsed = 0f;
        waitTimer = 0f;
        percentage = 0f;

        // setup axe variables
        axeFrom = axe.transform.position;
        axeTo = axeFrom + new Vector3(0f, -0.9505684f);
        axeAngleFrom = axe.transform.eulerAngles.z;
        axeAngleTo = 358.131f;
    }

    public override void Update()
    {
        timeElapsed += Time.deltaTime;

        percentage = timeElapsed / MaxTime;

        if (percentage > 1f)
        {
            percentage = 1f;

            waitTimer += Time.deltaTime;

            if(waitTimer > 0.5f)
                Tree.ChangeState("AxeManMinigameLowerToAxeMan");
        }

        UpdateAxe(percentage);
    }

    protected void UpdateAxe(float percentage)
    {
        Vector3 axePosition = Vector3.Lerp(axeFrom, axeTo, percentage);
        float axeAngle = Mathf.Lerp(axeAngleFrom, axeAngleTo, percentage);

        axe.transform.position = axePosition;
        axe.transform.eulerAngles = new Vector3(0f, 0f, axeAngle);
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
        axe.GetComponent<SpriteRenderer>().sortingOrder = i + 3;*/

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
        Tree.AxeMan.GetComponent<SpriteRenderer>().sortingOrder = i + 0;
        axe.GetComponent<SpriteRenderer>().sortingOrder = i + 3;
    }

    public override void Leave()
    {
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
