using System.Collections.Generic;
using UnityEngine;

public class TreeStateActive : TreeState
{
    private List<GameObject> npcsInRange;

    
    public override void Enter()
    {
        Tree.Active = true;
        npcsInRange = new List<GameObject>();

        Tree.BodyParts.RightUpperArm.SetActive(false);
		if (Tree.BodyParts.GrabbedNPC != null)
	        Tree.BodyParts.GrabbedNPC.SetActive(false);
        Tree.BodyParts.MinigameCircle.SetActive(false);
        Tree.BodyParts.LeftArm.SetActive(true);
        Tree.BodyParts.RightArm.SetActive(true);
		if (Tree.BodyParts.EatenNPC != null)
	        Tree.BodyParts.EatenNPC.SetActive(false);
        
        ChangeSpritesToDirection(Vector2.zero);
    }

    public override void Update()
    {        
        // Update velocity
        Vector2 velocity = new Vector2(Input.GetAxis("LSX"), Input.GetAxis("LSY")) * Tree.Speed * Time.deltaTime;
        Tree.rigidbody2D.velocity = velocity;

        Vector3 grabOffset = Tree.transform.position + new Vector3(0f, 0.25f);

        // Update all NPC's that are within grabbing range
        foreach(GameObject npc in GameObject.FindGameObjectsWithTag("NPC"))
        {
            int listIndex = npcsInRange.IndexOf(npc);
            bool inRange = Vector3.Distance(Tree.transform.position, npc.transform.position) <= 1.2f;

            if(inRange && (listIndex == -1))
            {
                npcsInRange.Add(npc);

                npc.GetComponent<SpriteRenderer>().color = Color.red;
            }
            else if(!inRange && (listIndex != -1))
            {
                npcsInRange.RemoveAt(listIndex);

                npc.GetComponent<SpriteRenderer>().color = Color.white;
            }
        }

        // See if we are going to grab
        if(Input.GetAxis("LT") > 0.5f /*&& npcsInRange.Count > 0*/)
        {
            Tree.ChangeState("EatingMinigameWrangle");

            return;
        }

        ChangeSpritesToDirection(velocity);
    }

    public override void UpdateSorting()
    {
        int i = Tree.BodyParts.Trunk.GetComponent<SpriteRenderer>().sortingOrder;

        Tree.BodyParts.Face.GetComponent<SpriteRenderer>().sortingOrder = i + 1;
        Tree.BodyParts.LeftArm.GetComponent<SpriteRenderer>().sortingOrder = i + 1;
        Tree.BodyParts.RightArm.GetComponent<SpriteRenderer>().sortingOrder = i + 1;
        Tree.BodyParts.Legs.GetComponent<SpriteRenderer>().sortingOrder = i - 1;
    }

    private void ChangeSpritesToDirection(Vector2 velocity)
    {
        SpriteRenderer trunkSpriteRenderer = Tree.BodyParts.Trunk.GetComponent<SpriteRenderer>();
        SpriteRenderer faceSpriteRenderer = Tree.BodyParts.Face.GetComponent<SpriteRenderer>();
        Animator legsAnimator = Tree.BodyParts.Legs.GetComponent<Animator>();
        float x = velocity.x;
        float y = velocity.y;

        if(velocity == Vector2.zero)
        {
            trunkSpriteRenderer.sprite = Tree.Sprites.Trunk.Front;
            Tree.BodyParts.Trunk.transform.localScale = new Vector3(1f, 1f, 1f);
            faceSpriteRenderer.sprite = Tree.Sprites.Face.EyesClosed;
            legsAnimator.enabled = false;
            legsAnimator.GetComponent<SpriteRenderer>().sprite = Tree.Sprites.LegsStill;

            return;
        }

        float maxVelocity = Tree.Speed * Time.deltaTime;

        legsAnimator.enabled = true;
        legsAnimator.speed = velocity.magnitude / maxVelocity;

        legsAnimator.speed = 1f;
        
        if(x == 0f && y < 0f) // Front
        {
            trunkSpriteRenderer.sprite = Tree.Sprites.Trunk.Front;
            Tree.BodyParts.Trunk.transform.localScale = new Vector3(1f, 1f, 1f);
            faceSpriteRenderer.sprite = Tree.Sprites.Face.MoveFront;
        }
        else if(x > 0f && y < 0f) // Front right
        {
            trunkSpriteRenderer.sprite = Tree.Sprites.Trunk.FrontRight;
            Tree.BodyParts.Trunk.transform.localScale = new Vector3(1f, 1f, 1f);
            faceSpriteRenderer.sprite = Tree.Sprites.Face.MoveFrontRight;
        }
        else if (x > 0f && y == 0f) // Right
        {
            trunkSpriteRenderer.sprite = Tree.Sprites.Trunk.Right;
            Tree.BodyParts.Trunk.transform.localScale = new Vector3(1f, 1f, 1f);
            faceSpriteRenderer.sprite = Tree.Sprites.Face.MoveRight;
        }
        else if (x > 0f && y > 0f) // Back right
        {
            trunkSpriteRenderer.sprite = Tree.Sprites.Trunk.BackRight;
            Tree.BodyParts.Trunk.transform.localScale = new Vector3(1f, 1f, 1f);
            faceSpriteRenderer.sprite = Tree.Sprites.Face.None;
        }
        else if (x == 0f && y > 0f) // Back
        {
            trunkSpriteRenderer.sprite = Tree.Sprites.Trunk.Back;
            Tree.BodyParts.Trunk.transform.localScale = new Vector3(1f, 1f, 1f);
            faceSpriteRenderer.sprite = Tree.Sprites.Face.None;
        }
        else if (x < 0f && y > 0f) // Back left
        {
            trunkSpriteRenderer.sprite = Tree.Sprites.Trunk.BackRight;
            Tree.BodyParts.Trunk.transform.localScale = new Vector3(-1f, 1f, 1f);
            faceSpriteRenderer.sprite = Tree.Sprites.Face.None;
        }
        else if (x < 0f && y == 0f) // Left
        {
            trunkSpriteRenderer.sprite = Tree.Sprites.Trunk.Right;
            Tree.BodyParts.Trunk.transform.localScale = new Vector3(-1f, 1f, 1f);
            faceSpriteRenderer.sprite = Tree.Sprites.Face.MoveRight;
        }
        else if (x < 0f && y < 0f) // FrontLeft
        {
            trunkSpriteRenderer.sprite = Tree.Sprites.Trunk.FrontRight;
            Tree.BodyParts.Trunk.transform.localScale = new Vector3(-1f, 1f, 1f);
            faceSpriteRenderer.sprite = Tree.Sprites.Face.MoveFrontRight;
        }
    }

    public override void Leave()
    {
        Tree.rigidbody2D.velocity = Vector2.zero;

        //ChangeSpritesToDirection(Vector2.zero);
    }
}