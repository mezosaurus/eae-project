using UnityEngine;

public class NPCData
{
    public readonly float LeftUpperArmStartAngle = 0f;
    public readonly float LeftUpperArmFinalAngle = 0f;
    public readonly float LeftLowerArmStartAngle = 0f;
    public readonly float LeftLowerArmFinalAngle = 0f;
    public readonly float RightUpperArmStartAngle = 340.2503f;
    public readonly float RightUpperArmFinalAngle = 374.1989f;
    public readonly float RightLowerArmStartAngle = 256.9366f;
    public readonly float RightLowerArmFinalAngle = 302.5608f;


    public NPCSkinType SkinType;

    
    public Vector3 LTOffset { get; protected set; }
    public float LeftUpperArmMidpointAngle { get; private set; }
    public float LeftUpperArmEndAngle { get; protected set; }
    public float LeftLowerArmMidpointAngle { get; private set; }
    public float LeftLowerArmEndAngle { get; protected set; }
    public float RightUpperArmMidpointAngle { get; private set; }
    public float RightUpperArmEndAngle { get; protected set; }
    public float RightLowerArmMidpointAngle { get; private set; }
    public float RightLowerArmEndAngle { get; protected set; }
    public float ArmEatTime { get; protected set; }
    public string AnimationTrigger { get; protected set; }


    protected void CalculateMidpointAngles()
    {
        LeftUpperArmMidpointAngle = LeftUpperArmStartAngle + ((LeftUpperArmEndAngle - LeftUpperArmStartAngle) / 2f);
        LeftLowerArmMidpointAngle = LeftLowerArmStartAngle + ((LeftLowerArmEndAngle - LeftLowerArmStartAngle) / 2f);
        RightUpperArmMidpointAngle = RightUpperArmStartAngle + ((RightUpperArmEndAngle - RightUpperArmStartAngle) / 2f);
        RightLowerArmMidpointAngle = RightLowerArmStartAngle + ((RightLowerArmEndAngle - RightLowerArmStartAngle) / 2f);
    }
}
