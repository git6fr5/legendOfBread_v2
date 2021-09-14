using System.Collections;
using System.Collections.Generic;
using UnityEngine;

using Action = State.Action;
using Movement = State.Movement;

public class Human : Creature {

    /* --- Additional Animations --- */
    public Sprite[] walk;
    public Sprite[] carryIdle;
    public Sprite[] carryWalk;
    public Sprite[] attack;
    public Sprite[] push;

    protected override void AddAnimations() {
        // Action Animations.
        actionAnimations.Add(Action.Attacking, (attack, true));
        actionAnimations.Add(Action.Pushing, (push, true));
        actionAnimations.Add(Action.Throwing, (push, true));
        // Movement Animations.
        movementAnimations.Add(Movement.Idle, (idle, true));
        movementAnimations.Add(Movement.Moving, (walk, true));
        movementAnimations.Add(Movement.CarryingIdle, (carryIdle, true));
        movementAnimations.Add(Movement.CarryingMoving, (carryWalk, true));
        movementAnimations.Add(Movement.Talking, (walk, true));

    }

}
