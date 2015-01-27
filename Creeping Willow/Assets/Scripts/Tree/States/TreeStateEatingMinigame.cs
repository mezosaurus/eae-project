using UnityEngine;

public class TreeStateEatingMinigame : TreeState
{
    private const float UpperArmStartAngle = -12.80748f;
    private const float UpperArmEndAngle = 20.5433f;
    private const float LowerArmStartAngle = -87.42857f;
    private const float LowerArmEndAngle = -78.57207f;

    
    public override void Enter()
    {
        // Set initial rotation on arms
        Tree.BodyParts.RightUpperArm.transform.eulerAngles = new Vector3(0f, 0f, UpperArmStartAngle);
        Tree.BodyParts.RightLowerForegroundArm.transform.eulerAngles = new Vector3(0f, 0f, LowerArmStartAngle);
    }
}