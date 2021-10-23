using System.Collections;
using System.Collections.Generic;
using UnityEngine;

/// <summary>
/// A collider to detect when the player is leaving a room.
/// </summary>
[RequireComponent(typeof(CircleCollider2D))]
public class Exitbox : MonoBehaviour {

    /* --- Components --- */
    [HideInInspector] protected Dungeon dungeon;
    [HideInInspector] protected Collider2D area;

    /* --- Variables --- */
    [HideInInspector] public int[] id = new int[] { 0, 0 }; // Used to indicate which direction this exit faces.
    [HideInInspector] static float offset = 8.85f; // The value to offset the player position by.

    /* --- Unity --- */
    // Runs once on instantiation
    void Awake() {
        // Cache these references.
        dungeon = GameObject.FindWithTag(GameRules.dungeonTag)?.GetComponent<Dungeon>();
        area = GetComponent<Collider2D>();
        // Set up the attached components.
        area.isTrigger = true;
    }

    // Called when the attached collider intersects with another collider
    void OnTriggerEnter2D(Collider2D collider) {
        ScanExit(collider);
    }

    /* --- Methods --- */
    // Scans for whether to trigger an exit event.
    void ScanExit(Collider2D collider) {
        if (collider.GetComponent<Hurtbox>() != null) {
            Hurtbox hurtbox = collider.GetComponent<Hurtbox>();
            if (hurtbox.controller.tag == GameRules.playerTag) {
                OnExit(hurtbox);
            }
        }
    }

    // The logic to execute when an exit event is triggered.
    void OnExit(Hurtbox hurtbox) {
        // Move the player
        Vector3 currPosition = hurtbox.controller.transform.position;
        Vector3 deltaPosition = new Vector3(-id[1] * offset, id[0] * offset, 0);
        hurtbox.controller.transform.position = currPosition + deltaPosition;

        // Load the new room.
        if (id[0] != 0 || id[1] != 0) {
            // Get the new room id.
            int[] newID = new int[] { dungeon.id[0] + id[0], dungeon.id[1] + id[1] };
            dungeon.LoadRoom(newID);
        }
    }

}
