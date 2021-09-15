using System.Collections;
using System.Collections.Generic;
using UnityEngine;

using ORIENTATION = Compass.ORIENTATION;
using Action = State.Action;

[RequireComponent(typeof(Rigidbody2D))]
public class Throwable : Structure {

    /* --- Components --- */
    [HideInInspector] public Rigidbody2D body;

    /* --- Variables --- */
    [SerializeField] [Range(2f, 10f)] protected float throwDistance = 1f; // The distance this can be thrown.
    [SerializeField] [Range(1f, 10f)] protected float throwSpeed = 5f; // The speed at which this is thrown.
    [SerializeField] [Range(0.05f, 1f)] public float carryWeight = 0.65f;
    // Internal Throwing Mechanics.
    [HideInInspector] protected Vector2 targetPosition; // The position this object is interpolating towards.
    [HideInInspector] protected float collisionBuffer = 0.25f; // The buffer time between which this is thrown and collisions are reenabled.
    [HideInInspector] protected float groundHeight; // The distance between the hull and the mesh when it is on the floor.
    [HideInInspector] protected float carryHeight; // The distance between the hull and the mesh when it is being carried.

    /* --- Unity --- */
    // Runs once on instantiation.
    void Awake() {
        // Cache these references.
        body = GetComponent<Rigidbody2D>();
        // Set up the attached components.
        body.constraints = RigidbodyConstraints2D.FreezeAll;
        body.gravityScale = 0f;
        body.angularDrag = 0f;
        // Set these variables
        groundHeight = transform.position.y - mesh.hull.position.y;
    }

    /* --- Overridden Methods --- */
    // Picks the structure up.
    public override bool Interact(Controller controller) {
        if (condition == Condition.Interactable) {
            return Carry(controller);
        }
        return false;
    }

    // Makes the structure fall towards its hull.
    protected override void Uninteractable() {
        float fallspeed = carryHeight / (throwDistance / throwSpeed);
        body.constraints = RigidbodyConstraints2D.FreezeRotation;

        if ((mesh.transform.position - mesh.hull.position).y > groundHeight) {
            mesh.transform.position -= fallspeed * Vector3.up * Time.deltaTime;
        }
        else {
            body.velocity *= 0.95f;
        }

        if (body.velocity.magnitude < GameRules.movementPrecision) {
            condition = Condition.Interactable;
            body.velocity = Vector3.zero;
            body.constraints = RigidbodyConstraints2D.FreezeAll;
        }

    }

    /* --- Action Methods --- */
    bool Carry(Controller controller) {
        OnCarry();

        // Set the hull under the main object.
        mesh.hull.parent = transform; mesh.hull.localPosition = new Vector3(0f, -GameRules.movementPrecision, 0f);
        // Set the main object under the controller's hull.
        transform.parent = controller.mesh.hull; transform.localPosition = Vector3.zero;
        // Set the position of the object over the player's head.
        mesh.transform.position = controller.mesh.overhead.position;
        // Set the internal height.
        carryHeight = mesh.transform.position.y - (mesh.hull.position.y - groundHeight);

        // Disable the rigidbody.
        body.simulated = false;
        body.constraints = RigidbodyConstraints2D.FreezeRotation;

        // Disable the collision frame.
        mesh.frame.enabled = false;

        // Inform the controller's state that it is carrying this structure.
        controller.state.carryingStructure = this;
        condition = Condition.Interacting;

        return true;
    }

    public bool Throw(Controller controller) {
        OnThrow();

        // Unattach the parent and reattach the mesh.
        transform.parent = null;
        mesh.transform.parent = transform;

        // Enable the rigidbody with a slight delay on the collider.
        body.simulated = true;
        StartCoroutine(IECollisionEnable(collisionBuffer));

        // Inform the controller's state that it is no longer carrying this.
        controller.state.carryingStructure = null;

        // Apply the throw.
        Vector2 direction = (Vector3)Compass.OrientationVectors[controller.state.orientation];
        body.velocity = throwSpeed * (Vector3)direction;
        condition = Condition.Uninteractable;

        return true;
    }

    /* --- Event Methods --- */
    // Extra carrying logic for inheriting classes.
    protected virtual void OnCarry() {
        // Determined by the particular class.
    }


    // Extra carrying logic for inheriting classes.
    protected virtual void OnThrow() {
        // Determined by the particular class.
    }

    /* --- Coroutines --- */
    protected IEnumerator IECollisionEnable(float delay) {
        yield return new WaitForSeconds(delay);
        mesh.frame.enabled = true;
        yield return null;
    }

}
