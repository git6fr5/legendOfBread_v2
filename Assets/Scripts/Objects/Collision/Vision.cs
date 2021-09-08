/* --- Libraries --- */
using System.Collections;
using System.Collections.Generic;
using UnityEngine;

/// <summary>
/// A collider to detect hostile entities.
/// </summary>
[RequireComponent(typeof(CircleCollider2D))]
public class Vision : MonoBehaviour {

    /* --- Components --- */
    [HideInInspector] protected Collider2D area;

    /* --- Variables ---*/
    [SerializeField] protected List<Hurtbox> container = new List<Hurtbox>(); // A list of hurtboxes within vision range.

    /* --- Unity --- */
    // Runs once on instantiation
    void Awake() {
        // Cache these references.
        area = GetComponent<Collider2D>();
        // Set up the attached components.
        area.isTrigger = true;
    }

    // Called when the attached collider intersects with another collider
    void OnTriggerEnter2D(Collider2D collider) {
        ScanVision(collider, true);
    }

    // Called when the attached collider intersects with another collider
    void OnTriggerExit2D(Collider2D collider) {
        ScanVision(collider, false);
    }

    /* --- Methods --- */
    // Scans for whether anything new has entered vision range.
    void ScanVision(Collider2D collider, bool see) {
        // Vision can see hurtboxes, but hurtboxes don't react to vision.
        if (collider.GetComponent<Hurtbox>() != null) {
            Hurtbox hurtbox = collider.GetComponent<Hurtbox>();
            if (!container.Contains(hurtbox) && see) {
                container.Add(hurtbox);
            }
            else if (container.Contains(hurtbox) && !see) {
                container.Remove(hurtbox);
            }
        }
    }

    // Reset the container.
    public void Reset() {
        container = new List<Hurtbox>();
    }
    
    // Check whether a particular type of entity is within vision range.
    public Hurtbox LookFor(string searchTag) {
        for (int i = 0; i < container.Count; i++) {
            if (container[i].controller.tag == searchTag) {
                return container[i];
            }
        }
        return null;
    }

}
