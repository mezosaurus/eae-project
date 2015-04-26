﻿using UnityEngine;

public class NPCDataBoppina : NPCData
{
    public NPCDataBoppina()
    {
        LTOffset = new Vector3(-0.003852f, -0.4921265f);

        LeftUpperArmEndAngle = 0f;
        LeftLowerArmEndAngle = 0f;
        RightUpperArmEndAngle = 374.70489f;
        RightLowerArmEndAngle = 308.3104f;

        ArmEatTime = 0.35f;

        AnimationTrigger = "Boppina";

        CalculateMidpointAngles();
    }
}
