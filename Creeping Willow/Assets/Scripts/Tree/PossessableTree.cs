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
    public Tree.Private.BodyParts BodyParts;
    public Tree.Private.Sprites Sprites;
    public Tree.Private.Sounds Sounds;
    public Tree.Private.Prefabs Prefabs;


    private Dictionary<string, TreeState> states;
    private TreeState currentState;


    public readonly float MaxBonusTime = 30f;
    public float BonusSpeedTimer = 0f;
    public float BonusPoisonTimer = 0f;

    public GUISkin BonusSkin;


	private void CreateStates()
    {
        states = new Dictionary<string, TreeState>();
        
        TreeState inactive = new TreeStateInactive();
        TreeState active = new TreeStateActive();
        TreeState eatingMinigameWrangle = new TreeStateEatingMinigameWrangle();
        TreeState eatingMinigameMash = new TreeStateEatingMinigameMash();
        TreeState eatingMinigameMashInstant = new TreeStateEatingMinigameMashInstant();
        TreeState eating = new TreeStateEating();

        states.Add("Inactive", inactive);
        states.Add("Active", active);
        states.Add("EatingMinigameWrangle", eatingMinigameWrangle);
        states.Add("EatingMinigameMash", eatingMinigameMash);
        states.Add("EatingMinigameMashInstant", eatingMinigameMashInstant);
        states.Add("Eating", eating);

        foreach (TreeState state in states.Values) state.Tree = this;

        currentState = (Active) ? active : inactive;

        if (Active) MessageCenter.Instance.Broadcast(new CameraChangeFollowedMessage(transform, Vector3.zero));

        currentState.Enter(null);
    }

    private void LoadCircle()
    {
        Sprites.EatingMinigame.Circle = new Sprite[101];

        for (int i = 0; i < 101; i++) Sprites.EatingMinigame.Circle[i] = Resources.Load<Sprite>("Textures/CircularProgressBar/CircleProgress" + (i + 1));
    }

	protected override void Start ()
    {
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
}

namespace Tree.Private
{
    [Serializable]
    public class BodyParts
    {
        public GameObject Trunk, Face, LeftArm, RightArm, LeftUpperArm, LeftLowerForegroundArm, LeftLowerBackgroundArm, RightUpperArm, RightLowerForegroundArm, RightLowerBackgroundArm, Legs, RightGrabbedNPC, EatenNPC, MinigameCircle, Eyes, FlameEyes;
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
            public Sprite None, EyesClosed, MoveFront, MoveFrontRight, MoveRight, Crazy;
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
        }


        public _Trunk Trunk;
        public _Face Face;
        public Sprite LegsStill;
        public _EatingMinigame EatingMinigame;
    }

    [Serializable]
    public class Sounds
    {
        public AudioClip Music, Chew, SoulConsumed;
        public AudioClip[] Walk;
    }

    [Serializable]
    public class Prefabs
    {
        public GameObject LT, ThumbStick, A;
    }
}
