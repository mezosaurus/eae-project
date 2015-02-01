﻿using UnityEngine;

public class TreeStateEatingMinigameWrangle : TreeStateEatingMinigame
{
    private const float InRadius = 0.4f;
    private const float MaxThumbStickRadius = 1.5f;
    private const float ForwardForceValue = 0.65f;
    private const float OpposingForceValue = 0.25f;
    private const float MaxTime = 2f * (MaxThumbStickRadius / (ForwardForceValue - OpposingForceValue));


    private bool initialized;
    private GameObject LS, RS, LSArrow, RSArrow;
    private float timeElapsed;


    public override void Enter()
    {
        base.Enter();

        Tree.BodyParts.LeftArm.SetActive(false);
        Tree.BodyParts.RightArm.SetActive(false);
        Tree.BodyParts.RightUpperArm.SetActive(true);
        Tree.BodyParts.MinigameCircle.SetActive(true);

        Tree.BodyParts.Face.GetComponent<SpriteRenderer>().sprite = Tree.Sprites.Face.Crazy;
        Tree.BodyParts.MinigameCircle.GetComponent<SpriteRenderer>().color = Color.white;

        MessageCenter.Instance.Broadcast(new CameraZoomMessage(1.8f, 20f));

        initialized = false;
    }

    private void Initialize()
    {
        // Create thumbsticks
        Vector3 lsOffset = Quaternion.Euler(0f, 0f, Random.Range(45f, 135f)) * new Vector3(0f, MaxThumbStickRadius);
        Vector3 rsOffset = Quaternion.Euler(0f, 0f, Random.Range(45f, 135f)) * new Vector3(0f, -MaxThumbStickRadius);

        LS = (GameObject)GameObject.Instantiate(Tree.Prefabs.ThumbStick, Tree.BodyParts.MinigameCircle.transform.position + lsOffset, Quaternion.identity);
        RS = (GameObject)GameObject.Instantiate(Tree.Prefabs.ThumbStick, Tree.BodyParts.MinigameCircle.transform.position + rsOffset, Quaternion.identity);

        LS.GetComponent<SpriteRenderer>().sprite = Tree.Sprites.EatingMinigame.LS;
        RS.GetComponent<SpriteRenderer>().sprite = Tree.Sprites.EatingMinigame.RS;

        LSArrow = LS.transform.GetChild(0).gameObject;
        RSArrow = RS.transform.GetChild(0).gameObject;

        Tree.BodyParts.MinigameCircle.GetComponent<SpriteRenderer>().sprite = Tree.Sprites.EatingMinigame.Circle[0];

        timeElapsed = 0f;

        initialized = true;
    }

