using System.Collections;
using System.Collections.Generic;
using UnityEngine;

using ORIENTATION = Compass.ORIENTATION;

[RequireComponent(typeof(Rigidbody2D))]
public class Throwable : Structure {

    /* --- Components --- */
    [HideInInspector] public Rigidbody2D body;

    /* --- Variables --- */
    [Range(0.005f, 0.1f)] public float throwBuffer = 0.025f; // The buffer time between which this can be picked up and thrown.
    [Range(0.5f, 4f)] public float throwDistance = 1f; // The distance this can be thrown. Consider moving this to a player stat?
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
    protected override void Interact(Player player) {
        if (condition == Condition.Interacting) {
            Throw(player);
        }
        else if (condition == Condition.Interactable) {
            Carry(player);
        }
    }

    // Runs while the structure is being interacted with.
    protected override void Interacting() {
        // Slow down the player.
    }

    void Carry(Player player) {
        OnCarry();
        // Set the position of the object over the player's head.
        transform.parent = player.mesh.overhead;
        transform.localPosition = Vector3.zero;

        // throwable.mesh.hull.position = //

        // Disable the 
        mesh.frame.enabled = false;
        player.state.isActive = true;
    }

    protected virtual void OnCarry() {
        //
    }

    void Throw(Player player) {
        OnThrow();
    }

    protected virtual void OnThrow() {
        //
    }

    public void Thrown() {
        
    }

}
