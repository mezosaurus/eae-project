using UnityEngine;

public class TreeStateInactive : TreeState
{
    public override void Enter(object data)
    {
        Tree.Active = false;
        GlobalGameStateManager.PosessionState = PosessionState.EXORCISABLE;
        Tree.Eating = false;
        Tree.BodyParts.Face.GetComponent<SpriteRenderer>().sprite = Tree.Sprites.Face.None;
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
        if (collider.tag == "Possessor") SetAlpha(Tree.transform, 0.75f);
    }

    public override void OnTriggerExit(Collider2D collider)
    {
        if (collider.tag == "Possessor") SetAlpha(Tree.transform, 1f);
    }

    public override void Leave()
    {
        SetAlpha(Tree.transform, 1f);
    }
}
