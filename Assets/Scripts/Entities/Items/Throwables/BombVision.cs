/* --- Libraries --- */
using System.Collections;
using System.Collections.Generic;
using UnityEngine;

public class BombVision : MonoBehaviour {

    /* --- Components --- */
    [HideInInspector] protected Collider2D area;

    /* --- Variables ---*/
    [SerializeField] public List<Bombable> container = new List<Bombable>(); // A list of bombable tiles within vision range.

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
        if (collider.GetComponent<Bombable>() != null) {
            Bombable bombable = collider.GetComponent<Bombable>();
            if (!container.Contains(bombable) && see) {
                container.Add(bombable);
            }
            else if (container.Contains(bombable) && !see) {
                container.Remove(bombable);
            }
        }
    }

    // Reset the container.
    public void Reset() {
        container = new List<Bombable>();
    }

}
