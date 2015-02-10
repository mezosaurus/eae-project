using UnityEngine;

public class NPCDataSquirrel : NPCData
{
    public NPCDataSquirrel()
    {
        LTOffset = new Vector3(0f, -0.4f);

        LeftUpperArmEndAngle = 0f;
        LeftLowerArmEndAngle = 0f;
        RightUpperArmEndAngle = 365.933216f;
        RightLowerArmEndAngle = 315.3768f;

        ArmEatTime = 0.4f;

        AnimationTrigger = "Squirrel";

        CalculateMidpointAngles();
    }
}

