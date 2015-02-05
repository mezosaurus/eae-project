using System;
using System.Collections.Generic;
using UnityEngine;

public class PossessableTree : Possessable
{
    public enum Direction
    {
        Front, FrontRight, Right, BackRight, Back, BackLeft, Left, FrontLeft
    }
    

    public bool Active;
    public float Speed;
    public Tree.Private.BodyParts BodyParts;
    public Tree.Private.Sprites Sprites;
    public Tree.Private.Sounds Sounds;
    public Tree.Private.Prefabs Prefabs;


    private Dictionary<string, TreeState> states;
    private TreeState currentState;


	private void CreateStates()
    {
        states = new Dictionary<string, TreeState>();
        
        TreeState inactive = new TreeStateInactive();
        TreeState active = new TreeStateActive();
        TreeState eatingMinigameWrangle = new TreeStateEatingMinigameWrangle();
        TreeState eatingMinigameMash = new TreeStateEatingMinigameMash();
        TreeState eating = new TreeStateEating();

        states.Add("Inactive", inactive);
        states.Add("Active", active);
        states.Add("EatingMinigameWrangle", eatingMinigameWrangle);
        states.Add("EatingMinigameMash", eatingMinigameMash);
        states.Add("Eating", eating);

        foreach (TreeState state in states.Values) state.Tree = this;

        currentState = (Active) ? active : inactive;

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
	
	protected override void lure(){
	}

    protected override void scare()
    {
        
    }

    protected override void GameUpdate()
    {
        base.GameUpdate();

        if (transform.localRotation.z != 0f) transform.rotation = Quaternion.identity;

        currentState.Update();
    }

    protected void OnGUI()
    {
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
        public GameObject Trunk, Face, LeftArm, RightArm, LeftUpperArm, LeftLowerForegroundArm, LeftLowerBackgroundArm, RightUpperArm, RightLowerForegroundArm, RightLowerBackgroundArm, Legs, RightGrabbedNPC, EatenNPC, MinigameCircle, Eyes;
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
        public AudioClip Chew;
    }

    [Serializable]
    public class Prefabs
    {
        public GameObject LT, ThumbStick;
    }
}
