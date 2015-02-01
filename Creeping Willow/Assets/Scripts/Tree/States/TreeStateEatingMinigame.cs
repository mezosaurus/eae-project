using UnityEngine;

public class TreeStateEatingMinigame : TreeState
{
    protected const float UpperArmStartAngle = -12.80748f;
    protected const float UpperArmEndAngle = 20.5433f;
    protected const float LowerArmStartAngle = -87.42857f;
    protected const float LowerArmEndAngle = -78.57207f;


    protected static int Percentage;

    
    public override void Enter()
    {
        // Set initial rotation on arms
        Tree.BodyParts.RightUpperArm.transform.eulerAngles = new Vector3(0f, 0f, UpperArmStartAngle);
        Tree.BodyParts.RightLowerForegroundArm.transform.eulerAngles = new Vector3(0f, 0f, LowerArmStartAngle);
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
        MessageCenter.Instance.Broadcast(new CameraZoomMessage(4f, 20f));

        Tree.ChangeState("Active");
    }
}