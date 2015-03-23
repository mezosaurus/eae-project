using UnityEngine;

public class TreeStateAxeManMinigameWaitForChop : TreeState
{
    /*private GameObject aButton;*/
    private GameObject axeMan;
    
    public override void Enter(object data)
    {
        Tree.Active = false;
        GlobalGameStateManager.PosessionState = PosessionState.NON_EXORCISABLE;
        Tree.Eating = true;

        Tree.BodyParts.LeftArm.SetActive(true);
        Tree.BodyParts.RightArm.SetActive(true);
        Tree.BodyParts.RightUpperArm.SetActive(false);
        Tree.BodyParts.LeftUpperArm.SetActive(false);
        Tree.BodyParts.MinigameCircle.SetActive(false);
        Tree.BodyParts.RightGrabbedNPC.SetActive(false);

        Tree.BodyParts.Face.GetComponent<SpriteRenderer>().sprite = Tree.Sprites.Face.Crazy;

        axeMan = GameObject.FindGameObjectWithTag("AxeManKillActiveTree");
    }

    /*private void SetAlpha(Transform root, float alpha)
    {
        SpriteRenderer s = root.GetComponent<SpriteRenderer>();

        if (s != null) s.color = new Color(1f, 1f, 1f, alpha);

        foreach (Transform child in root)
        {
            SetAlpha(child, alpha);
        }
    }

    public override void OnTriggerEnter(Collider2D collider)
    {
        if (collider.tag == "PossessorTrigger")
        {
            SetAlpha(Tree.transform, 0.75f);
            aButton.GetComponent<SpriteRenderer>().color = new Color(1f, 1f, 1f, 1f);
        }
    }

    public override void OnTriggerExit(Collider2D collider)
    {
        if (collider.tag == "PossessorTrigger")
        {
            SetAlpha(Tree.transform, 1f);
            aButton.GetComponent<SpriteRenderer>().color = new Color(1f, 1f, 1f, 0.4901960784f);
        }
    }

    public override void Update()
    {
        if(Tree.Dead)
        {
            aButton.SetActive(false);

            return;
        }

        if (GameObject.FindGameObjectWithTag("Possessor") != null) aButton.SetActive(true);
        else aButton.SetActive(false);
    }

    public override void Leave()
    {
        SetAlpha(Tree.transform, 1f);
        GameObject.Destroy(aButton);
    }*/

    public override void UpdateSorting()
    {
        //Tree.BodyParts.Trunk.GetComponent<SpriteRenderer>().sortingOrder = 800;

        int i = Tree.BodyParts.Trunk.GetComponent<SpriteRenderer>().sortingOrder;

        Tree.BodyParts.Face.GetComponent<SpriteRenderer>().sortingOrder = i + 1;
        //Tree.BodyParts.LeftArm.GetComponent<SpriteRenderer>().sortingOrder = i + 1;
        Tree.BodyParts.RightUpperArm.GetComponent<SpriteRenderer>().sortingOrder = i + 1;
        Tree.BodyParts.RightLowerForegroundArm.GetComponent<SpriteRenderer>().sortingOrder = i + 4;
        Tree.BodyParts.RightLowerBackgroundArm.GetComponent<SpriteRenderer>().sortingOrder = i + 2;
        Tree.BodyParts.Legs.GetComponent<SpriteRenderer>().sortingOrder = i - 1;
        /*//Tree.BodyParts.Axe.GetComponent<SpriteRenderer>().sortingOrder = i + 6;
        Tree.BodyParts.MinigameCircle.GetComponent<SpriteRenderer>().sortingOrder = i + 7;
        Tree.BodyParts.Axe.GetComponent<SpriteRenderer>().sortingOrder = i + 3;*/
        axeMan.GetComponent<SpriteRenderer>().sortingOrder = i + 2;
    }
}
