/* --- Libraries --- */
using System.Collections;
using System.Collections.Generic;
using UnityEngine;

/* --- Enumerations --- */
using ORIENTATION = Compass.ORIENTATION;

/// <summary>
/// Controls the behaviour of an entity.
/// </summary>
[RequireComponent(typeof(Rigidbody2D))]
[RequireComponent(typeof(State))]
public class Controller : MonoBehaviour {
    
    /* --- Components --- */
    [HideInInspector] public State state;
    [HideInInspector] public Rigidbody2D body;

    /* --- Variables --- */
    [SerializeField] public int id; // The id of this entity (used to distinguish between types of entities in a particular class).
    [SerializeField] public Vector2 origin; // The initial position of the entity (usually associated with spawn position).
    [SerializeField] static float fieldPlane = 0f; // The level above which the field applies.

    // Action Controls
    [SerializeField] protected Vector2 movementVector; // The internal movement control.
    [SerializeField] protected float moveSpeed; // The internal speed at which this entity is moving.
    [SerializeField] protected Vector2 momentumVector; // The external movement control.
    [SerializeField] protected Vector2 orientationVector; // The direction this entity is facing.
    [SerializeField] protected float fieldPulse; // The incremental field value (per frame).

    /* --- Unity --- */
    // Runs once on instantiation.
    void Awake() {
        // Cache these references.
        origin = transform.position;
        state = GetComponent<State>();
        body = GetComponent<Rigidbody2D>();
        // Set up the attached components.
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
        state.isFalling = Fall();
        state.orientation = Turn();
    }

    /* --- Thinking Actions --- */
    // Sets the action controls
    protected virtual void Think() {
        // Determined by the particular type of controller.
    }

    // Adjusts the velocity of this entity with respect to internal and external movement controls.
    bool Move() {
        momentumVector *= (1f - GameRules.dynamicFriction);
        if (state.isSliding) {
            body.velocity = moveSpeed * (body.velocity * (1f - GameRules.dynamicFriction) + movementVector).normalized + momentumVector;
        }
        else {
            body.velocity = moveSpeed * movementVector.normalized + momentumVector;
        }
        if (movementVector != Vector2.zero) {
            return true; 
        }
        return false;
    }

    // Adjusts the height of this state based on the field and any internal impulses.
    bool Fall() {
        fieldPulse = fieldPulse + Time.fixedDeltaTime * -GameRules.gravityScale;
        // state.height = state.height + Mathf.Sign(fieldPulse) * Mathf.Pow(fieldPulse, 2) * Time.fixedDeltaTime;
        state.height = state.height + Mathf.Pow(fieldPulse, 3) * Time.fixedDeltaTime;
        if (state.height <= fieldPlane) {
            fieldPulse = Mathf.Max(0f, fieldPulse);
            state.height = fieldPlane;
            if (state.isJumping) {
                OnLand();
            }
            return false;
        }
        return true;
    }

    // Adjusts the internal orientation enumerator of the entity.
    ORIENTATION Turn() {
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

    protected void Jump() {
        OnJump();
        state.isJumping = true;
    }

    protected virtual void OnJump() {
        // Determined by the particular type of controller.
    }

    protected virtual void OnLand() {
        // Determined by the particular type of controller.
    }

    /* --- External Event Actions --- */
    public void Push(Vector2 momentum, bool reset = false) {
        if (reset) {
            momentumVector = Vector2.zero;
        }
        momentumVector += momentum;
    }

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

}
