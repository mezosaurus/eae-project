using UnityEngine;

public class TreeStateAxeManMinigameGroan : TreeState
{
    private const float ShakeAmount = 0.04f;
    
    private Vector3 originalCameraPosition;
    
    public override void Enter(object data)
    {
        Tree.audio.clip = Tree.Sounds.Groan;
        Tree.audio.Play();

        originalCameraPosition = GameObject.FindGameObjectWithTag("MainCamera").transform.position;

        Tree.BodyParts.Face.GetComponent<SpriteRenderer>().sprite = Tree.Sprites.Face.Crazy;
    }

    public override void Update()
    {
        if (!Tree.audio.isPlaying)
        {
            Tree.ChangeState("AxeManMinigamePanToAxe");

            return;
        }

        // Shake the camera
        /*Vector3 position = Random.insideUnitSphere * ShakeAmount;

        position.z = originalCameraPosition.z;

        Camera.main.transform.position = position;*/

        Vector2 offset = Random.insideUnitCircle * ShakeAmount;

        Camera.main.transform.position = originalCameraPosition + (Vector3)offset;
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

    public override void Leave()
    {
        Camera.main.transform.position = originalCameraPosition;
    }
}
