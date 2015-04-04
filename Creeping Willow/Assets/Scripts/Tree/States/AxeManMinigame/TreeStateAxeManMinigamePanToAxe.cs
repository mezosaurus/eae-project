using UnityEngine;

public class TreeStateAxeManMinigamePanToAxe : TreeState
{
    private float timer;
    
    public override void Enter(object data)
    {
        Tree.Active = true;

        Tree.BodyParts.LeftArm.SetActive(false);
        Tree.BodyParts.RightArm.SetActive(false);
        Tree.BodyParts.RightUpperArm.SetActive(true);
        Tree.BodyParts.LeftUpperArm.SetActive(true);
        Tree.BodyParts.MinigameCircle.SetActive(true);

        timer = 0f;

        MessageCenter.Instance.Broadcast(new CameraZoomAndFocusMessage(Tree.BodyParts.Axe.transform.position, 0.25f, 1.2f, 20f));
    }

    public override void Update()
    {
        if (Camera.main.orthographicSize == Camera.main.GetComponent<CameraScript>().TargetSize)
        {
            timer += Time.deltaTime;

            if (!Tree.audio.isPlaying)
            {
                Tree.audio.clip = Tree.Sounds.AxeManMinigameMusic;
                //Tree.audio.volume = 0.8f;
                Tree.audio.Play();
            }

            if(timer > 0.01f)
            {
                Tree.ChangeState("AxeManMinigameWrangleAxe");
            }
        }
    }

    public override void UpdateSorting()
    {
        Tree.BodyParts.Trunk.GetComponent<SpriteRenderer>().sortingOrder = 800;

        int i = Tree.BodyParts.Trunk.GetComponent<SpriteRenderer>().sortingOrder;

        Tree.BodyParts.Face.GetComponent<SpriteRenderer>().sortingOrder = i + 1;
        Tree.BodyParts.LeftUpperArm.GetComponent<SpriteRenderer>().sortingOrder = i + 1;
        Tree.BodyParts.RightUpperArm.GetComponent<SpriteRenderer>().sortingOrder = i + 1;
        Tree.BodyParts.RightLowerForegroundArm.GetComponent<SpriteRenderer>().sortingOrder = i + 4;
        Tree.BodyParts.RightLowerBackgroundArm.GetComponent<SpriteRenderer>().sortingOrder = i + 2;
        Tree.BodyParts.Legs.GetComponent<SpriteRenderer>().sortingOrder = i - 1;
        Tree.BodyParts.MinigameCircle.GetComponent<SpriteRenderer>().sortingOrder = i + 7;
        Tree.BodyParts.Axe.GetComponent<SpriteRenderer>().sortingOrder = i + 3;
        Tree.AxeMan.GetComponent<SpriteRenderer>().sortingOrder = i + 0;
    }
}
