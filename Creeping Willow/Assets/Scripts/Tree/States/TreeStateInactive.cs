﻿using UnityEngine;

public class TreeStateInactive : TreeState
{
    public override void Enter()
    {
        Tree.Active = false;
        Tree.BodyParts.Face.GetComponent<SpriteRenderer>().sprite = Tree.Sprites.Face.None;
        Debug.Log("Never should be called...");
    }
}
