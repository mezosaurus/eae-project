using UnityEngine;

public class TreeStateAxeManMinigameEatingLastHalf : TreeState
{
    private const float UpperArmStartAngle = 377.36395f;
    private const float LowerArmStartAngle = 298.9052f;
    private const float UpperArmEndAngle = 368.436346f;
    private const float LowerArmEndAngle = 318.2794f;
    private const float UpperArmEnd2Angle = 349.2221f;
    private const float LowerArmEnd2Angle = 292.7946f;

    private const float ShakeAmount = 0.01f;

    private int frame;
    private float frameTimer, timeElapsed, timer;
    private Vector3 originalCameraPosition;
    private uint phase;


    public override void Enter(object data)
    {        
        //Tree.BodyParts.Eyes.SetActive(true);
        //Tree.BodyParts.RightGrabbedNPC.SetActive(false);
        //Tree.BodyParts.MinigameCircle.SetActive(false);

        frame = 11;
        frameTimer = 0f;
        timeElapsed = 0f;
        phase = 0;

        originalCameraPosition = Camera.main.transform.position;

        Tree.BodyParts.Face.GetComponent<SpriteRenderer>().sprite = Tree.Sprites.EatingAxeMan[frame];

        Tree.BodyParts.Trunk.audio.rolloffMode = AudioRolloffMode.Linear;
        Tree.BodyParts.Trunk.audio.clip = Tree.Sounds.ChewAxeMan[1];
        Tree.BodyParts.Trunk.audio.Play();
    }

