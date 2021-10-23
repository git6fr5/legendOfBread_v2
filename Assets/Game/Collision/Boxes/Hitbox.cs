/* --- Libraries --- */
using System.Collections;
using System.Collections.Generic;
using UnityEngine;

/// <summary>
/// A collider to detect hostile collisions.
/// </summary>
[RequireComponent(typeof(BoxCollider2D))]
public class Hitbox : MonoBehaviour {

    /* --- Components --- */
    [HideInInspector] protected Collider2D area;

    /* --- Variables ---*/
    [SerializeField] public Controller controller; // A reference to the parent controlling this hitbox.
    [SerializeField] public List<Hurtbox> container = new List<Hurtbox>(); // A list of hurtboxes currently intersected with, to avoid doubling events.

    /* --- Unity --- */
    // Runs once on instantiation
    void Awake() {
        // Cache these references.
        area = GetComponent<Collider2D>();
        // Set up the attached components.
        area.isTrigger = true;
    }

    // Called when the attached collider intersects with another collider.
    void OnTriggerEnter2D(Collider2D collider) {
        ScanHit(collider, true);
    }

    // Called when the attached collider intersects with another collider.
    void OnTriggerExit2D(Collider2D collider) {
        ScanHit(collider, false);
    }

    /* --- Methods --- */
    // Scans for whether to trigger an hit event.
    void ScanHit(Collider2D collider, bool hit) {
        // If we intersected with a new hurtbox, then hit it.
        if (collider.GetComponent<Hurtbox>() != null) {
            Hurtbox hurtbox = collider.GetComponent<Hurtbox>();
            if (!container.Contains(hurtbox) && hit) {
                for (int i = 0; i < controller.state.enemyTags.Count; i++) {
                    if (hurtbox.controller.tag == controller.state.enemyTags[i]) {
                        container.Add(hurtbox);
                        OnAdd(hurtbox);
                    }
                }
            }
            else if (container.Contains(hurtbox) && !hit) {
                container.Remove(hurtbox);
            }
        }
    }

    // Reset the container.
    public void Reset() {
        container = new List<Hurtbox>();
    }

    // Reset the container.
    public bool IsEmpty() {
        return (container.Count == 0);
    }

    protected virtual void OnAdd(Hurtbox hurtbox) {
        controller?.Hit(hurtbox);
    }

}
