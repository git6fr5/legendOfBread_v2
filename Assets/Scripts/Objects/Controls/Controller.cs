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
    [SerializeField] public Vector2 origin;
    [SerializeField] static float friction = 0.025f;
    [SerializeField] static float field = -5f;
    [SerializeField] static float fieldPlane = 0f;

    // Action Controls
    [SerializeField] protected Vector2 movementVector;
    [SerializeField] protected Vector2 momentumVector;
    [SerializeField] protected float fieldPulse;
    [SerializeField] protected Vector2 orientationVector;
    [SerializeField] protected float moveSpeed;
    [SerializeField] protected bool activateAttack;

    /* --- Unity --- */
    // Runs once on compilation.
    void Awake() {
        origin = transform.position;
        state = GetComponent<State>();
        body = GetComponent<Rigidbody2D>();
        body.constraints = RigidbodyConstraints2D.FreezeRotation;
        body.gravityScale = 0f;
        body.angularDrag = 0f;
    }

    // Runs every frame.
    void Update() {
        Think();
    }

    // Runs every fixed interval
    void FixedUpdate() {
        state.isMoving = Move();
        Fall();
        state.orientation = Orientation();
    }

    /* --- Thinking Actions --- */
    // Sets the action controls
    protected virtual void Think() {
        // Determined by the particular type of controller.
    }

    // Moves the transform in the direction of the movement vector, at the move speed.
    bool Move() {
        momentumVector *= (1f - friction);
        body.velocity = moveSpeed * movementVector.normalized + momentumVector ;
        if (movementVector != Vector2.zero) {
            return true; 
        }
        return false;
    }

    void Fall() {
        fieldPulse = fieldPulse + Time.fixedDeltaTime * field;
        // state.height = state.height + Mathf.Sign(fieldPulse) * Mathf.Pow(fieldPulse, 2) * Time.fixedDeltaTime;
        state.height = state.height + Mathf.Pow(fieldPulse, 3) * Time.fixedDeltaTime;
        if (state.height <= fieldPlane) {
            fieldPulse = Mathf.Max(0f, fieldPulse);
            state.height = fieldPlane;
            state.isJumping = false;
        }
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
    public void Hurt(int damage) {
        if (!state.isHurt) {
            OnHurt();
            state.health -= damage;
            state.isHurt = true;
            if (state.health <= 0) {
                Death();
            }
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

    protected virtual void Jump() {
        // Determined by the particular type of controller.
    }

}
