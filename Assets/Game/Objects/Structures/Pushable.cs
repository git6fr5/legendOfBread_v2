/* --- Libraries --- */
using System.Collections;
using System.Collections.Generic;
using UnityEngine;

/* --- Enumerations --- */
using Orientation = Compass.Orientation;

[RequireComponent(typeof(Rigidbody2D))]
public class Pushable : Structure {

    /* --- Components --- */
    [HideInInspector] public Rigidbody2D body;

    /* --- Variables --- */
    [SerializeField] [Range(0f, 5f)] protected float pushDistance = 1f; // The distance this structure is pushed.
    [SerializeField] [Range(0f, 5f)] protected float pushSpeed = 3f; // The speed at which this structure is pushed.
    // Internal Pushing Mechanics.
    [SerializeField] [ReadOnly] protected Vector2 targetPoint; // The position this object is interpolating towards.
    [SerializeField] [ReadOnly] protected float pushFriction; // The factor by which this object slows down.
    [HideInInspector] [ReadOnly] protected Vector3 origin; // The last snapped position of this object.
    [HideInInspector] [ReadOnly] public float pushedTime = 0f; // The amount of time this has been pushed for.
    [SerializeField] [ReadOnly] private float pushInterval = 1f; // The maximum interval that this can be pushed for before snapping.
    [SerializeField] [ReadOnly] private float pushBuffer; // The time between being interacted with and being pushed.


    /* --- Unity --- */
    // Runs once on instantiation.
    void Awake() {
        // Cache these references.
        body = GetComponent<Rigidbody2D>();
        // Set up these components.
        body.constraints = RigidbodyConstraints2D.FreezeAll;
        body.gravityScale = 0f;
        body.angularDrag = 0f;
        // Initialize these variables.
        origin = transform.position;
        pushFriction = 2 * pushDistance / pushSpeed;
        interactAction = State.Action.Pushing;
    }

    /* --- Overridden Methods --- */
    public override bool Interact(Controller controller) {

        Orientation pusherOrientation = controller.state.orientation;
        Vector3 pusherPosition = controller.transform.position;
        float pushBuffer = controller.state.activeItem ? controller.state.activeItem.actionBuffer / 2f : 0f;

        if (condition != Condition.Interactable) { return false; }

        condition = Condition.Interacting;
        body.constraints = RigidbodyConstraints2D.FreezeRotation;
        if (ValidPushDirection(pusherOrientation, pusherPosition)) {
            Push(pusherOrientation, pusherPosition);
            return true;
        }
        return false;
    }

    protected override void Interacting() {
        pushedTime += Time.deltaTime;
        
        // Slow down this structure.
        body.velocity = body.velocity - (body.velocity * pushFriction * Time.deltaTime);

        if (Vector2.Distance(transform.position, targetPoint) < GameRules.movementPrecision || pushedTime >= pushInterval || body.velocity.magnitude < GameRules.movementPrecision) {
            // Snap to the nearest grid.
            // transform.position = new Vector3(Mathf.Round(transform.position.x), Mathf.Round(transform.position.y), transform.position.z) + (Vector3)offset;
            transform.position = Room.SnapToGrid(transform.position);

            // Freeze the body.
            body.constraints = RigidbodyConstraints2D.FreezeAll;

            // Reset these variables.
            body.velocity = Vector2.zero;
            origin = transform.position;
            condition = Condition.Interactable;
            pushedTime = 0f;
        }
    }

    /* --- Action Methods --- */
    void Push(Orientation pusherOrientation, Vector3 pusherPosition) {
        // Get the push direction.
        Vector2 pushDirection = Compass.OrientationVectors[pusherOrientation];

        // Get the target point and snap it to the grid.
        targetPoint = (Vector2)transform.position + pushDistance * pushDirection;
        targetPoint = new Vector2(targetPoint.x, targetPoint.y);

        // Apply the force.
        body.velocity = pushSpeed * pushDirection;
    }

    // Checks that the player orientation and position are aligned such that
    // It makes sense for the player to push this object in this direction
    // From where he is (e.g. pushes right while standing on the left).
    bool ValidPushDirection(Orientation pusherOrientation, Vector3 pusherPosition) {

        // Get the respective directions.
        Vector2 pushDirection = (Vector3)Compass.OrientationVectors[pusherOrientation];
        Vector2 pusherDirection = Compass.SnapVector(transform.position - pusherPosition);

        // Check that the directions are aligned.
        return (Vector2.Dot(pushDirection, pusherDirection) > 0) ? true : false;
    }

    /* --- Coroutines --- */
    protected IEnumerator IEPush(float delay, Orientation pusherOrientation, Vector3 pusherPosition) {
        yield return new WaitForSeconds(delay);
        Push(pusherOrientation, pusherPosition);
        yield return null;
    }

}
