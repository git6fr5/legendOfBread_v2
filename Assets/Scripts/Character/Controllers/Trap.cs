using System.Collections;
using System.Collections.Generic;
using UnityEngine;

public class Trap : Controller {

    /* --- Variables --- */
    // Controls the decision flow of the trap.
    public enum BUTTON {
        OFF,
        ON,
        count
    }
    public BUTTON button = BUTTON.OFF;

    /* --- Override --- */
    // Sets the action controls.
    protected override void Think() {

        // Reset the movement.
        movementVector = Vector2.zero;
        moveSpeed = state.baseSpeed;

        switch (button) {
            case (BUTTON.OFF):
                Off();
                break;
            case (BUTTON.ON):
                On();
                break;
            default:
                break;
        }
    }

    /* --- Action Flow --- */
    // The action logic while this trap is off.
    protected virtual void Off() {
        // Determined by the particular type of trap.
    }

    // The action logic while this trap is on.
    protected virtual void On() {
        // Determined by the particular type of trap.
    }

}
