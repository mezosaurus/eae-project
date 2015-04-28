using UnityEngine;

public class TreeStateInactive : TreeState
{
    private GameObject aButton;
    private float buttonScale, buttonScaleDirection;
    bool triggered;
    
    public override void Enter(object data)
    {
        Tree.Active = false;
        GlobalGameStateManager.PosessionState = PosessionState.EXORCISABLE;
        Tree.Eating = false;
        Tree.BodyParts.Face.GetComponent<SpriteRenderer>().sprite = Tree.Sprites.Face.None;
        aButton = (GameObject)GameObject.Instantiate(Tree.Prefabs.A, Tree.transform.position + new Vector3(0f, 1.8f), Quaternion.identity);
        aButton.transform.parent = Tree.transform;

        buttonScale = 1f;
        buttonScaleDirection = 1f;
        triggered = false;

        Tree.rigidbody2D.mass = 1000000f;
        Tree.rigidbody2D.drag = 1000000f;
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
            triggered = true;
        }
    }

    public override void OnTriggerExit(Collider2D collider)
    {
        if (collider.tag == "PossessorTrigger")
        {
            SetAlpha(Tree.transform, 1f);
            aButton.GetComponent<SpriteRenderer>().color = new Color(1f, 1f, 1f, 0.4901960784f);
            
            buttonScale = 1f;
            buttonScaleDirection = 1f;
            triggered = false;
            aButton.transform.localScale = new Vector3(1f, 1f, 1f);
        }
    }

    public override void Update()
    {
        Tree.rigidbody2D.velocity = Vector2.zero;
        
        if(Tree.Dead)
        {
            aButton.SetActive(false);

            return;
        }

        if(triggered)
        {
            buttonScale += (Time.deltaTime * buttonScaleDirection * 0.5f);

            if (buttonScale > 1.1f)
            {
                buttonScale = 1.1f;
                buttonScaleDirection = -1f;
            }

            if (buttonScale < 0.9f)
            {
                buttonScale = 0.9f;
                buttonScaleDirection = 1f;
            }

            aButton.transform.localScale = new Vector3(buttonScale, buttonScale, 1f);
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
            /*if(Tree.AxeMan != null)*/ Tree.AxeMan.GetComponent<SpriteRenderer>().sortingOrder = i + 10;
        }
    }

    public override void Leave()
    {
        SetAlpha(Tree.transform, 1f);
        GameObject.Destroy(aButton);

        Tree.rigidbody2D.mass = 1000f;
        Tree.rigidbody2D.drag = 0f;
    }
}
