using System.Collections;
using System.Collections.Generic;
using UnityEngine;

using Orientation = Compass.ORIENTATION;

public class Pushable : Structure {

    /* --- Variables --- */
    
    public float pushBuffer = 0.025f; // port this to structure?

    [SerializeField] protected Vector2 targetPoint; // port this to structure as well?
    [SerializeField] protected float speed = 3f;
    [SerializeField] protected float pushDistance = 1f;
    [SerializeField] protected float friction;

    public Vector3 origin;
    public float pushedTime = 0f;
    public float maxPushedTime = 1f;

    Rigidbody2D body;

    void Start() {
        // Cache these references.
        body = GetComponent<Rigidbody2D>();
        // Set up these components.
        body.constraints = RigidbodyConstraints2D.FreezeAll;
        body.gravityScale = 0f;
        body.angularDrag = 0f;
        // Initialize these variables.
        origin = transform.position;
        friction = 2 * pushDistance / speed;
        interactAction = State.Action.Pushing;
    }

    protected override void Interacting() {
        pushedTime += Time.deltaTime;
        
        // Slow down this structure.
        body.velocity = body.velocity - (body.velocity * friction * Time.deltaTime);

        if (Vector2.Distance(transform.position, targetPoint) < GameRules.movementPrecision || pushedTime >= maxPushedTime || body.velocity.magnitude < GameRules.movementPrecision) {            
            // Snap to the nearest grid.
            transform.position = new Vector3(Mathf.Round(transform.position.x), Mathf.Round(transform.position.y), transform.position.z);

            // Freeze the body.
            body.constraints = RigidbodyConstraints2D.FreezeAll;

            // Reset these variables.
            body.velocity = Vector2.zero;
            origin = transform.position;
            condition = Condition.Interactable;
            pushedTime = 0f;
        }
    }

    public override bool Interact(Controller controller) {

        Orientation pusherOrientation = controller.state.orientation;
        Vector3 pusherPosition = controller.transform.position;

        if (condition != Condition.Interactable) { return false; }

        condition = Condition.Interacting;
        body.constraints = RigidbodyConstraints2D.FreezeRotation;
        if (ValidPushDirection(pusherOrientation, pusherPosition)) {
            Push(pusherOrientation, pusherPosition);
            return true;
        }
        return false;
    }

    void Push(Orientation pusherOrientation, Vector3 pusherPosition) {
        // Get the push direction.
        Vector2 pushDirection = Compass.OrientationVectors[pusherOrientation];

        // Get the target point and snap it to the grid.
        targetPoint = (Vector2)transform.position + pushDistance * pushDirection;
        targetPoint = new Vector2((float)(int)targetPoint.x, (float)(int)targetPoint.y);

        // Apply the force.
        body.velocity = speed * pushDirection;
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

}