    public override void Update()
    {
        Tree.BodyParts.RightLowerBackgroundArm.transform.localEulerAngles = new Vector3(0f, 0f, 0f);
        
        float percentage;
        float upperAngle, lowerAngle;
        
        if (phase == 1)
        {            
            /*if (Tree.audio.volume > 0f)
            {
                Tree.audio.volume -= (Time.deltaTime * 1.785714286f);
            }
            else
            {
                phase = 2;
                timer = 0f;
            }*/

            if(!Tree.BodyParts.Trunk.audio.isPlaying)
            {
                phase = 2;
                timer = 0f;
            }


            return;
        }
        if(phase == 2)
        {
            timer += Time.deltaTime;

            if(timer > 1f)
            {
                phase = 3;
                timer = 0f;

                Tree.audio.volume = 1f;

                // Choose a saying clip to play
                Tree.BodyParts.Trunk.audio.clip = Tree.Sounds.Saying[Random.Range(0, Tree.Sounds.Saying.Length)];
                Tree.BodyParts.Trunk.audio.Play();
            }

            return;
        }
        if(phase == 3)
        {

            if (Tree.audio.volume > 0f)
            {
                Tree.audio.volume -= (Time.deltaTime * 0.8f);
            }
            
            if(!Tree.BodyParts.Trunk.audio.isPlaying)
            {
                /*timer += Time.deltaTime;

                if (timer > 0.25f)
                {*/
                    timer = 0f;
                    phase = 4;
                    Tree.BodyParts.Trunk.audio.clip = Tree.Sounds.Laugh[Random.Range(0, Tree.Sounds.Laugh.Length)];
                    Tree.BodyParts.Trunk.audio.Play();
                //}
            }

            percentage = 1f - Tree.audio.volume;

            upperAngle = UpperArmEndAngle + ((UpperArmEnd2Angle - UpperArmEndAngle) * percentage);
            lowerAngle = LowerArmEndAngle + ((LowerArmEnd2Angle - LowerArmEndAngle) * percentage);

            Tree.BodyParts.RightUpperArm.transform.localEulerAngles = new Vector3(0f, 0f, upperAngle);
            Tree.BodyParts.RightLowerForegroundArm.transform.localEulerAngles = new Vector3(0f, 0f, lowerAngle);

            return;
        }
        if(phase == 4)
        {
            /*if (Tree.audio.volume > 0f)
            {
                Tree.audio.volume -= (Time.deltaTime * 0.5f);
            }*/

            if (!Tree.BodyParts.Trunk.audio.isPlaying)
            {
                phase = 5;
                Tree.audio.volume = 1f;
            }

            /*percentage = 1f - Tree.audio.volume;

            upperAngle = UpperArmEndAngle + ((UpperArmEnd2Angle - UpperArmEndAngle) * percentage);
            lowerAngle = LowerArmEndAngle + ((LowerArmEnd2Angle - LowerArmEndAngle) * percentage);

            Tree.BodyParts.RightUpperArm.transform.localEulerAngles = new Vector3(0f, 0f, upperAngle);
            Tree.BodyParts.RightLowerForegroundArm.transform.localEulerAngles = new Vector3(0f, 0f, lowerAngle);*/

            return;
        }
        if (phase == 5)
        {            
            MessageCenter.Instance.Broadcast(new NPCEatenMessage(Tree.ActualAxeMan));
            MessageCenter.Instance.Broadcast(new CameraChangeFollowedMessage(Tree.transform, new Vector3(0f, 0.7f)));
            MessageCenter.Instance.Broadcast(new CameraZoomMessage(4f, 10f));
            
            GlobalGameStateManager.SoulConsumedTimer = 3.5f;

            Tree.audio.Stop();

			SoundManager soundManager = GameObject.FindObjectOfType<SoundManager>();
			if(soundManager != null) soundManager.ResumeMusic();

            Tree.audio.clip = Tree.Sounds.SoulConsumed2;
            Tree.audio.Play();

            foreach(GameObject go in Tree.DisabledForMinigame)
            {
                go.SetActive(true);
            }

            Tree.DisabledForMinigame = new System.Collections.Generic.List<GameObject>();

            GameObject.Destroy(Tree.ActualAxeMan);

            Tree.ChangeState("Active");

            return;
        }
        
        frameTimer += Time.deltaTime;

        if(frameTimer > 0.05f)
        {
            frame++;
            frameTimer = 0f;

            Tree.BodyParts.Face.GetComponent<SpriteRenderer>().sprite = Tree.Sprites.EatingAxeMan[frame];

            if(frame == 39)
            {
                Tree.BodyParts.Eyes.SetActive(false);
                Tree.BodyParts.FlameEyes.SetActive(true);
                Tree.BodyParts.FlameEyes.particleSystem.renderer.sortingOrder = 802;
                Tree.BodyParts.FlameEyes.particleSystem.Simulate(0f);
                Tree.BodyParts.Face.GetComponent<SpriteRenderer>().sprite = Tree.Sprites.Face.Crazy;

                Tree.BodyParts.Trunk.audio.clip = Tree.Sounds.Burp;
                Tree.BodyParts.Trunk.audio.Play();

                phase = 1;

                return;
            }
        }

        // Shake the camera
        Vector2 offset = Random.insideUnitCircle * ShakeAmount;

        Camera.main.transform.position = originalCameraPosition + (Vector3)offset;

        if (timeElapsed < 0.5f)
            timeElapsed += Time.deltaTime;

        if (timeElapsed > 0.5f) timeElapsed = 0.5f;

        percentage = timeElapsed / 0.5f;

        upperAngle = UpperArmStartAngle + ((UpperArmEndAngle - UpperArmStartAngle) * percentage);
        lowerAngle = LowerArmStartAngle + ((LowerArmEndAngle - LowerArmStartAngle) * percentage);

        Tree.BodyParts.RightUpperArm.transform.localEulerAngles = new Vector3(0f, 0f, upperAngle);
        Tree.BodyParts.RightLowerForegroundArm.transform.localEulerAngles = new Vector3(0f, 0f, lowerAngle);
        
        /*// Update arm rotation
        if(timeElapsed < npcData.ArmEatTime)
            timeElapsed += Time.deltaTime;

        float percentage = Mathf.Clamp(timeElapsed / npcData.ArmEatTime, 0f, 1f);

        float upperAngle = npcData.RightUpperArmEndAngle + ((npcData.RightUpperArmFinalAngle - npcData.RightUpperArmEndAngle) * percentage);
        float lowerAngle = npcData.RightLowerArmEndAngle + ((npcData.RightLowerArmFinalAngle - npcData.RightLowerArmEndAngle) * percentage);

        Tree.BodyParts.RightUpperArm.transform.localEulerAngles = new Vector3(0f, 0f, upperAngle);
        Tree.BodyParts.RightLowerForegroundArm.transform.localEulerAngles = new Vector3(0f, 0f, lowerAngle);*/
    }

