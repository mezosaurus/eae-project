﻿using UnityEngine;

public class TreeStateEatingTutorial : TreeState
{
    private GameObject npc;
    private NPCData npcData;
    private float timeElapsed;


    public override void Enter(object data)
    {
        GlobalGameStateManager.PosessionState = PosessionState.NON_EXORCISABLE;
        Tree.Eating = true;
        
        Tree.BodyParts.Eyes.SetActive(true);
        Tree.BodyParts.RightGrabbedNPC.SetActive(false);
        Tree.BodyParts.MinigameCircle.SetActive(false);

        // Get parameters
        Data parameters = data as Data;

        npc = parameters.NPC;
        npcData = GlobalGameStateManager.NPCData[npc.GetComponent<AIController>().SkinType];

        Tree.BodyParts.Face.GetComponent<Animator>().enabled = true;
        Tree.BodyParts.Face.GetComponent<SpriteRenderer>().sprite = Tree.Sprites.Face.Crazy;
        Tree.BodyParts.Face.GetComponent<Animator>().SetTrigger(npcData.AnimationTrigger);

        // Set arm angles
        Tree.BodyParts.RightUpperArm.transform.eulerAngles = new Vector3(0f, 0f, npcData.RightUpperArmEndAngle);
        Tree.BodyParts.RightLowerForegroundArm.transform.eulerAngles = new Vector3(0f, 0f, npcData.RightLowerArmEndAngle);

        // Play chew sound
        //if (Tree.audio.isPlaying) Tree.audio.Stop();

        Tree.audio.clip = Tree.Sounds.Chew;

        Tree.audio.Play();

        timeElapsed = 0f;
    }

    private void Eat()
    {
        GlobalGameStateManager.SoulConsumedTimer = 3.5f;

        if (TutorialManager.Instance.Phase == 17) TutorialManager.Instance.AdvancePhase();

        Tree.audio.Stop();

		/*SoundManager soundManager = GameObject.FindObjectOfType<SoundManager>();
		soundManager.ResumeMusic();*/

        Tree.audio.clip = Tree.Sounds.SoulConsumed;
        Tree.audio.Play();

        MessageCenter.Instance.Broadcast(new NPCEatenMessage(npc));
        MessageCenter.Instance.Broadcast(new CameraChangeFollowedMessage(Tree.transform, new Vector3(0f, 0.15f)));
        MessageCenter.Instance.Broadcast(new CameraZoomMessage(4f, 10f));

        /*Tree.BodyParts.FlameEyes.SetActive(true);
        Tree.BodyParts.FlameEyes.particleSystem.Play();*/

        if(npc.GetComponent<AIController>().isCritterType)
        {
            switch(npc.GetComponent<CritterController>().critterUpgradeType)
            {
                case CritterType.poisonous:
                    Tree.BonusPoisonTimer = Tree.MaxBonusTime;
                    break;

                default:
                    Tree.BonusSpeedTimer = Tree.MaxBonusTime;
                    break;
            }
        }

        GameObject.Destroy(npc);
    }

    public override void Update()
    {
        if(!Tree.audio.isPlaying)
        {
            Eat();
            Tree.ChangeState("Active");

            return;
        }
        
        // Update arm rotation
        if(timeElapsed < npcData.ArmEatTime)
            timeElapsed += Time.deltaTime;

        float percentage = Mathf.Clamp(timeElapsed / npcData.ArmEatTime, 0f, 1f);

        float upperAngle = npcData.RightUpperArmEndAngle + ((npcData.RightUpperArmFinalAngle - npcData.RightUpperArmEndAngle) * percentage);
        float lowerAngle = npcData.RightLowerArmEndAngle + ((npcData.RightLowerArmFinalAngle - npcData.RightLowerArmEndAngle) * percentage);

        Tree.BodyParts.RightUpperArm.transform.localEulerAngles = new Vector3(0f, 0f, upperAngle);
        Tree.BodyParts.RightLowerForegroundArm.transform.localEulerAngles = new Vector3(0f, 0f, lowerAngle);
    }

    public override void UpdateSorting()
    {
        Tree.BodyParts.Trunk.GetComponent<SpriteRenderer>().sortingOrder = 800;
        
        int i = Tree.BodyParts.Trunk.GetComponent<SpriteRenderer>().sortingOrder;

        //Tree.BodyParts.FlameEyes.particleSystem.renderer.sortingOrder = i + 1;
        Tree.BodyParts.Eyes.GetComponent<SpriteRenderer>().sortingOrder = i + 2;
        Tree.BodyParts.Face.GetComponent<SpriteRenderer>().sortingOrder = i + 4;
        Tree.BodyParts.LeftUpperArm.GetComponent<SpriteRenderer>().sortingOrder = i + 1;
        Tree.BodyParts.LeftLowerForegroundArm.GetComponent<SpriteRenderer>().sortingOrder = i + 2;
        Tree.BodyParts.LeftLowerBackgroundArm.GetComponent<SpriteRenderer>().sortingOrder = i + 1;
        //Tree.BodyParts.LeftArm.GetComponent<SpriteRenderer>().sortingOrder = i + 1;
        Tree.BodyParts.RightUpperArm.GetComponent<SpriteRenderer>().sortingOrder = i + 2;
        Tree.BodyParts.RightLowerForegroundArm.GetComponent<SpriteRenderer>().sortingOrder = i + 3;
        Tree.BodyParts.RightLowerBackgroundArm.GetComponent<SpriteRenderer>().sortingOrder = i + 5;
        Tree.BodyParts.Legs.GetComponent<SpriteRenderer>().sortingOrder = i - 1;
    }

    public override void Leave()
    {
        Tree.BodyParts.Eyes.SetActive(false);
        Tree.BodyParts.Face.GetComponent<Animator>().enabled = false;
        Tree.BodyParts.RightGrabbedNPC.SetActive(true);
        //Tree.BodyParts.Face.SetActive(true);
        //Tree.BodyParts.GrabbedNPC.SetActive(true);
    }

    public class Data
    {
        public GameObject NPC;


        public Data(GameObject npc)
        {
            NPC = npc;
        }
    }
}
