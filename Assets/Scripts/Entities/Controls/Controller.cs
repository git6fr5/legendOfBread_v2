/* --- Libraries --- */
using System.Collections;
using System.Collections.Generic;
using UnityEngine;

/* --- Enumerations --- */
using Orientation = Compass.ORIENTATION;
using Movement = State.Movement;
using Action = State.Action;
using Vitality = State.Vitality;

/// <summary>
/// Controls the behaviour of an entity.
/// </summary>
[RequireComponent(typeof(Rigidbody2D))]
[RequireComponent(typeof(State))]
public class Controller : MonoBehaviour {
    
    /* --- Components --- */
    [HideInInspector] public State state;
    [HideInInspector] public Rigidbody2D body;
    [SerializeField] public Mesh mesh;

    /* --- Variables --- */
    [SerializeField] public int id; // The id of this entity (used to distinguish between types of entities in a particular class).
    [SerializeField] public Vector2 origin; // The initial position of the entity (usually associated with spawn position).

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
        state.movement = Move();
        state.orientation = Turn();
    }

    /* --- Thinking Actions --- */
    // Sets the action controls
    protected virtual void Think() {
        // Determined by the particular type of controller.
    }

    // Adjusts the velocity of this entity with respect to internal and external movement controls.
    Movement Move() {
        int movement = 0;
        momentumVector *= (1f - GameRules.dynamicFriction);
        if (movementVector != Vector2.zero) {
            movement++;
        }
        if (state.carryingStructure != null) {
            moveSpeed = moveSpeed * state.carryingStructure.carryWeight;
            movement = movement + 2;
        }
        body.velocity = moveSpeed * movementVector.normalized + momentumVector;

        return (Movement)movement;
    }

    // Adjusts the internal orientation enumerator of the entity.
    Orientation Turn() {
        if (orientationVector != Vector2.zero) {
            return Compass.VectorOrientations[orientationVector]; // Compass.SnapVectorToOrientation(orientationVector);
        }
        return Orientation.UP;
    }

    /* --- Internal Event Actions --- */
    // Activates the attack of this controller.
    protected void Action(int index = 0) {
        if (index < state.equipment.Count) {
            state.equipment[index].Activate(this);
        }
        OnAction(index);
    }

    protected virtual void OnAction(int index = 0) {
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
    }

    protected virtual void OnHit(Hurtbox hurtbox) {
        // Determined by the particular type of controller.
    }

    // Damages the state by the given damage.
    public void Hurt(int damage) {
        if (state.vitality == Vitality.Healthy) {
            OnHurt();
            state.health -= damage;
            state.vitality = Vitality.Hurt;
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
        state.vitality = Vitality.Dead;
        enabled = false;
    }

    protected virtual void OnDeath() {
        // Determined by the particular type of controller.
    }

}
