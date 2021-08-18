/* --- Libraries --- */
using System.Collections;
using System.Collections.Generic;
using UnityEngine;

[RequireComponent(typeof(BoxCollider2D))]
public class Hitbox : MonoBehaviour {

    /* --- Components ---*/
    public Controller controller;
    public string targetTag;
    public List<Hurtbox> container = new List<Hurtbox>();

    /* --- Unity --- */
    void Awake() {
        GetComponent<BoxCollider2D>().isTrigger = true;
    }

    // called when the attached collider intersects with another collider
    void OnTriggerEnter2D(Collider2D collider) {
        ScanHit(collider, true);
    }

    // called when the attached collider intersects with another collider
    void OnTriggerExit2D(Collider2D collider) {
        ScanHit(collider, false);
    }

    /* --- Methods --- */
    void ScanHit(Collider2D collider, bool hit) {
        // If we intersected with a new hurtbox, then hit it.
        if (collider.GetComponent<Hurtbox>() != null) {
            Hurtbox hurtbox = collider.GetComponent<Hurtbox>();
            if (!container.Contains(hurtbox) && hit && hurtbox.controller.tag == targetTag) {
                container.Add(hurtbox);
                controller.Hit(hurtbox);
            }
            else if (container.Contains(hurtbox) && !hit) {
                container.Remove(hurtbox);
            }
        }
    }

    public void Reset() {
        container = new List<Hurtbox>();
    }

}
