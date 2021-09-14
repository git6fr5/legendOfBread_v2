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
    [Range(0.005f, 0.1f)] public float throwBuffer = 0.025f; // The buffer time between which this can be picked up and thrown.
    [Range(0.5f, 4f)] public float throwDistance = 1f; // The distance this can be thrown. Consider moving this to a player stat?
    [Range(0.05f, 1f)] public float carryWeight = 0.65f;
    // Internal Throwing Mechanics.
    [HideInInspector] protected Vector2 targetPosition; // The position this object is interpolating towards.

    /* --- Unity --- */
    // Runs once on instantiation.
   void Awake() {
        // Cache these references.
        body = GetComponent<Rigidbody2D>();
        // Set up the attached components.
        body.constraints = RigidbodyConstraints2D.FreezeAll;
        body.gravityScale = 0f;
        body.angularDrag = 0f;
    }

    /* --- Overridden Methods --- */
    // Picks the structure up or throws it, depending on the current state.
    public override bool Interact(Controller controller) {
        print("interacting");
        if (condition == Condition.Interactable) {
            return Carry(controller);
        }
        else if (condition == Condition.Interacting) {
            return Throw(controller);
        }
        return false;
    }

    // Runs while the structure is being interacted with.
    protected override void Interacting() {
        // Slow down the player.
    }

    bool Carry(Controller controller) {
        OnCarry();
        // Set the position of the object over the player's head.
        transform.parent = controller.mesh.overhead;
        transform.localPosition = Vector3.zero;

        mesh.hull.parent = controller.mesh.hull;
        mesh.hull.localPosition = new Vector3(0f, -GameRules.movementPrecision, 0f);

        // Getting weird bounce effect.
        body.simulated = false;
        body.constraints = RigidbodyConstraints2D.FreezeRotation;

        // Disable the collision frame.
        mesh.frame.enabled = false;
        controller.state.carryingStructure = this;
        // interactAction = Action.Throwing;
        return true;
    }

    protected virtual void OnCarry() {
        //
    }

    public bool Throw(Controller controller) {
        OnThrow();
        print("throwing");
        transform.parent = null;
        mesh.hull.parent = mesh.transform;

        // Getting weird bounce effect.
        body.simulated = true;
        body.constraints = RigidbodyConstraints2D.FreezeAll;

        // Disable the collision frame.
        mesh.frame.enabled = true;
        controller.state.carryingStructure = null;
        // interactAction = Action.Carrying;
        return true;
    }

    protected virtual void OnThrow() {
        //
    }

    public void Thrown() {

    }

}
