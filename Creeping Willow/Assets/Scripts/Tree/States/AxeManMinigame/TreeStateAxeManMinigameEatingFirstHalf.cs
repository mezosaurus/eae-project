using UnityEngine;

public class TreeStateAxeManMinigameEatingFirstHalf : TreeState
{
    private const float UpperArmStartAngle = 371.83527f;
    private const float LowerArmStartAngle = 276f; //294.9953f;
    private const float UpperArmEndAngle = 377.36395f;
    private const float LowerArmEndAngle = 288.3144f; //298.9052f;


    private int frame;
    private float frameTimer, timer, timeElapsed;


    public override void Enter(object data)
    {        
        Tree.BodyParts.Eyes.SetActive(true);
        //Tree.BodyParts.RightGrabbedNPC.SetActive(false);
        Tree.BodyParts.MinigameCircle.SetActive(false);

        Tree.BodyParts.Face.GetComponent<SpriteRenderer>().sprite = Tree.Sprites.EatingAxeMan[0];

        MessageCenter.Instance.Broadcast(new AxeManMinigameAxeManChangePhaseMessage(98765));

        frame = 0;
        frameTimer = 0f;
        timer = 0f;
        timeElapsed = 0f;
    }

    public override void Update()
    {
        Tree.BodyParts.RightLowerBackgroundArm.transform.localEulerAngles = new Vector3(0f, 0f, 6.799903f);
        
        if (frame == 10)
        {
            if (!Tree.BodyParts.Trunk.audio.isPlaying)
            {
                timer += Time.deltaTime;

                if (timer > 1f)
                    Tree.ChangeState("AxeManMinigameEatingLastWords");
            }

            return;
        }
        
        frameTimer += Time.deltaTime;

        if(frameTimer > 0.05f)
        {
            frame++;
            frameTimer = 0f;

            Tree.BodyParts.Face.GetComponent<SpriteRenderer>().sprite = Tree.Sprites.EatingAxeMan[frame];

            if(frame == 10)
            {
                Tree.BodyParts.Face.GetComponent<SpriteRenderer>().sprite = Tree.Sprites.Face.ChewedAxeMan;

                return;
            }

            if(frame == 3)
            {
                Tree.BodyParts.Trunk.audio.priority = 255;
                Tree.BodyParts.Trunk.audio.rolloffMode = AudioRolloffMode.Linear;
                Tree.BodyParts.Trunk.audio.clip = Tree.Sounds.ChewAxeMan[0];
                Tree.BodyParts.Trunk.audio.Play();
            }
        }

        if (timeElapsed < 0.55f)
            timeElapsed += Time.deltaTime;

        if (timeElapsed > 0.55f) timeElapsed = 0.55f;

        float percentage = timeElapsed / 0.55f;

        float upperAngle = UpperArmStartAngle + ((UpperArmEndAngle - UpperArmStartAngle) * percentage);
        float lowerAngle = LowerArmStartAngle + ((LowerArmEndAngle - LowerArmStartAngle) * percentage);

        Tree.BodyParts.RightUpperArm.transform.localEulerAngles = new Vector3(0f, 0f, upperAngle);
        Tree.BodyParts.RightLowerForegroundArm.transform.localEulerAngles = new Vector3(0f, 0f, lowerAngle);
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
        Tree.BodyParts.RightLowerForegroundArm.GetComponent<SpriteRenderer>().sortingOrder = i + 3;
        Tree.BodyParts.RightLowerBackgroundArm.GetComponent<SpriteRenderer>().sortingOrder = i + 5;
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
        //Tree.BodyParts.RightGrabbedNPC.SetActive(true);
        //Tree.BodyParts.Face.SetActive(true);
        //Tree.BodyParts.GrabbedNPC.SetActive(true);*/
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
