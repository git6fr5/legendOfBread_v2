/* --- Libraries --- */
using System.Collections;
using System.Collections.Generic;
using UnityEngine;

/// <summary>
/// A collider to detect hostile collisions.
/// </summary>
[RequireComponent(typeof(BoxCollider2D))]
public class Interactbox : MonoBehaviour {

    /* --- Components --- */
    [HideInInspector] protected Collider2D area;
    public Structure structure;

    /* --- Variables ---*/
    [SerializeField] protected List<Structure> container = new List<Structure>(); // A list of hurtboxes currently intersected with, to avoid doubling events.

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
        ScanStructure(collider, true);
    }

    // Called when the attached collider intersects with another collider.
    void OnTriggerExit2D(Collider2D collider) {
        ScanStructure(collider, false);
    }

    /* --- Methods --- */
    // Scans for whether to trigger an hit event.
    void ScanStructure(Collider2D collider, bool enter) {
        // If we intersected with a new hurtbox, then hit it.
        if (collider.GetComponent<Interactbox>() != null) {
            Structure structure = collider.GetComponent<Interactbox>().structure;
            if (structure != null && !container.Contains(structure) && enter) {
                container.Add(structure);
            }
            else if (container.Contains(structure) && !enter) {
                container.Remove(structure);
            }
        }
    }

    // Reset the container.
    public void Reset() {
        container = new List<Structure>();
    }

    public Structure LookFor(State.Action interactAction) {
        for (int i = 0; i < container.Count; i++) {
            if (container[i].interactAction == interactAction) {
                return container[i];
            }
        }
        return null;
    }

}
