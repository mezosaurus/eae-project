using UnityEngine;

public class TreeStateAxeManMinigameEatingLastWords : TreeState
{
    private static string[] Buttons = { "A", "B", "X", "Y" };
    private const float TextureRatio = 4.923809524f;
    private const float TimeFactor = 2f;

    private float maxWidth, maxHeight;
    private float minWidth, minHeight;
    private float percentage, direction;
    private int button;
    private float buttonScale, buttonScaleDirection;


    public override void Enter(object data)
    {
        maxHeight = Mathf.Round(Screen.height * 0.25f);
        maxWidth = Mathf.Round(maxHeight * TextureRatio);
        minHeight = Mathf.Round(Screen.height * 0.15f);
        minWidth = Mathf.Round(minHeight * TextureRatio);

        percentage = 0f;
        direction = 1f;

        buttonScale = 1f;
        buttonScaleDirection = 1f;

        // Choose a random button
        float range = Random.Range(0f, 1f);

        if (range <= 0.25f) button = 1;
        else if (range > 0.25f && range <= 0.5f) button = 2;
        else if (range > 0.5f && range <= 0.75f) button = 0;
        else if (range > 0.75f && range <= 1f) button = 3;

        //Tree.BodyParts.Trunk.audio.rolloffMode = AudioRolloffMode.Logarithmic;
        Tree.BodyParts.Trunk.audio.volume = 0.5f;
        Tree.BodyParts.Trunk.audio.clip = Tree.Sounds.AxeManLastWords[Random.Range(0, Tree.Sounds.AxeManLastWords.Length)];
        Tree.BodyParts.Trunk.audio.Play();
    }

    public override void Update()
    {
        if (!Tree.BodyParts.Trunk.audio.isPlaying)
        {
            percentage += (Time.deltaTime * TimeFactor * direction);

            if (percentage <= 0f)
            {
                percentage = 0f;
                direction = 1f;
            }

            if (percentage >= 1f)
            {
                percentage = 1f;
                direction = -1f;
            }

            if(Input.GetButton(Buttons[button]))
            {
                Tree.ChangeState("AxeManMinigameEatingLastHalf");
            }

            buttonScale += (Time.deltaTime * buttonScaleDirection * 2f);

            if (buttonScale > 1.25f)
            {
                buttonScale = 1.25f;
                buttonScaleDirection = -1f;
            }

            if (buttonScale < 0.75f)
            {
                buttonScale = 0.75f;
                buttonScaleDirection = 1f;
            }
        }
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
        Tree.BodyParts.LeftLowerForegroundArm.GetComponent<SpriteRenderer>().sortingOrder = i + 2;
        Tree.BodyParts.LeftLowerBackgroundArm.GetComponent<SpriteRenderer>().sortingOrder = i + 1;
        Tree.BodyParts.RightUpperArm.GetComponent<SpriteRenderer>().sortingOrder = i + 1;
        Tree.BodyParts.RightLowerForegroundArm.GetComponent<SpriteRenderer>().sortingOrder = i + 5;
        Tree.BodyParts.RightLowerBackgroundArm.GetComponent<SpriteRenderer>().sortingOrder = i + 2;
        Tree.BodyParts.Legs.GetComponent<SpriteRenderer>().sortingOrder = i - 1;
        Tree.BodyParts.MinigameCircle.GetComponent<SpriteRenderer>().sortingOrder = i + 7;
        Tree.BodyParts.Axe.GetComponent<SpriteRenderer>().sortingOrder = i + 3;
        //Tree.AxeMan.GetComponent<SpriteRenderer>().sortingOrder = i + 0;
    }

    public override void OnGUI()
    {
        if (!Tree.BodyParts.Trunk.audio.isPlaying)
        {
            float width = Tree.Sprites.EatingMinigame.Buttons[0].width * buttonScale;
            float height = Tree.Sprites.EatingMinigame.Buttons[0].height * buttonScale;
            Vector3 position = Camera.main.WorldToScreenPoint(Tree.BodyParts.MinigameCircle.transform.position + new Vector3(0f, 0.6f));

            GUI.DrawTexture(new Rect(position.x - (width / 2f), position.y - (height / 2f), width, height), Tree.Sprites.EatingMinigame.Buttons[button]);

            float x = Screen.width / 2f;
            float y = Screen.height / 4f;
            float w = Mathf.Lerp(minWidth, maxWidth, percentage);
            float h = Mathf.Lerp(minHeight, maxHeight, percentage);
            Rect rect = new Rect(x - (w / 2f), y - (h / 2f), w, h);

            GUI.DrawTexture(rect, Tree.Sprites.FinishHim);
        }
    }

    public override void Leave()
    {
        //Tree.BodyParts.Trunk.audio.priority = 128;
        Tree.BodyParts.Trunk.audio.volume = 1f;
    }

    public class Data
    {
        public int Button;


        public Data(int button)
        {
            Button = button;
        }
    }
}
