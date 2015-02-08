using UnityEngine;

public class NPCDataHippie : NPCData
{
    public NPCDataHippie()
    {
        LTOffset = new Vector3(-0.031583f, -0.80241f);

        LeftUpperArmEndAngle = 0f;
        LeftLowerArmEndAngle = 0f;
        RightUpperArmEndAngle = 376.08445f;
        RightLowerArmEndAngle = 292.0871f;

        ArmEatTime = 0.8f;

        AnimationTrigger = "Hippie";

        CalculateMidpointAngles();
    }
}
