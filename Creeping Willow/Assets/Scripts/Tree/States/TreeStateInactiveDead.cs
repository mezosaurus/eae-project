using UnityEngine;

public class TreeStateInactiveDead : TreeState
{
    /*private GameObject aButton;*/
    public GameObject AxeMan;
    
    public override void Enter(object data)
    {
        AxeMan = (GameObject)data;

        GlobalGameStateManager.PosessionState = PosessionState.NON_EXORCISABLE;

        Debug.Log("Entered");
    }

    public override void UpdateSorting()
    {
        Tree.BodyParts.Trunk.GetComponent<SpriteRenderer>().sortingOrder = 800;

        int i = Tree.BodyParts.Trunk.GetComponent<SpriteRenderer>().sortingOrder;

        Tree.BodyParts.Face.GetComponent<SpriteRenderer>().sortingOrder = i + 1;
        Tree.BodyParts.LeftUpperArm.GetComponent<SpriteRenderer>().sortingOrder = i + 1;
        Tree.BodyParts.RightUpperArm.GetComponent<SpriteRenderer>().sortingOrder = i + 1;
        Tree.BodyParts.RightLowerForegroundArm.GetComponent<SpriteRenderer>().sortingOrder = i + 4;
        Tree.BodyParts.RightLowerBackgroundArm.GetComponent<SpriteRenderer>().sortingOrder = i + 2;
        Tree.BodyParts.Legs.GetComponent<SpriteRenderer>().sortingOrder = i - 1;
        Tree.BodyParts.MinigameCircle.GetComponent<SpriteRenderer>().sortingOrder = i + 7;
        Tree.BodyParts.Axe.GetComponent<SpriteRenderer>().sortingOrder = i + 3;
        AxeMan.GetComponent<SpriteRenderer>().sortingOrder = i + 8;
    }
}
