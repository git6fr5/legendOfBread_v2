using System.Collections;
using System.Collections.Generic;
using UnityEngine;

using Action = State.Action;
using Movement = State.Movement;

public class Human : Creature {

    /* --- Additional Animations --- */
    public Sprite[] walk;
    public Sprite[] attack;

    protected override void AddAnimations() {
        // Action Animations.
        actionAnimations.Add(Action.Attacking, (attack, true));
        // Movement Animations.
        movementAnimations.Add(Movement.Idle, (idle, true));
        movementAnimations.Add(Movement.Moving, (walk, true));
    }

}
