﻿using UnityEngine;

public abstract class TreeState
{
    public PossessableTree Tree;

    
    public virtual void Enter() { }
    public virtual void OnTriggerEnter(Collider2D collider) { }
    public virtual void OnTriggerExit(Collider2D collider) { }
    public virtual void Update() { }
    public virtual void UpdateSorting() { }
    public virtual void OnGUI() { }
    public virtual void Leave() { }
}
