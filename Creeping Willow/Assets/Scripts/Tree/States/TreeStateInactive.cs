using UnityEngine;

public class TreeStateInactive : TreeState
{
    private GameObject aButton;
    
    public override void Enter(object data)
    {
        Tree.Active = false;
        GlobalGameStateManager.PosessionState = PosessionState.EXORCISABLE;
        Tree.Eating = false;
        Tree.BodyParts.Face.GetComponent<SpriteRenderer>().sprite = Tree.Sprites.Face.None;
        aButton = (GameObject)GameObject.Instantiate(Tree.Prefabs.A, Tree.transform.position + new Vector3(0f, 1.8f), Quaternion.identity);
        aButton.transform.parent = Tree.transform;
    }

    private void SetAlpha(Transform root, float alpha)
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

    public override void UpdateSorting()
    {
        int i = Tree.BodyParts.Trunk.GetComponent<SpriteRenderer>().sortingOrder;

        //Tree.BodyParts.Face.GetComponent<SpriteRenderer>().sortingOrder = i + 1;
        Tree.BodyParts.LeftArm.GetComponent<SpriteRenderer>().sortingOrder = i + 1;
        Tree.BodyParts.RightArm.GetComponent<SpriteRenderer>().sortingOrder = i + 1;
        Tree.BodyParts.Legs.GetComponent<SpriteRenderer>().sortingOrder = i - 1;
        
        if(Tree.Dead)
        {
            Tree.AxeMan.GetComponent<SpriteRenderer>().sortingOrder = i + 10;
        }
    }

    public override void Leave()
    {
        SetAlpha(Tree.transform, 1f);
        GameObject.Destroy(aButton);
    }
}
