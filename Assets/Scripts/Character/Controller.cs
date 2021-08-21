using System.Collections;
using System.Collections.Generic;
using UnityEngine;
using ORIENTATION = Compass.ORIENTATION;

[RequireComponent(typeof(Rigidbody2D))]
[RequireComponent(typeof(State))]
public class Controller : MonoBehaviour {
    
    /* --- Components --- */
    [HideInInspector] public State state;
    protected Rigidbody2D body;

    /* --- Variables --- */
    [SerializeField] public int id;

    // Action Controls
    [SerializeField] protected Vector2 movementVector;
    [SerializeField] protected Vector2 orientationVector;
    [SerializeField] protected float moveSpeed;
    [SerializeField] protected bool activateAttack;


    /* --- Unity --- */
    // Runs once on compilation.
    void Awake() {
        state = GetComponent<State>();
        body = GetComponent<Rigidbody2D>();
        body.constraints = RigidbodyConstraints2D.FreezeRotation;
        body.gravityScale = 0f;
    }

    // Runs every frame.
    void Update() {
        Think();
    }

    // Runs every fixed interval
    void FixedUpdate() {
        state.isMoving = Move();
        state.orientation = Orientation();
    }

    /* --- Thinking Actions --- */
    // Sets the action controls
    protected virtual void Think() {
        // Determined by the particular type of controller.
    }

    // Moves the transform in the direction of the movement vector, at the move speed.
    bool Move() {
        if (movementVector != Vector2.zero) {
            Vector2 deltaPosition = movementVector.normalized * moveSpeed * Time.fixedDeltaTime;
            state.transform.position = state.transform.position + (Vector3)deltaPosition;
            return true;
        }
        return false;
    }

    ORIENTATION Orientation() {
        if (orientationVector != Vector2.zero) {
            return Compass.VectorOrientations[orientationVector]; // Compass.SnapVectorToOrientation(orientationVector);
        }
        return ORIENTATION.UP;
    }

    /* --- Internal Event Actions --- */
    // Activates the attack of this controller.
    protected void Attack() {
        OnAttack();
        state.isAttacking = true;
    }

    protected virtual void OnAttack() {
        // Determined by the particular type of controller.
    }

    /* --- External Event Actions --- */
    // If we hit something, then perform the hit actions.
    public void Hit(Hurtbox hurtbox) {
        OnHit(hurtbox);
        // state.hitSomething?
    }

    protected virtual void OnHit(Hurtbox hurtbox) {
        // Determined by the particular type of controller.
    }

    // Damages the state by the given damage.
    public void Hurt(double damage) {
        OnHurt();
        state.health -= damage;
        state.isHurt = true;
        if (state.health <= 0) {
            Death();
        }
    }

    protected virtual void OnHurt() {
        // Determined by the particular type of controller.
    }

    // Deactivates the controller and sets the state to dead.
    public void Death() {
        OnDeath();
        state.isDead = true;
        enabled = false;
    }

    protected virtual void OnDeath() {
        // Determined by the particular type of controller.
    }

}
