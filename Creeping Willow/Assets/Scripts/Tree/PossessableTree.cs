using System;
using System.Collections.Generic;
using UnityEngine;

public class PossessableTree : Possessable
{
    public enum Direction
    {
        Front, FrontRight, Right, BackRight, Back, BackLeft, Left, FrontLeft
    }
    
    public float Speed;
    public bool Eating;
    public bool Dead;
    public List<GameObject> DisabledForMinigame;
    public GameObject AxeMan, ActualAxeMan;
    public Tree.Private.BodyParts BodyParts;
    public Tree.Private.Sprites Sprites;
    public Tree.Private.Sounds Sounds;
    public Tree.Private.Prefabs Prefabs;


    protected Dictionary<string, TreeState> states;
    protected TreeState currentState;


    public readonly float MaxBonusTime = 30f;
    public float BonusSpeedTimer = 0f;
    public float BonusPoisonTimer = 0f;

    public GUISkin BonusSkin;


	protected virtual void CreateStates()
    {        
        states = new Dictionary<string, TreeState>();

        TreeState inactive = new TreeStateInactive();
        TreeState active = new TreeStateActive();
        TreeState eatingMinigameWrangle = new TreeStateEatingMinigameWrangle();
        TreeState eatingMinigameMash = new TreeStateEatingMinigameMash();
        TreeState eatingMinigameMashInstant = new TreeStateEatingMinigameMashInstant();
        TreeState eating = new TreeStateEating();

        // Axe man minigame states
        TreeState axeManMinigameDead = new TreeStateAxeManMinigameDead();
        TreeState axeManMinigameWaitForChop = new TreeStateAxeManMinigameWaitForChop();
        TreeState axeManMinigameGroan = new TreeStateAxeManMinigameGroan();
        TreeState axeManMinigamePanToAxe = new TreeStateAxeManMinigamePanToAxe();
        TreeState axeManMinigameWrangleAxe = new TreeStateAxeManMinigameWrangleAxe();
        TreeState axeManMinigameGrabAxe = new TreeStateAxeManMinigameGrabAxe();
        TreeState axeManMinigameRemoveAxe = new TreeStateAxeManMinigameRemoveAxe();
        TreeState axeManMinigameRaiseAxe = new TreeStateAxeManMinigameRaiseAxe();
        TreeState axeManMinigameDropAxe = new TreeStateAxeManMinigameDropAxe();
        TreeState axeManMinigameLowerToAxeMan = new TreeStateAxeManMinigameLowerToAxeMan();
        TreeState axeManMinigameRaiseAxeMan = new TreeStateAxeManMinigameRaiseAxeMan();
        TreeState axeManMinigameWrangleAxeMan = new TreeStateAxeManMinigameWrangleAxeMan();
        TreeState axeManMinigameMash = new TreeStateAxeManMinigameMash();
        TreeState axeManMinigameEatingFirstHalf = new TreeStateAxeManMinigameEatingFirstHalf();
        TreeState axeManMinigameEatingLastWords = new TreeStateAxeManMinigameEatingLastWords();
        TreeState axeManMinigameEatingLastHalf = new TreeStateAxeManMinigameEatingLastHalf();

        states.Add("Inactive", inactive);
        states.Add("Active", active);
        states.Add("EatingMinigameWrangle", eatingMinigameWrangle);
        states.Add("EatingMinigameMash", eatingMinigameMash);
        states.Add("EatingMinigameMashInstant", eatingMinigameMashInstant);
        states.Add("Eating", eating);

        // Axe man minigame state
        states.Add("AxeManMinigameDead", axeManMinigameDead);
        states.Add("AxeManMinigameWaitForChop", axeManMinigameWaitForChop);
        states.Add("AxeManMinigameGroan", axeManMinigameGroan);
        states.Add("AxeManMinigamePanToAxe", axeManMinigamePanToAxe);
        states.Add("AxeManMinigameWrangleAxe", axeManMinigameWrangleAxe);
        states.Add("AxeManMinigameGrabAxe", axeManMinigameGrabAxe);
        states.Add("AxeManMinigameRemoveAxe", axeManMinigameRemoveAxe);
        states.Add("AxeManMinigameRaiseAxe", axeManMinigameRaiseAxe);
        states.Add("AxeManMinigameDropAxe", axeManMinigameDropAxe);
        states.Add("AxeManMinigameLowerToAxeMan", axeManMinigameLowerToAxeMan);
        states.Add("AxeManMinigameRaiseAxeMan", axeManMinigameRaiseAxeMan);
        states.Add("AxeManMinigameWrangleAxeMan", axeManMinigameWrangleAxeMan);
        states.Add("AxeManMinigameMash", axeManMinigameMash);
        states.Add("AxeManMinigameEatingFirstHalf", axeManMinigameEatingFirstHalf);
        states.Add("AxeManMinigameEatingLastWords", axeManMinigameEatingLastWords);
        states.Add("AxeManMinigameEatingLastHalf", axeManMinigameEatingLastHalf);

        foreach (TreeState state in states.Values) state.Tree = this;

        currentState = (Active) ? states["Active"] : states["Inactive"];

        if (Active) MessageCenter.Instance.Broadcast(new CameraChangeFollowedMessage(transform, Vector3.zero));

        currentState.Enter(null);
    }

