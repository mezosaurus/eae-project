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
    public Tree.Private.Prefabs Prefabs;


    private Dictionary<string, TreeState> states;
    private TreeState currentState;


	private void CreateStates()
    {
        states = new Dictionary<string, TreeState>();
        
        TreeState inactive = new TreeStateInactive();
        TreeState active = new TreeStateActive();
        TreeState eatingMinigameWrangle = new TreeStateEatingMinigameWrangle();

        states.Add("Inactive", inactive);
        states.Add("Active", active);
        states.Add("EatingMinigameWrangle", eatingMinigameWrangle);

        foreach (TreeState state in states.Values) state.Tree = this;

        currentState = (Active) ? active : inactive;

        currentState.Enter();
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

    protected override void act()
    {
        
    }

    protected override void GameUpdate()
    {
        base.GameUpdate();

        if (transform.localRotation.z != 0f) transform.rotation = Quaternion.identity;

        currentState.Update();
    }

    public void UpdateSorting()
    {
        currentState.UpdateSorting();
    }

    public void ChangeState(string newState)
    {
        currentState.Leave();
        
        currentState = states[newState];

        currentState.Enter();
    }
}

namespace Tree.Private
{
    [Serializable]
    public class BodyParts
    {
        public GameObject Trunk, Face, LeftArm, RightArm, LeftUpperArm, LeftLowerForegroundArm, LeftLowerBackgroundArm, RightUpperArm, RightLowerForegroundArm, RightLowerBackgroundArm, Legs, MinigameCircle;
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
        }


        public _Trunk Trunk;
        public _Face Face;
        public Sprite LegsStill;
        public _EatingMinigame EatingMinigame;
    }

    [Serializable]
    public class Prefabs
    {
        public GameObject ThumbStick;
    }
}
