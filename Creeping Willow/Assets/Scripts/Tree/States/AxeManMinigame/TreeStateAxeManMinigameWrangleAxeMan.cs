﻿using UnityEngine;

public class TreeStateAxeManMinigameWrangleAxeMan : TreeState
{
    private const float UpperArmStartAngle = 337.5777f;
    private const float LowerArmStartAngle = 245.239f;
    private const float UpperArmEndAngle = 357.470825f;
    private const float LowerArmEndAngle = 273.74145f;


    private const float InRadius = 0.4f;
    private const float MaxThumbStickRadius = 1.5f;
    private const float ForwardForceValue = 0.65f;
    private const float OpposingForceValue = 0.46f;
    private const float MaxTime = 2f * (MaxThumbStickRadius / (ForwardForceValue - 0.25f));


    private bool initialized;
    private GameObject LS, RS, LSArrow, RSArrow;
    private GameObject axeMan;
    private float timeElapsed;
    private int percentage;
    private float maxTime;


    public override void Enter(object data)
    {
        MessageCenter.Instance.Broadcast(new CameraZoomAndFocusMessage2(Tree.BodyParts.MinigameCircle.transform.position, 1.7f, 0.25f));
        MessageCenter.Instance.Broadcast(new AxeManMinigameAxeManChangePhaseMessage(1001));

        axeMan = (data as Data).AxeMan;

        initialized = false;
        maxTime = MaxTime * GlobalGameStateManager.AxeManMinigameModifier2;

        Initialize();
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
        //if (!initialized) Initialize();

        Transform circle = Tree.BodyParts.MinigameCircle.transform;

        // Determine whether thumbsticks are in the circle
        float lsDistance = Vector3.Distance(circle.position, LS.transform.position);
        float rsDistance = Vector3.Distance(circle.position, RS.transform.position);

        bool lsIn = (lsDistance <= InRadius);
        bool rsIn = (rsDistance <= InRadius);

        // If both sticks are in, change to next phase of minigame
        if (lsIn && rsIn)
        {
            TreeStateAxeManMinigameMash.Data data = new TreeStateAxeManMinigameMash.Data(axeMan, percentage);
            
            Tree.ChangeState("AxeManMinigameMash", data);

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

        float cameraSize = Mathf.Lerp(1.7f, 1.2f, progress);   
        Camera.main.orthographicSize = cameraSize;
        Camera.main.GetComponent<CameraScript>().TargetSize = cameraSize;

        // Update timer
        timeElapsed += Time.deltaTime;

        float timePercentage = Mathf.Clamp(timeElapsed / maxTime, 0f, 1f);
        percentage = Mathf.RoundToInt(timePercentage * 100f);

        Tree.BodyParts.MinigameCircle.GetComponent<SpriteRenderer>().sprite = Tree.Sprites.EatingMinigame.Circle[percentage];

        // To-do
        if (timePercentage >= 1f)
        {
            Lose();

            return;
        }
    }

    protected void UpdateArms(float percentage)
    {
        float upperAngle = UpperArmStartAngle + ((UpperArmEndAngle - UpperArmStartAngle) * percentage);
        float lowerAngle = LowerArmStartAngle + ((LowerArmEndAngle - LowerArmStartAngle) * percentage);

        Tree.BodyParts.RightUpperArm.transform.localEulerAngles = new Vector3(0f, 0f, upperAngle);
        Tree.BodyParts.RightLowerForegroundArm.transform.localEulerAngles = new Vector3(0f, 0f, lowerAngle);
    }

    protected void Lose()
    {
        Tree.BodyParts.RightLowerBackgroundArm.transform.localEulerAngles = new Vector3(0f, 0f, 0f);
        Tree.BodyParts.Axe.SetActive(false);

        Tree.audio.Stop();

        MessageCenter.Instance.Broadcast(new AxeManMinigameAxeManChangePhaseMessage(8001));

        Tree.ChangeState("AxeManMinigameDead");
    }

    public override void UpdateSorting()
    {
        /*Tree.BodyParts.Trunk.GetComponent<SpriteRenderer>().sortingOrder = 800;
        
        int i = Tree.BodyParts.Trunk.GetComponent<SpriteRenderer>().sortingOrder;

        Tree.BodyParts.Face.GetComponent<SpriteRenderer>().sortingOrder = i + 1;
        //Tree.BodyParts.LeftArm.GetComponent<SpriteRenderer>().sortingOrder = i + 1;
        Tree.BodyParts.RightUpperArm.GetComponent<SpriteRenderer>().sortingOrder = i + 1;
        Tree.BodyParts.RightLowerForegroundArm.GetComponent<SpriteRenderer>().sortingOrder = i + 4;
        Tree.BodyParts.RightLowerBackgroundArm.GetComponent<SpriteRenderer>().sortingOrder = i + 2;
        Tree.BodyParts.Legs.GetComponent<SpriteRenderer>().sortingOrder = i - 1;
        Tree.BodyParts.MinigameCircle.GetComponent<SpriteRenderer>().sortingOrder = i + 7;
        axeMan.GetComponent<SpriteRenderer>().sortingOrder = i + 3;*/

        Tree.BodyParts.Trunk.GetComponent<SpriteRenderer>().sortingOrder = 800;

        int i = Tree.BodyParts.Trunk.GetComponent<SpriteRenderer>().sortingOrder;

        Tree.BodyParts.Face.GetComponent<SpriteRenderer>().sortingOrder = i + 1;
        Tree.BodyParts.LeftUpperArm.GetComponent<SpriteRenderer>().sortingOrder = i + 1;
        Tree.BodyParts.LeftLowerForegroundArm.GetComponent<SpriteRenderer>().sortingOrder = i + 2;
        Tree.BodyParts.LeftLowerBackgroundArm.GetComponent<SpriteRenderer>().sortingOrder = i + 1;
        Tree.BodyParts.RightUpperArm.GetComponent<SpriteRenderer>().sortingOrder = i + 1;
        Tree.BodyParts.RightLowerForegroundArm.GetComponent<SpriteRenderer>().sortingOrder = i + 4;
        Tree.BodyParts.RightLowerBackgroundArm.GetComponent<SpriteRenderer>().sortingOrder = i + 2;
        Tree.BodyParts.Legs.GetComponent<SpriteRenderer>().sortingOrder = i - 1;
        Tree.BodyParts.MinigameCircle.GetComponent<SpriteRenderer>().sortingOrder = i + 7;
        Tree.BodyParts.Axe.GetComponent<SpriteRenderer>().sortingOrder = i + 3;
        Tree.AxeMan.GetComponent<SpriteRenderer>().sortingOrder = i + 3;
    }

    public override void Leave()
    {
        GameObject.Destroy(LS);
        GameObject.Destroy(RS);

        Tree.BodyParts.MinigameCircle.GetComponent<SpriteRenderer>().sprite = Tree.Sprites.EatingMinigame.Circle[100];
    }

    
    public class Data
    {
        public GameObject AxeMan;

        public Data(GameObject axeMan)
        {
            AxeMan = axeMan;
        }
    }
}
