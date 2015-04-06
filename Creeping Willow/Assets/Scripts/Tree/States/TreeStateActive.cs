using System.Collections.Generic;
using UnityEngine;

public class TreeStateActive : TreeState
{
    private List<GameObject> npcsInRange;
    private GameObject lt1, lt2;
    private uint walkAudio;
    private int lastLegFrame;

    
    public override void Enter(object data)
    {
        // Check to see if we're dead first of all
        if(Tree.Dead)
        {
            Tree.StartActiveAxeManMinigame();

            return;
        }

        Tree.Active = true;
        GlobalGameStateManager.PosessionState = PosessionState.EXORCISABLE;
        Tree.Eating = false;
        npcsInRange = new List<GameObject>();

        Tree.BodyParts.RightUpperArm.SetActive(false);
        Tree.BodyParts.LeftUpperArm.SetActive(false);
		/*if (Tree.BodyParts.GrabbedNPC != null)
	        Tree.BodyParts.GrabbedNPC.SetActive(false);*/
        Tree.BodyParts.MinigameCircle.SetActive(false);
        Tree.BodyParts.LeftArm.SetActive(true);
        Tree.BodyParts.RightArm.SetActive(true);
		if (Tree.BodyParts.EatenNPC != null)
	        Tree.BodyParts.EatenNPC.SetActive(false);

        // Create LT icons
        lt1 = (GameObject)GameObject.Instantiate(Tree.Prefabs.LT);
        lt2 = (GameObject)GameObject.Instantiate(Tree.Prefabs.LT);
        
        ChangeSpritesToDirection(Vector2.zero);

        /*Tree.audio.clip = Tree.Sounds.Walk;
        Tree.audio.volume = 0.25f;
        Tree.audio.loop = true;*/

        walkAudio = 0;
        lastLegFrame = -1;

		//Debug.Log ("THERE IS SOMETHING BROKEN HERE!!!!!!!!!\nPLEASE FIX THIS!!!!\nI CAN\'T PLAY THE GAME WITH THIS LINE UNCOMMENTED\nMAKE SURE TO RUN YOUR CODE BEFORE PUSHING!!!!!\nNO CODE SHOULD BE PUSHED THAT REQUIRES UNCOMMENTING!!!!!\nTHIS IS IN FILE TreeStateActive.cs");
        Tree.BodyParts.Trunk.audio.clip = Tree.Sounds.Walk[walkAudio];
    }

