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
    public Block currentBlock;

    protected override void Off() {

    }

    protected override void On() {

        if (currentBlock == null) {
            Memory memory = GetComponent<Memory>();

            Vector2 dir = new Vector2(memory.direction.x, -memory.direction.y);
            RaycastHit2D[] hits = Physics2D.RaycastAll(transform.position, dir, 1f);
            for (int j = 0; j < hits.Length; j++) {
                if (hits[j].collider.transform.parent.GetComponent<Block>() != null) {
                    Block block = hits[j].collider.transform.parent.GetComponent<Block>();
                    currentBlock = block;
                    break;
                }
            }
            // If still can't find anything.
            if (currentBlock == null) {
                return;
            }
        }

        // Check the current block is still the closest block
        Block newBlock = currentBlock;
        Vector2 displacement = currentBlock.transform.position - transform.position;
        for (int i = 0; i < currentBlock.group.Count; i++) {
            Vector2 newDisplacement = currentBlock.group[i].transform.position - transform.position;
            if (newDisplacement.magnitude < displacement.magnitude) {
                displacement = newDisplacement;
                newBlock = currentBlock.group[i];
            }
        }
        // Otherwise we would swap the current block.
        currentBlock = newBlock;

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

        // Account for internal edges and going astray.
        if (largerDisplacement.magnitude > 1f) {
            movementVector += largerDisplacement.normalized;
        }

        // Make sure it stays on the grid.
        if (largerDisplacement.magnitude < 1f - GameRules.movementPrecision) {
            transform.position = transform.position - (Vector3)(1f * largerDisplacement.normalized - largerDisplacement);
        }
    }

    /* --- Event Actions --- */
    protected override void OnHit(Hurtbox hurtbox) {
        hurtbox.controller.Hurt(damage);
        Destroy(gameObject);
    }

}
