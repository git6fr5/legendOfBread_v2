using System.Collections;
using System.Collections.Generic;
using UnityEngine;

[RequireComponent(typeof(CircleCollider2D))]
public class Vision : MonoBehaviour {

    /* --- Components ---*/
    public List<Hurtbox> container = new List<Hurtbox>();

    /* --- Unity --- */
    void Awake() {
        GetComponent<CircleCollider2D>().isTrigger = true;
    }

    // called when the attached collider intersects with another collider
    void OnTriggerEnter2D(Collider2D collider) {
        ScanVision(collider, true);
    }

    // called when the attached collider intersects with another collider
    void OnTriggerExit2D(Collider2D collider) {
        ScanVision(collider, false);
    }

    /* --- Methods --- */
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

    public void Reset() {
        container = new List<Hurtbox>();
    }

    /* --- Searching Vision --- */
    public Hurtbox LookFor(string searchTag) {
        for (int i = 0; i < container.Count; i++) {
            if (container[i].controller.tag == searchTag) {
                return container[i];
            }
        }
        return null;
    }

}
