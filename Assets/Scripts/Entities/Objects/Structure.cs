﻿using System.Collections;
using System.Collections.Generic;
using UnityEngine;

using Action = State.Action;

public class Structure : MonoBehaviour {

    /* --- Enumerations --- */
    // Tracks the condition of this structure.
    public enum Condition {
        Interactable,
        Interacting,
        Uninteractable
    }

    public Condition condition;
    public Action interactAction;
    public Mesh mesh;

    void Update() {
        // Check if this is currently being interacted with.
        switch (condition) {
            case (Condition.Interacting):
                Interacting();
                return;
            case (Condition.Interactable):
                Interactable();
                return;
            case (Condition.Uninteractable):
                Uninteractable();
                return;
            default:
                return;
        }
    }

    public virtual bool Interact(Controller controller) {
        return false;
    }

    protected virtual void Interactable() {
        //
    }

    protected virtual void Interacting() {
        //
    }

    protected virtual void Uninteractable() {
        //
    }

}
