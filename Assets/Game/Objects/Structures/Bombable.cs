using System.Collections;
using System.Collections.Generic;
using UnityEngine;

/// <summary>
/// An object that is affected by bombs.
/// </summary>
[RequireComponent(typeof(Hurtbox))]
public class Bombable : MonoBehaviour {

    /* --- Variables --- */
    [SerializeField] public int threshold = 1; // The number of damage this object can take before collapsing.
    [SerializeField] protected int damage = 0; // The damage this object has taken.

    /* --- Unity --- */
    // Runs once on instantiation.
    void Awake() {
        // Set up the attached components.
        tag = GameRules.bombableTag;
    }

    /* --- Methods --- */
    // Add another increment of damage to this object.
    public void Blast() {
        damage++;
        if (damage >= threshold) {
            Destroy(gameObject);
        }
    }

}