    private void LoadCircle()
    {
        Sprites.EatingMinigame.Circle = new Sprite[101];

        for (int i = 0; i < 101; i++) Sprites.EatingMinigame.Circle[i] = Resources.Load<Sprite>("Textures/CircularProgressBar2/CircleProgress" + (i + 1));
    }

    public void StartActiveAxeManMinigame()
    {
        GameObject axeMan = (GameObject)GameObject.Instantiate(Prefabs.AxeManKillActive, transform.position + new Vector3(-1.14f, 0.091f), Quaternion.identity);
        AxeMan = axeMan;

        axeMan.GetComponent<AxeManKillActiveTree>().Instantiate(gameObject, ActualAxeMan);
        ChangeState("AxeManMinigameWaitForChop");
    }

	protected override void Start()
    {
        Dead = false;

        // Setup listener so we can know if we died
        MessageCenter.Instance.RegisterListener(MessageType.PlayerKilled, HandleDeath);
        MessageCenter.Instance.RegisterListener(MessageType.AxeManMinigameTreeChangePhase, HandleChangePhase);

        CreateStates();
        LoadCircle();

        BodyParts.Legs.GetComponent<Animator>().speed = 0f;
	}
	
	public override void possess()
    {
		base.possess ();
		ChangeState("Active");
        MessageCenter.Instance.Broadcast(new CameraChangeFollowedMessage(transform, Vector3.zero));
	}

    public override void exorcise()
    {
		base.exorcise ();

		ChangeState("Inactive");
    }

    protected void OnTriggerEnter2D(Collider2D collider)
    {
        currentState.OnTriggerEnter(collider);
    }

    protected void OnTriggerExit2D(Collider2D collider)
    {
        currentState.OnTriggerExit(collider);
    }

    private void HandleDeath(Message m)
    {        
        PlayerKilledMessage message = m as PlayerKilledMessage;

        if(message.Tree == gameObject)
        {
            Dead = true;
            ActualAxeMan = message.NPC;

            // See if the tree is already in the active state
            if(GlobalGameStateManager.PosessionState == PosessionState.EXORCISABLE && Active)
            {
                StartActiveAxeManMinigame();

                return;
            }
        }
    }

    private void HandleChangePhase(Message m)
    {
        AxeManMinigameTreeChangePhaseMessage message = m as AxeManMinigameTreeChangePhaseMessage;

        if(message.Tree == gameObject)
            ChangeState("AxeManMinigame" + message.Phase);
    }

    private void FixedUpdate()
    {
        currentState.FixedUpdate();
    }

    protected override void GameUpdate()
    {
        base.GameUpdate();

        // Update bonus timers
        if (BonusSpeedTimer > 0f) BonusSpeedTimer -= Time.deltaTime;
        if (BonusPoisonTimer > 0f) BonusPoisonTimer -= Time.deltaTime;

        if (transform.localRotation.z != 0f) transform.rotation = Quaternion.identity;

        currentState.Update();
    }

