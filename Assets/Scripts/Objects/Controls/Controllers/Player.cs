using System.Collections;
using System.Collections.Generic;
using UnityEngine;

public class Player : Controller {

    /* --- Variables --- */
    // Player-Specific Components
    public Weapon weapon;
    public Transform overhead;

    // Movement Keys
    public KeyCode runKey = KeyCode.Z;
    public Dictionary<KeyCode, Vector2> movementKeys = new Dictionary<KeyCode, Vector2>() {
        { KeyCode.W, Vector2.up },
        { KeyCode.A, -Vector2.right },
        { KeyCode.S, -Vector2.up },
        { KeyCode.D, Vector2.right }
    };
    List<KeyCode> pressedKeys = new List<KeyCode>();

    // Attack Keys
    public KeyCode attackKey = KeyCode.J;

    // Player-Specific Action Controls
    [Range(0.05f, 1f)] public float attackMoveFactor = 0.25f;
    [Range(0.05f, 1f)] public float carryMoveFactor = 0.5f;
    [Range(0.05f, 2f)] public float runMoveFactor = 1.25f;

    // Interactions
    public KeyCode interactKey = KeyCode.Space;
    public KeyCode jumpKey = KeyCode.M;
    public Dialogue dialogue;
    public Throwable throwable;

    /* --- Override --- */
    // Sets the action controls.
    protected override void Think() {
        AttackInput();
        MoveInput();
        InteractInput();
        JumpInput();
    }

    /* --- Thinking Actions --- */
    // Activate an attack on input.
    void AttackInput() {
        if (Input.GetKeyDown(attackKey) && !state.isAttacking) {
            Attack();
        }
    }

    // Get the movement input.
    void MoveInput() {

        // Reset the controls.
        movementVector = Vector2.zero;
        moveSpeed = state.baseSpeed;

        // Factor for any move speed modifiers.
        if (state.isAttacking) { moveSpeed *= attackMoveFactor; }
        else if (Input.GetKey(runKey)) { moveSpeed *= runMoveFactor; }

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

        // Adjust the orientation if a key is pressed
        if (!state.isAttacking && !state.isThrowing && pressedKeys.Count > 0) {
            orientationVector = movementKeys[pressedKeys[0]];
        }
    }

    /* --- Event Actions --- */
    // Activate the weapon
    protected override void OnAttack() {
        weapon.Activate(true);
    }

    // When hitting something through an attack
    protected override void OnHit(Hurtbox hurtbox) {
        weapon.OnHit(hurtbox);
    }

    /* --- Interactions --- */
    void InteractInput() {
        if (state.isTalking) {
            Talking();
        }
        else if (state.isCarrying) {
            Carrying();
        }
    }

    public void Talk(string npcName) {
        dialogue.talkDelay = dialogue.regularTalkDelay;
        dialogue.Run(npcName);
        state.isTalking = true;
    }

    void Talking() {
        movementVector = Vector2.zero;
        if (Input.GetKeyDown(interactKey) && dialogue.isPrintingDialogue) {
            dialogue.talkDelay = dialogue.fastTalkDelay;
        }
        if (Input.GetKeyDown(interactKey) && !dialogue.isRunningCommand) {
            dialogue.Clear();
            dialogue.gameObject.SetActive(false);
            state.isTalking = false;
        }
    }

    public void Carry(Throwable _throwable) {
        throwable = _throwable;
        // Set the position of the object over the player's head.
        throwable.transform.parent = overhead;
        throwable.transform.localPosition = Vector3.zero;
        foreach (Transform child in transform) {
            if (child.tag == GameRules.meshTag) {
                throwable.mesh.hull.position = child.GetComponent<Mesh>().hull.position + GameRules.movementPrecision * -Vector3.up;
            }
        }
        throwable.mesh.GetComponent<Collider2D>().enabled = false;
        state.isCarrying = true;
    }

    void Carrying() {
        //
        moveSpeed = state.baseSpeed * carryMoveFactor;
        if (Input.GetKeyDown(interactKey) && throwable != null && throwable.isCarried) {
            Throw();
        }
    }

    void Throw() {
        throwable.transform.parent = null;
        state.isCarrying = false;
        state.isThrowing = true;
        throwable.Throw(state.orientation, transform.position);
    }


    public void Push(Pushable pushable) {
        pushable.Push(state.orientation, transform.position);
        state.isThrowing = true;
    }

    void JumpInput() {
        if (Input.GetKeyDown(jumpKey) && !state.isJumping && !state.isCarrying && !state.isAttacking) {
            state.isJumping = true;
            fieldPulse += state.jumpPulse; 
        }
        else if (state.isJumping) {
            //
        }
    }

}

