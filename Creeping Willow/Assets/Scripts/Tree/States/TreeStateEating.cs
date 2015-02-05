using UnityEngine;

public class TreeStateEating : TreeState
{
    protected const float UpperArmStartAngle = 20.5433f;
    protected const float UpperArmEndAngle = 27.94905f;
    protected const float LowerArmStartAngle = -78.57207f;
    protected const float LowerArmEndAngle = -72.60649f;


    private float timeElapsed;


    public override void Enter(object data)
    {
        Tree.BodyParts.Eyes.SetActive(true);
        Tree.BodyParts.RightGrabbedNPC.SetActive(false);
        Tree.BodyParts.MinigameCircle.SetActive(false);
        Tree.BodyParts.Face.GetComponent<Animator>().enabled = true;
        Tree.BodyParts.Face.GetComponent<Animator>().SetTrigger("Bopper");

        // Set arm angles
        Tree.BodyParts.RightUpperArm.transform.eulerAngles = new Vector3(0f, 0f, UpperArmStartAngle);
        Tree.BodyParts.RightLowerForegroundArm.transform.eulerAngles = new Vector3(0f, 0f, LowerArmStartAngle);

        // Play chew sound
        Tree.audio.clip = Tree.Sounds.Chew;

        Tree.audio.Play();

        timeElapsed = 0f;
    }

    public override void Update()
    {
        if(!Tree.audio.isPlaying)
        {
            Tree.ChangeState("Active");

            return;
        }
        
        // Update arm rotation
        timeElapsed += Time.deltaTime;

        if (timeElapsed > 1f) timeElapsed = 1f;

        float percentage = timeElapsed / 1f;

        float upperAngle = UpperArmStartAngle + ((UpperArmEndAngle - UpperArmStartAngle) * percentage);
        float lowerAngle = LowerArmStartAngle + ((LowerArmEndAngle - LowerArmStartAngle) * percentage);

        Tree.BodyParts.RightUpperArm.transform.localEulerAngles = new Vector3(0f, 0f, upperAngle);
        Tree.BodyParts.RightLowerForegroundArm.transform.localEulerAngles = new Vector3(0f, 0f, lowerAngle);
    }

    public override void UpdateSorting()
    {
        Tree.BodyParts.Trunk.GetComponent<SpriteRenderer>().sortingOrder = 800;
        
        int i = Tree.BodyParts.Trunk.GetComponent<SpriteRenderer>().sortingOrder;

        Tree.BodyParts.Eyes.GetComponent<SpriteRenderer>().sortingOrder = i + 1;
        Tree.BodyParts.Face.GetComponent<SpriteRenderer>().sortingOrder = i + 3;
        //Tree.BodyParts.LeftArm.GetComponent<SpriteRenderer>().sortingOrder = i + 1;
        Tree.BodyParts.RightUpperArm.GetComponent<SpriteRenderer>().sortingOrder = i + 1;
        Tree.BodyParts.RightLowerForegroundArm.GetComponent<SpriteRenderer>().sortingOrder = i + 2;
        Tree.BodyParts.RightLowerBackgroundArm.GetComponent<SpriteRenderer>().sortingOrder = i + 4;
        Tree.BodyParts.Legs.GetComponent<SpriteRenderer>().sortingOrder = i - 1;
    }

    public override void Leave()
    {
        Tree.BodyParts.Eyes.SetActive(false);
        Tree.BodyParts.Face.GetComponent<Animator>().enabled = false;
        //Tree.BodyParts.Face.SetActive(true);
        //Tree.BodyParts.GrabbedNPC.SetActive(true);
    }
}
