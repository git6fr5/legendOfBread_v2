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
        if (attack != null && attack.Length > 0) { 
            actionAnimations.Add(Action.Attacking, (attack, true)); 
        }
        if (push != null && push.Length > 0) {
            actionAnimations.Add(Action.Pushing, (push, true));
        }
        if (push != null && push.Length > 0) {
            actionAnimations.Add(Action.Throwing, (push, true));
        }
        // Movement Animations.
        if (idle != null && idle.Length > 0) {
            movementAnimations.Add(Movement.Idle, (idle, true));
        }
        if (walk != null && walk.Length > 0) {
            movementAnimations.Add(Movement.Moving, (walk, true));
        }
        if (carryIdle != null && carryIdle.Length > 0) {
            movementAnimations.Add(Movement.CarryingIdle, (carryIdle, true));
        }
        if (carryWalk != null && carryWalk.Length > 0) {
            movementAnimations.Add(Movement.CarryingMoving, (carryWalk, true));
        }
        if (idle != null && idle.Length > 0) {
            movementAnimations.Add(Movement.Talking, (idle, true));
        }
    }

}
