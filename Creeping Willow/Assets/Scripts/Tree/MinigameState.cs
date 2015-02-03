using UnityEngine;
using System.Collections;

public class MinigameState : MonoBehaviour
{
    public PossessableTree Tree;
    
    public float UpperRightArmStartAngle { get; protected set; }
    public float UpperRightArmMidpointAngle { get; protected set; }
    public float UpperRightArmEndAngle { get; protected set; }

    public float LowerRightArmStartAngle { get; protected set; }
    public float LowerRightArmMidpointAngle { get; protected set; }
    public float LowerRightArmEndAngle { get; protected set; }

    
    public MinigameState(PossessableTree tree)
    {
        Tree = tree;
    }
}
