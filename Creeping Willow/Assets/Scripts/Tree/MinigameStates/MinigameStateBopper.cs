using UnityEngine;

public class MinigameStateBopper : MinigameState
{
    public MinigameStateBopper(PossessableTree tree) : base(tree)
    {
        UpperRightArmStartAngle = 340.2503f;
        UpperRightArmMidpointAngle = 357.477595f;
        UpperRightArmEndAngle = 374.70489f;

        LowerRightArmStartAngle = 256.9366f;
        LowerRightArmMidpointAngle = 282.6235f;
        LowerRightArmEndAngle = 308.3104f;
    }
}
