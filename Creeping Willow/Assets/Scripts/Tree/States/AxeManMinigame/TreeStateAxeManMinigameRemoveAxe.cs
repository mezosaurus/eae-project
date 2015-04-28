using UnityEngine;

public class TreeStateAxeManMinigameRemoveAxe : TreeState
{
    // arm constants
    private const float UpperArmStartAngle = 378.95144f;
    private const float LowerArmStartAngle = 268.2893f;
    private const float ForearmStartAngle = 6.799903f;
    private const float UpperArmEndAngle = 364.831751f;
    private const float LowerArmEndAngle = 261.1888f;
    private const float ForearmEndAngle = 6.799903f;

    private const float MaxTime = 0.05f;

    private float timeElapsed, timeElapsed2;
    private float percentage;
    private GameObject axe;


    public override void Enter(object data)
    {
        //MessageCenter.Instance.Broadcast(new CameraZoomAndFocusMessage(Tree.BodyParts.Axe.transform.position, 0.25f, 1.8f, 20f));
        
        // Create a custom axe
        axe = (GameObject)GameObject.Instantiate(Tree.BodyParts.Axe);

        axe.transform.SetParent(Tree.BodyParts.RightLowerForegroundArm.transform);
        axe.transform.localPosition = new Vector3(0.610626f, 0.08036125f);

        Tree.BodyParts.Axe.SetActive(false);

        timeElapsed = 0f;
        timeElapsed2 = 0f;
        percentage = 0f;

        // play thump sound
        Tree.BodyParts.Trunk.audio.clip = Tree.Sounds.RemoveAxe;
        Tree.BodyParts.Trunk.audio.Play();

        UpdateArms(percentage);
    }

    public override void Update()
    {
        timeElapsed += Time.deltaTime;

        percentage = timeElapsed / MaxTime;

        if (percentage > 1f) percentage = 1f;

        if(!Tree.BodyParts.Trunk.audio.isPlaying)
        {
            timeElapsed2 += Time.deltaTime;

            if(timeElapsed2 > 0.64f)
            {
                Tree.ChangeState("AxeManMinigameRaiseAxe", new TreeStateAxeManMinigameRaiseAxe.Data(axe));
            }
        }

        UpdateArms(percentage);
    }

    protected void UpdateArms(float percentage)
    {
        float upperAngle = UpperArmStartAngle + ((UpperArmEndAngle - UpperArmStartAngle) * percentage);
        float lowerAngle = LowerArmStartAngle + ((LowerArmEndAngle - LowerArmStartAngle) * percentage);
        float foreAngle = ForearmStartAngle + ((ForearmEndAngle - ForearmStartAngle) * percentage);

        Tree.BodyParts.RightUpperArm.transform.localEulerAngles = new Vector3(0f, 0f, upperAngle);
        Tree.BodyParts.RightLowerForegroundArm.transform.localEulerAngles = new Vector3(0f, 0f, lowerAngle);
        Tree.BodyParts.RightLowerBackgroundArm.transform.localEulerAngles = new Vector3(0f, 0f, foreAngle);
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
}