    public override void Update()
    {
        if (Camera.main.orthographicSize != Camera.main.GetComponent<CameraScript>().TargetSize) return;
        if (!initialized) Initialize();

        Transform circle = Tree.BodyParts.MinigameCircle.transform;

        // Determine whether thumbsticks are in the circle
        float lsDistance = Vector3.Distance(circle.position, LS.transform.position);
        float rsDistance = Vector3.Distance(circle.position, RS.transform.position);

        bool lsIn = (lsDistance <= InRadius);
        bool rsIn = (rsDistance <= InRadius);

        // If both sticks are in, change to next phase of minigame
        if (lsIn && rsIn)
        {
            Tree.ChangeState("EatingMinigameMash");

            return;
        }

        Vector3 lsDifference = circle.position - LS.transform.position;
        Vector3 rsDifference = circle.position - RS.transform.position;

        // Apply forces if necessary
        if(!lsIn || !rsIn)
        {
            if (lsDistance < MaxThumbStickRadius)
            {
                LS.transform.position -= (lsDifference.normalized * OpposingForceValue * Time.deltaTime);
            }

            if (rsDistance < MaxThumbStickRadius)
            {
                RS.transform.position -= (rsDifference.normalized * OpposingForceValue * Time.deltaTime);
            }
        }

        // Move the user toward their goal
        LS.transform.position += (new Vector3(Input.GetAxis("LSX"), Input.GetAxis("LSY")).normalized * ForwardForceValue * Time.deltaTime);
        RS.transform.position += (new Vector3(Input.GetAxis("RSX"), Input.GetAxis("RSY")).normalized * ForwardForceValue * Time.deltaTime);

        // Make sure the user can't go too far and make things even more difficult
        if (Vector3.Distance(circle.position, LS.transform.position) > MaxThumbStickRadius)
        {
            LS.transform.position = circle.position - ((circle.position - LS.transform.position).normalized * MaxThumbStickRadius);
        }

        if (Vector3.Distance(circle.position, RS.transform.position) > MaxThumbStickRadius)
        {
            RS.transform.position = circle.position - ((circle.position - RS.transform.position).normalized * MaxThumbStickRadius);
        }

        // Update arrows
        LSArrow.GetComponent<SpriteRenderer>().enabled = (lsIn) ? false : true;
        RSArrow.GetComponent<SpriteRenderer>().enabled = (rsIn) ? false : true;

        float lsAngle = Mathf.Atan2(lsDifference.y, lsDifference.x) * Mathf.Rad2Deg;
        float rsAngle = Mathf.Atan2(rsDifference.y, rsDifference.x) * Mathf.Rad2Deg;

        LSArrow.transform.eulerAngles = new Vector3(0f, 0f, lsAngle);
        RSArrow.transform.eulerAngles = new Vector3(0f, 0f, rsAngle);

        // Update arms based on user progress
        float lsProgress = Mathf.Clamp(1f - ((Vector3.Distance(circle.position, LS.transform.position) - InRadius) / (MaxThumbStickRadius - InRadius)), 0f, 1f);
        float rsProgress = Mathf.Clamp(1f - ((Vector3.Distance(circle.position, RS.transform.position) - InRadius) / (MaxThumbStickRadius - InRadius)), 0f, 1f);
        float progress = (lsProgress + rsProgress) / 2f;

        UpdateArms(progress);

        // Update timer
        timeElapsed += Time.deltaTime;

        float timePercentage = Mathf.Clamp(timeElapsed / MaxTime, 0f, 1f);
        Percentage = Mathf.RoundToInt(timePercentage * 100f);

        Tree.BodyParts.MinigameCircle.GetComponent<SpriteRenderer>().sprite = Tree.Sprites.EatingMinigame.Circle[Percentage];

        // To-do
        if (timePercentage >= 1f)
        {
            Lose();

            return;
        }
    }

    public override void UpdateSorting()
    {
        int i = Tree.BodyParts.Trunk.GetComponent<SpriteRenderer>().sortingOrder;

        Tree.BodyParts.Face.GetComponent<SpriteRenderer>().sortingOrder = i + 1;
        //Tree.BodyParts.LeftArm.GetComponent<SpriteRenderer>().sortingOrder = i + 1;
        Tree.BodyParts.RightUpperArm.GetComponent<SpriteRenderer>().sortingOrder = i + 1;
        Tree.BodyParts.RightLowerForegroundArm.GetComponent<SpriteRenderer>().sortingOrder = i + 2;
        Tree.BodyParts.RightLowerBackgroundArm.GetComponent<SpriteRenderer>().sortingOrder = i + 4;
        Tree.BodyParts.Legs.GetComponent<SpriteRenderer>().sortingOrder = i - 1;
        Tree.BodyParts.MinigameCircle.GetComponent<SpriteRenderer>().sortingOrder = i + 7;
        Tree.BodyParts.GrabbedNPC.GetComponent<SpriteRenderer>().sortingOrder = i + 3;
    }

    public override void Leave()
    {
        GameObject.Destroy(LS);
        GameObject.Destroy(RS);

        Tree.BodyParts.MinigameCircle.GetComponent<SpriteRenderer>().sprite = Tree.Sprites.EatingMinigame.Circle[100];
    }
}
