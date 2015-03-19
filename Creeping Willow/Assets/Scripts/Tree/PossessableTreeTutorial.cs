using System;
using System.Collections.Generic;
using UnityEngine;

public class PossessableTreeTutorial : PossessableTree
{
    protected override void CreateStates()
    {
        states = new Dictionary<string, TreeState>();

        TreeState inactive = new TreeStateInactiveTutorial();
        TreeState active = new TreeStateActiveTutorial();
        TreeState eatingMinigameWrangle = new TreeStateEatingMinigameWrangleTutorial();
        TreeState eatingMinigameMash = new TreeStateEatingMinigameMashTutorial();
        TreeState eating = new TreeStateEatingTutorial();

        states.Add("Inactive", inactive);
        states.Add("Active", active);
        states.Add("EatingMinigameWrangle", eatingMinigameWrangle);
        states.Add("EatingMinigameMash", eatingMinigameMash);
        states.Add("Eating", eating);

        foreach (TreeState state in states.Values) state.Tree = this;

        currentState = (Active) ? states["Active"] : states["Inactive"];

        if (Active) MessageCenter.Instance.Broadcast(new CameraChangeFollowedMessage(transform, Vector3.zero));

        currentState.Enter(null);
    }
	
	public override void possess()
    {
        if (TutorialManager.Instance.Phase == 15 && GetInstanceID() == TutorialManager.Instance.TreeA.GetInstanceID()) TutorialManager.Instance.DecrementPhase();
        if (TutorialManager.Instance.Phase == 15 && GetInstanceID() == TutorialManager.Instance.TreeB.GetInstanceID()) TutorialManager.Instance.AdvancePhase();
        if (TutorialManager.Instance.Phase == 3 || TutorialManager.Instance.Phase == 9) TutorialManager.Instance.DecrementPhase();
        
        base.possess ();
	}

    public override void exorcise()
    {
        //if (TutorialManager.Instance.Phase == 16 && GetInstanceID() == TutorialManager.Instance.TreeB.GetInstanceID()) TutorialManager.Instance.DecrementPhase();
        if (TutorialManager.Instance.Phase == 2 || TutorialManager.Instance.Phase == 8 || TutorialManager.Instance.Phase == 14)
            TutorialManager.Instance.AdvancePhase();
        
        base.exorcise ();
    }
}
