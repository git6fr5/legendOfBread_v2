using System.Collections;
using System.Collections.Generic;
using UnityEngine;

public class Mob : Controller {

    /* --- Variables --- */
    // Controls the decision flow of the mob.
    public enum MINDSET {
        IDLE,
        ANGRY,
        count
    }
    protected MINDSET mindset;

    // The loot that this mob will drop on death.
    public Item[] loot;

    // The vision of this mob.
    public Vision vision;

    // Mob-Specific Action Controls
    [Range(0.05f, 1f)] public float idleFactor = 1f;
    [Range(0.05f, 2f)] public float aggressiveFactor = 1.25f;

    /* --- Override --- */
    // Sets the action controls.
    protected override void Think() {

        // Reset the movement.
        movementVector = Vector2.zero;
        moveSpeed = state.baseSpeed;

        switch (mindset) {
            case (MINDSET.IDLE):
                Idle();
                break;
            case (MINDSET.ANGRY):
                Angry();
                break;
            default:
                break;
        }
    }

    /* --- Action Flow --- */
    // The action logic while this mob is idle.
    protected virtual void Idle() {
        // Determined by the particular type of mob.
    }

    // The action logic while this mob is angry.
    protected virtual void Angry() {
        // Determined by the particular type of mob.
    }

}
