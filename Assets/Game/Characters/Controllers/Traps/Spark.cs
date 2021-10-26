/* --- Libraries --- */
using System.Collections;
using System.Collections.Generic;
using UnityEngine;

/// <summary>
/// 
/// </summary>
public class Spark : Trap {

    /* --- Variables --- */
    public int damage;

    /* --- Properties --- */
    public Transform[] tracking;

    protected override void Off() {

    }

    protected override void On() {

        Vector2 displacement = Vector2.zero;
        for (int i = 0; i < tracking.Length; i++) {
            // Get the displacement from the tracking object.
            displacement = displacement + (Vector2)(tracking[i].position - transform.position);       
        }

        // Draw the displacement.
        Debug.DrawRay(transform.position, displacement, Color.green);

        // Get the standard bases of the vector.
        Vector2 horizontalDisplacement = new Vector2(displacement.x, 0f);
        Vector2 verticalDisplacement = new Vector2(0f, displacement.y);

        // Redraw the bigger one in red.
        Vector2 largerDisplacement = horizontalDisplacement.magnitude > verticalDisplacement.magnitude ? horizontalDisplacement : verticalDisplacement;
        Vector2 smallerDisplacement = horizontalDisplacement.magnitude < verticalDisplacement.magnitude ? horizontalDisplacement : verticalDisplacement;

        // Draw the components.
        Debug.DrawRay(transform.position, largerDisplacement, Color.red);
        Debug.DrawRay(transform.position, smallerDisplacement, Color.cyan);

        // Travel perpendicular to the larger displacement.
        Vector2 direction = (Vector2)(Quaternion.Euler(0, 0, -90f) * largerDisplacement);

        Debug.DrawRay(transform.position, direction.normalized, Color.yellow);
        movementVector = direction.normalized;

    }

    /* --- Event Actions --- */
    protected override void OnHit(Hurtbox hurtbox) {
        hurtbox.controller.Hurt(damage);
        Destroy(gameObject);
    }

}
