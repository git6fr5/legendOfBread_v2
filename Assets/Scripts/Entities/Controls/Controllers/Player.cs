using System.Collections;
using System.Collections.Generic;
using UnityEngine;

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

    // Player-Specific Action Controls
    [Range(0.05f, 1f)] public float actionMoveFactor = 0.25f;
    [Range(0.05f, 2f)] public float runMoveFactor = 1.25f;

    // Interactions
    public KeyCode interactKey = KeyCode.Space;
    public KeyCode jumpKey = KeyCode.M;
    public Dialogue dialogue;
    public Throwable throwable;

    /* --- Override --- */
    // Sets the action controls.
    protected override void Think() {
        ActionInput();
        MoveInput();
    }

    /* --- Thinking Actions --- */
    // Activate an action on input if the state is not active already.
    void ActionInput() {
        if (state.activeItem == null) {
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
            // moveSpeed *= state.activeItem.moveSpeedFactor;
        }

    }

    /* --- Event Actions --- */
    // Activate the weapon
    protected override void OnAction(int index) {
        state.activeItem = state.equipment[index];
    }

    // When hitting something through an attack
    protected override void OnHit(Hurtbox hurtbox) {
        GameRules.CameraShake();
    }

    protected override void OnHurt() {
        GameRules.CameraShake();
    }

    // Need to clean below.
    /* --- Interactions --- */
    public void Talk(string npcName) {
        dialogue.talkDelay = dialogue.regularTalkDelay;
        dialogue.Run(npcName);
        // state.isTalking = true;
    }

    void Talking() {
        movementVector = Vector2.zero;
        if (Input.GetKeyDown(interactKey) && dialogue.isPrintingDialogue) {
            dialogue.talkDelay = dialogue.fastTalkDelay;
        }
        if (Input.GetKeyUp(interactKey) && !dialogue.isRunningCommand) {
            dialogue.Clear();
            dialogue.gameObject.SetActive(false);
            // state.isTalking = false;
        }
    }

}