    protected void OnGUI()
    {

        if (Active)
        {
            // Draw bonuses if applicable
            GUI.matrix = GlobalGameStateManager.PrepareMatrix();
            GUI.skin = BonusSkin;

            if (BonusPoisonTimer > 0f)
            {
                string content = "Poisonous: " + Mathf.RoundToInt(BonusPoisonTimer) + " s";
                Vector2 size = BonusSkin.GetStyle("Label").CalcSize(new GUIContent(content));
                Vector2 size2 = BonusSkin.GetStyle("Label").CalcSize(new GUIContent("Bonus Speed"));

                GUI.Label(new Rect(20f, 1080f - size2.y - size.y - 20f, size.x, size.y), content);
            }

            if(BonusSpeedTimer > 0f)
            {
                string content = "Bonus Speed: " + Mathf.RoundToInt(BonusSpeedTimer) + " s";
                Vector2 size = BonusSkin.GetStyle("Label").CalcSize(new GUIContent(content));

                GUI.Label(new Rect(20f, 1080f - size.y - 20f, size.x, size.y), content);
            }

            GUI.matrix = Matrix4x4.identity;
        }
        
        currentState.OnGUI();
    }

    public void UpdateSorting()
    {        
        currentState.UpdateSorting();

        BodyParts.Shadow.GetComponent<SpriteRenderer>().sortingOrder = BodyParts.Legs.GetComponent<SpriteRenderer>().sortingOrder - 1;
    }

    public void ChangeState(string newState)
    {
        currentState.Leave();
        
        currentState = states[newState];

        currentState.Enter(null);
    }

    public void ChangeState(string newState, object data)
    {
        currentState.Leave();

        currentState = states[newState];

        currentState.Enter(data);
    }

    private void OnDestroy()
    {
        MessageCenter.Instance.UnregisterListener(MessageType.PlayerKilled, HandleDeath);
        MessageCenter.Instance.UnregisterListener(MessageType.AxeManMinigameTreeChangePhase, HandleChangePhase);
    }
}

namespace Tree.Private
{
    [Serializable]
    public class BodyParts
    {
        public GameObject Trunk, Shadow, Face, LeftArm, RightArm, LeftUpperArm, LeftLowerForegroundArm, LeftLowerBackgroundArm, RightUpperArm, RightLowerForegroundArm, RightLowerBackgroundArm, Legs, RightGrabbedNPC, EatenNPC, MinigameCircle, Eyes, FlameEyes, Axe;
    }
    
    [Serializable]
    public class Sprites
    {
        [Serializable]
        public class _Trunk
        {
            public Sprite Front, FrontRight, Right, BackRight, Back;
        }

        [Serializable]
        public class _Face
        {
            public Sprite None, EyesClosed, MoveFront, MoveFrontRight, MoveRight, Crazy, ChewedAxeMan;
        }

        [Serializable]
        public class _Arms
        {
            public Sprite Left, LeftUpper, LeftLowerForeground, LeftLowerBackground, Right, RightUpper, RightLowerForeground, RightLowerBackground;
        }

        [Serializable]
        public class _EatingMinigame
        {
            public Sprite[] Circle;
            public Sprite LS, RS;
            public Texture[] Buttons;
            public Sprite[] ButtonSprites;
        }


        public _Trunk Trunk;
        public _Face Face;
        public Sprite LegsStill;
        public _EatingMinigame EatingMinigame;
        public Sprite[] EatingAxeMan;
        public Texture2D FinishHim;
    }

    [Serializable]
    public class Sounds
    {
        public AudioClip Music, Chew, Burp, SoulConsumed, SoulConsumed2, AxeManMinigameMusic;
        public AudioClip[] Laugh;
        public AudioClip[] Saying;
        public AudioClip[] Walk;
        public AudioClip Groan, RemoveAxe;
        public AudioClip[] ChewAxeMan;
        public AudioClip[] AxeManLastWords;
    }

    [Serializable]
    public class Prefabs
    {
        public GameObject LT, ThumbStick, A, AxeManKillActive;
    }
}