    public override void UpdateSorting()
    {
        /*Tree.BodyParts.Trunk.GetComponent<SpriteRenderer>().sortingOrder = 800;
        
        int i = Tree.BodyParts.Trunk.GetComponent<SpriteRenderer>().sortingOrder;

        //Tree.BodyParts.FlameEyes.particleSystem.renderer.sortingOrder = i + 1;
        Tree.BodyParts.Eyes.GetComponent<SpriteRenderer>().sortingOrder = i + 2;
        Tree.BodyParts.Face.GetComponent<SpriteRenderer>().sortingOrder = i + 4;
        //Tree.BodyParts.LeftArm.GetComponent<SpriteRenderer>().sortingOrder = i + 1;
        Tree.BodyParts.RightUpperArm.GetComponent<SpriteRenderer>().sortingOrder = i + 2;
        Tree.BodyParts.RightLowerForegroundArm.GetComponent<SpriteRenderer>().sortingOrder = i + 5;
        Tree.BodyParts.RightLowerBackgroundArm.GetComponent<SpriteRenderer>().sortingOrder = i + 3;
        Tree.BodyParts.Legs.GetComponent<SpriteRenderer>().sortingOrder = i - 1;*/

        Tree.BodyParts.Trunk.GetComponent<SpriteRenderer>().sortingOrder = 800;

        int i = Tree.BodyParts.Trunk.GetComponent<SpriteRenderer>().sortingOrder;

        Tree.BodyParts.Eyes.GetComponent<SpriteRenderer>().sortingOrder = i + 2;
        Tree.BodyParts.Face.GetComponent<SpriteRenderer>().sortingOrder = i + 4;
        Tree.BodyParts.LeftUpperArm.GetComponent<SpriteRenderer>().sortingOrder = i + 1;
        Tree.BodyParts.RightUpperArm.GetComponent<SpriteRenderer>().sortingOrder = i + 1;
        Tree.BodyParts.RightLowerForegroundArm.GetComponent<SpriteRenderer>().sortingOrder = i + 5;
        Tree.BodyParts.RightLowerBackgroundArm.GetComponent<SpriteRenderer>().sortingOrder = i + 2;
        Tree.BodyParts.Legs.GetComponent<SpriteRenderer>().sortingOrder = i - 1;
        Tree.BodyParts.MinigameCircle.GetComponent<SpriteRenderer>().sortingOrder = i + 7;
        Tree.BodyParts.Axe.GetComponent<SpriteRenderer>().sortingOrder = i + 3;
        //Tree.AxeMan.GetComponent<SpriteRenderer>().sortingOrder = i + 0;
    }

    public override void Leave()
    {
        //Tree.BodyParts.Eyes.SetActive(false);
        Tree.BodyParts.RightGrabbedNPC.SetActive(true);
        //Tree.BodyParts.Face.SetActive(true);
        //Tree.BodyParts.GrabbedNPC.SetActive(true);*/

        Camera.main.transform.position = originalCameraPosition;
        
        Tree.BodyParts.Trunk.audio.priority = 128;
        Tree.BodyParts.Trunk.audio.volume = 1f;
        Tree.BodyParts.Trunk.audio.rolloffMode = AudioRolloffMode.Logarithmic;

        Tree.Dead = false;
        Tree.AxeMan = null;
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
