using UnityEngine;

public class NPCDataOldMan : NPCData
{
    public NPCDataOldMan()
    {
        LTOffset = new Vector3(-0.069304f, -0.580232f);

        LeftUpperArmEndAngle = 0f;
        LeftLowerArmEndAngle = 0f;
        RightUpperArmEndAngle = 369.9384f;
        RightLowerArmEndAngle = 290.8225f;

        ArmEatTime = 0.75f;

        AnimationTrigger = "OldMan";

        CalculateMidpointAngles();
    }
}
