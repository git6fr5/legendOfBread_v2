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
    public Dialogue dialogue;
    public Carriable carried;

    /* --- Override --- */
    // Sets the action controls.
    protected override void Think() {
        AttackInput();
        MoveInput();
        InteractInput();
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

        // Reset the movement.
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

    public void Carry(Carriable carriable) {
        carriable.transform.parent = overhead;
        carriable.transform.localPosition = Vector3.zero;
        carriable.GetComponent<Rigidbody2D>().mass = 0f;
        carriable.GetComponent<Rigidbody2D>().isKinematic = true;
        foreach (Transform child in carriable.transform) {
            if (child.tag == GameRules.meshTag) {
                child.GetComponent<Mesh>().hull.localPosition = new Vector3(0, -1.5f, 0);
                child.GetComponent<Collider2D>().enabled = false;
            }
        }
        carried = carriable;
        state.isCarrying = true;
    }

    void Carrying() {
        //
        moveSpeed = state.baseSpeed * carryMoveFactor;
        if (Input.GetKeyDown(interactKey) && carried != null && carried.isThrowable) {
            Throw();
        }
    }

    void Throw() {
        carried.transform.parent = null;
        state.isCarrying = false;
        state.isThrowing = true;
        carried.Throw(state.orientation, transform.position);
    }

}