    public override void Update()
    {        
        float bonusSpeed = (Tree.BonusSpeedTimer > 0f) ? 1.2f : 1f;
        
        // Update velocity
        Vector2 velocity = new Vector2(Input.GetAxis("LSX"), Input.GetAxis("LSY")) * Tree.Speed * bonusSpeed * Time.deltaTime;

        // Handle keyboard
        if(Input.anyKey)
        {
            velocity = Vector2.zero;

            if (Input.GetKey(KeyCode.LeftArrow)) velocity.x = -1f;
            if (Input.GetKey(KeyCode.RightArrow)) velocity.x = 1f;
            if (Input.GetKey(KeyCode.UpArrow)) velocity.y = 1f;
            if (Input.GetKey(KeyCode.DownArrow)) velocity.y = -1f;

            velocity *= Tree.Speed * bonusSpeed * Time.deltaTime;
        }

        if(velocity != Vector2.zero)
        {
            // Get which frame the leg animation is on
            AnimatorStateInfo stateInfo = Tree.BodyParts.Legs.GetComponent<Animator>().GetCurrentAnimatorStateInfo(0);
            float animationPercent = (stateInfo.normalizedTime % 1.0f);
            int currentLegFrame = Mathf.FloorToInt(animationPercent * 3f) + 1;

			//Debug.Log ("THERE IS ALSO SOMETHING BROKEN HERE!!!!!!!!!\nPLEASE FIX THIS!!!!\nI CAN\'T PLAY THE GAME WITH THIS LINE UNCOMMENTED\nMAKE SURE TO RUN YOUR CODE BEFORE PUSHING!!!!!\nNO CODE SHOULD BE PUSHED THAT REQUIRES UNCOMMENTING!!!!!\nTHIS IS IN FILE TreeStateActive.cs");
			
            if (!Tree.BodyParts.Trunk.audio.isPlaying && currentLegFrame != lastLegFrame)
            {
                lastLegFrame = currentLegFrame;
                walkAudio++;

                Tree.BodyParts.Trunk.audio.clip = Tree.Sounds.Walk[walkAudio % Tree.Sounds.Walk.Length];

                Tree.BodyParts.Trunk.audio.Play();
            }
            
        }
        else
        {  
            Tree.BodyParts.Trunk.audio.Stop();

            walkAudio = 1;
            lastLegFrame = -1;
        }

        Tree.rigidbody2D.velocity = velocity;

        Vector3 grabOffset = Tree.transform.position + new Vector3(0f, 0.25f);

        // Update all NPC's that are within grabbing range
        foreach(GameObject npc in GameObject.FindGameObjectsWithTag("NPC"))
        {
            int listIndex = npcsInRange.IndexOf(npc);
            bool inRange = Vector3.Distance(Tree.transform.position, npc.transform.position) <= 1.5f;

            if(inRange && (listIndex == -1)) npcsInRange.Add(npc);
            else if (!inRange && (listIndex != -1))
            {
                npcsInRange.RemoveAt(listIndex);

                // Untag NPC if necessary
                npc.GetComponent<AIController>().IsTaggedByTree = false;
            }
        }

        // Show LT icons if NPCs are in range
        GameObject[] closestNPCs = GetTwoClosestNPCs();

        if (closestNPCs.Length == 1)
        {
            lt1.GetComponent<LTScript>().Initialize(closestNPCs[0]);
            lt2.GetComponent<LTScript>().Initialize(null);
        }
        else if (closestNPCs.Length == 2)
        {
            lt1.GetComponent<LTScript>().Initialize(closestNPCs[0]);
            lt2.GetComponent<LTScript>().Initialize(closestNPCs[1]);
        }
        else
        {
            lt1.GetComponent<LTScript>().Initialize(null);
            lt2.GetComponent<LTScript>().Initialize(null);
        }

        // See if we are going to grab
        if(Input.GetAxis("LT") > 0.5f && closestNPCs.Length > 0)
        {
            // Send messages for grabbed NPCs
            MessageCenter.Instance.Broadcast(new PlayerGrabbedNPCsMessage(new List<GameObject>() { closestNPCs[0] }));

            closestNPCs[0].SetActive(false);

            if(closestNPCs[0].name.Contains("Enemy"))
            {
                Tree.ActualAxeMan = closestNPCs[0];

                Tree.StartActiveAxeManMinigame();
            }
            else if (closestNPCs[0].GetComponent<AIController>().isCritterType || Tree.BonusPoisonTimer > 0f)
            {
                TreeStateEatingMinigameMashInstant.Data data = new TreeStateEatingMinigameMashInstant.Data(closestNPCs[0]);

                Tree.ChangeState("EatingMinigameMashInstant", data);
            }
            else
            {
                TreeStateEatingMinigameWrangle.Data data = new TreeStateEatingMinigameWrangle.Data(closestNPCs, true);

                Tree.ChangeState("EatingMinigameWrangle", data);
            }

            return;
        }

        ChangeSpritesToDirection(velocity);
    }

    private GameObject[] GetTwoClosestNPCs()
    {
        // Trivial cases
        if (npcsInRange.Count == 0) return new GameObject[0];
        else if (npcsInRange.Count == 1) return new GameObject[1] { npcsInRange[0] };
        //else if (npcsInRange.Count == 2) return new GameObject[2] { npcsInRange[0], npcsInRange[1] };
        else
        {
            GameObject[] closestNPCs = new GameObject[2];

            // closest 1 is closer than closest 2
            float closest1 = float.MaxValue, closest2 = float.MaxValue;

            foreach (GameObject npc in npcsInRange)
            {
                if (!npc.GetComponent<AIController>().IsTaggedByTree)
                {
                    float distance = Vector3.Distance(Tree.transform.position, npc.transform.position);

                    if (distance < closest1)
                    {
                        closest2 = closest1;
                        closestNPCs[1] = closestNPCs[0];

                        closest1 = distance;
                        closestNPCs[0] = npc;
                    }
                    else if (distance < closest2)
                    {
                        closest2 = distance;
                        closestNPCs[1] = npc;
                    }
                }
            }

            return new GameObject[1] { closestNPCs[0] };
        }
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
        legsAnimator.speed = (velocity.magnitude / maxVelocity) * (2f / 3f);
        
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

        /*Tree.audio.Stop();
        Tree.audio.volume = 1f;
        Tree.audio.loop = false;*/

        GameObject.Destroy(lt1);
        GameObject.Destroy(lt2);

        ChangeSpritesToDirection(Vector2.zero);
    }
}