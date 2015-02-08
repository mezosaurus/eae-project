using UnityEngine;

public class NPCDataMowerMan : NPCData
{
    public NPCDataMowerMan()
    {
        LTOffset = new Vector3(0.138897f, -0.523063f);

        LeftUpperArmEndAngle = 0f;
        LeftLowerArmEndAngle = 0f;
        RightUpperArmEndAngle = 379.9801f;
        RightLowerArmEndAngle = 279.1071f;

        ArmEatTime = 0.9f;

        AnimationTrigger = "MowerMan";

        CalculateMidpointAngles();
    }
}
