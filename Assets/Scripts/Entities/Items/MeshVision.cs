/* --- Libraries --- */
using System.Collections;
using System.Collections.Generic;
using UnityEngine;

public class MeshVision : MonoBehaviour {

    /* --- Components --- */
    [HideInInspector] protected Collider2D area;

    /* --- Variables ---*/
    [SerializeField] public List<Controller> container = new List<Controller>(); // A list of bombable tiles within vision range.

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
        if (collider.GetComponent<Mesh>() != null && collider.transform.parent.GetComponent<Controller>() != null) {
            Controller controller = collider.transform.parent.GetComponent<Controller>();
            if (!container.Contains(controller) && see) {
                container.Add(controller);
            }
            else if (container.Contains(controller) && !see) {
                container.Remove(controller);
            }
        }
    }

    // Reset the container.
    public void Reset() {
        container = new List<Controller>();
    }

}
