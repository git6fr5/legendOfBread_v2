using System.Collections;
using System.Collections.Generic;
using UnityEngine;

public class Player : Controller
{
    /* --- Variables --- */
    // Player-Specific Components
    public Weapon weapon;

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
    [Range(0.05f, 2f)] public float runMoveFactor = 1.25f;

    /* --- Override --- */
    // Sets the action controls.
    protected override void Think() {
        AttackInput();
        MoveInput();
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
        if (!state.isAttacking && pressedKeys.Count > 0) {
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

}
