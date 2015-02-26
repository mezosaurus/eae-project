using UnityEngine;

public class NPCDataHottie : NPCData
{
    public NPCDataHottie()
    {
        LTOffset = new Vector3(0.03f, -0.7f);

        LeftUpperArmEndAngle = 0f;
        LeftLowerArmEndAngle = 0f;
        RightUpperArmEndAngle = 367.01577f;
        RightLowerArmEndAngle = 293.9036f;

        ArmEatTime = 0.75f;

        AnimationTrigger = "Hottie";

        CalculateMidpointAngles();
    }
}
