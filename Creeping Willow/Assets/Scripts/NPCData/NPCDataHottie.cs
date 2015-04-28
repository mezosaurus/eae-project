using UnityEngine;

public class NPCDataHottie : NPCData
{
    public NPCDataHottie()
    {
        LTOffset = new Vector3(0.03f, -0.7f);

        LeftUpperArmEndAngle = 0f;
        LeftLowerArmEndAngle = 0f;
        RightUpperArmEndAngle = 378.9408f;
        RightLowerArmEndAngle = 286.4655f;

        ArmEatTime = 0.75f;

        AnimationTrigger = "Hottie";

        CalculateMidpointAngles();
    }
}
