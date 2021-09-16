using System.Collections;
using System.Collections.Generic;
using UnityEngine;

using StateAction = State.Action;

public class Player : Controller {

    /* --- Variables --- */
    // Movement Keys
    public Dictionary<KeyCode, Vector2> movementKeys = new Dictionary<KeyCode, Vector2>() {
        { KeyCode.W, Vector2.up },
        { KeyCode.A, -Vector2.right },
        { KeyCode.S, -Vector2.up },
        { KeyCode.D, Vector2.right }
    };
    List<KeyCode> pressedKeys = new List<KeyCode>();

    // Action Keys
    public KeyCode[] actionKeys = new KeyCode[] { KeyCode.J, KeyCode.K, KeyCode.L };

    // Interact Key
    public Interactbox interactbox;
    public KeyCode interactKey = KeyCode.Space;
    public Equipable[] internalEquipment;

    // Player-Specific Action Controls
    [Range(0.05f, 1f)] public float actionMoveFactor = 0.25f;
    [Range(0.05f, 2f)] public float runMoveFactor = 1.25f;

    //// Interactions
    //public Dialogue dialogue;
    //public Throwable throwable;

    /* --- Override --- */
    // Sets the action controls.
    protected override void Think() {
        ActionInput();
        MoveInput();
        InteractInput();
    }

    /* --- Thinking Actions --- */
    // Activate an action on input if the state is not active already.
    void ActionInput() {
        if (state.activeItem == null && state.carryingStructure == null) {
            for (int i = 0; i < actionKeys.Length; i++) {
                if (Input.GetKeyDown(actionKeys[i]) && state.equipment.Count > i && state.equipment[i] != null) {
                    Action(i);
                }
            }
        }
    }

    // Get the movement input.
    void MoveInput() {

        // Reset the controls.
        movementVector = Vector2.zero;
        moveSpeed = state.baseSpeed;

        // Itterate through the movement keys.
        foreach (KeyValuePair<KeyCode, Vector2> movement in movementKeys) {
            // Get the active keys.
            if (Input.GetKey(movement.Key)) {
                movementVector += movement.Value;
                if (!pressedKeys.Contains(movement.Key)) {
                    pressedKeys.Add(movement.Key);
                }
            }
            else if (pressedKeys.Contains(movement.Key)) {
                pressedKeys.Remove(movement.Key);
            }
        }

        // Adjust the orientation if a key is pressed and no items are active.
        if (state.activeItem == null && pressedKeys.Count > 0) {
            orientationVector = movementKeys[pressedKeys[0]];
        }
        else if (state.activeItem) {
            moveSpeed *= state.activeItem.moveSpeed;
        }

    }

    void InteractInput() {
        if (Input.GetKeyDown(interactKey)) {
            for (int i = 0; i < internalEquipment.Length; i++) {
                if (state.action != StateAction.Inactive) { break; }
                internalEquipment[i].Activate(this);
            }
        }
    }

    /* --- Event Actions --- */
    // Activate the weapon
    protected override void OnAction(int index) {

    }

    // When hitting something through an attack
    protected override void OnHit(Hurtbox hurtbox) {
        hurtbox.controller.Hurt((int)state.activeItem?.damage);
        GameRules.CameraShake();
    }

    protected override void OnHurt() {
        GameRules.CameraShake();
    }

}

