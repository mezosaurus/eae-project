using UnityEngine;

public class TreeStateAxeManMinigameWaitForChop : TreeState
{
    /*private GameObject aButton;*/
    private GameObject axeMan;
    
    public override void Enter(object data)
    {
        //Tree.Active = true;
        GlobalGameStateManager.PosessionState = PosessionState.NON_EXORCISABLE;
        Tree.Eating = true;

        Tree.BodyParts.LeftArm.SetActive(true);
        Tree.BodyParts.RightArm.SetActive(true);
        Tree.BodyParts.RightUpperArm.SetActive(false);
        Tree.BodyParts.LeftUpperArm.SetActive(false);
        Tree.BodyParts.MinigameCircle.SetActive(false);
        Tree.BodyParts.RightGrabbedNPC.SetActive(false);

        Tree.BodyParts.Face.GetComponent<SpriteRenderer>().sprite = Tree.Sprites.Face.EyesClosed;

        axeMan = GameObject.FindGameObjectWithTag("AxeManKillActiveTree");

        MessageCenter.Instance.Broadcast(new CameraZoomAndFocusMessage2(Tree.transform.position + new Vector3(0f, 0.7f), 1.5f, 0.25f));

        // Disable all unecessary systems
        SoundManager soundManager = GameObject.FindObjectOfType<SoundManager>();
        GameObject levelGUI = GameObject.Find("LevelGUI");
        GameObject mainCamera = GameObject.FindGameObjectWithTag("MainCamera");

        Tree.DisabledForMinigame = new System.Collections.Generic.List<GameObject>();

        if (soundManager != null)
        {
            soundManager.gameObject.SetActive(false);
            Tree.DisabledForMinigame.Add(soundManager.gameObject);
        }

        if (levelGUI != null)
        {
            levelGUI.SetActive(false);
            Tree.DisabledForMinigame.Add(levelGUI);
        }

        mainCamera.audio.Stop();
    }

    /*private void SetAlpha(Transform root, float alpha)
    {
        SpriteRenderer s = root.GetComponent<SpriteRenderer>();

        if (s != null) s.color = new Color(1f, 1f, 1f, alpha);

        foreach (Transform child in root)
        {
            SetAlpha(child, alpha);
        }
    }

    public override void OnTriggerEnter(Collider2D collider)
    {
        if (collider.tag == "PossessorTrigger")
        {
            SetAlpha(Tree.transform, 0.75f);
            aButton.GetComponent<SpriteRenderer>().color = new Color(1f, 1f, 1f, 1f);
        }
    }

    public override void OnTriggerExit(Collider2D collider)
    {
        if (collider.tag == "PossessorTrigger")
        {
            SetAlpha(Tree.transform, 1f);
            aButton.GetComponent<SpriteRenderer>().color = new Color(1f, 1f, 1f, 0.4901960784f);
        }
    }

    public override void Update()
    {
        if(Tree.Dead)
        {
            aButton.SetActive(false);

            return;
        }

        if (GameObject.FindGameObjectWithTag("Possessor") != null) aButton.SetActive(true);
        else aButton.SetActive(false);
    }

    public override void Leave()
    {
        SetAlpha(Tree.transform, 1f);
        GameObject.Destroy(aButton);
    }*/

    public override void UpdateSorting()
    {
        Tree.BodyParts.Trunk.GetComponent<SpriteRenderer>().sortingOrder = 800;

        int i = Tree.BodyParts.Trunk.GetComponent<SpriteRenderer>().sortingOrder;

        Tree.BodyParts.Face.GetComponent<SpriteRenderer>().sortingOrder = i + 1;
        Tree.BodyParts.LeftArm.GetComponent<SpriteRenderer>().sortingOrder = i + 1;
        Tree.BodyParts.RightArm.GetComponent<SpriteRenderer>().sortingOrder = i + 1;
        Tree.BodyParts.Legs.GetComponent<SpriteRenderer>().sortingOrder = i - 1;
        Tree.BodyParts.MinigameCircle.GetComponent<SpriteRenderer>().sortingOrder = i + 7;
        Tree.BodyParts.Axe.GetComponent<SpriteRenderer>().sortingOrder = i + 3;
        Tree.AxeMan.GetComponent<SpriteRenderer>().sortingOrder = i + 8;
    }
}
