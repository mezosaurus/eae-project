using UnityEngine;

public class TreeStateAxeManMinigameGroan : TreeState
{    
    public override void Enter(object data)
    {
        Tree.audio.clip = Tree.Sounds.Groan;
        Tree.audio.Play();
    }

    public override void Update()
    {
        if (!Tree.audio.isPlaying)
            Tree.ChangeState("AxeManMinigamePanToAxe");
    }

    public override void UpdateSorting()
    {
        //Tree.BodyParts.Trunk.GetComponent<SpriteRenderer>().sortingOrder = 800;

        int i = Tree.BodyParts.Trunk.GetComponent<SpriteRenderer>().sortingOrder;

        Tree.BodyParts.Face.GetComponent<SpriteRenderer>().sortingOrder = i + 1;
        //Tree.BodyParts.LeftArm.GetComponent<SpriteRenderer>().sortingOrder = i + 1;
        /*Tree.BodyParts.RightUpperArm.GetComponent<SpriteRenderer>().sortingOrder = i + 1;
        Tree.BodyParts.RightLowerForegroundArm.GetComponent<SpriteRenderer>().sortingOrder = i + 4;
        Tree.BodyParts.RightLowerBackgroundArm.GetComponent<SpriteRenderer>().sortingOrder = i + 2;*/
        Tree.BodyParts.Legs.GetComponent<SpriteRenderer>().sortingOrder = i - 1;
        /*//Tree.BodyParts.Axe.GetComponent<SpriteRenderer>().sortingOrder = i + 6;
        Tree.BodyParts.MinigameCircle.GetComponent<SpriteRenderer>().sortingOrder = i + 7;*/
        Tree.BodyParts.Axe.GetComponent<SpriteRenderer>().sortingOrder = i + 3;
    }
}
