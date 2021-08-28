using System.Collections;
using System.Collections.Generic;
using UnityEngine;

[RequireComponent(typeof(CircleCollider2D))]
public class Exitbox : MonoBehaviour {

    /* --- Components --- */
    Dungeon dungeon;

    /* --- Variables --- */
    public int[] id = new int[] { 0, 0 };

    /* --- Unity --- */
    // Runs once on instantiation
    void Awake() {
        GameObject dungeonObject = GameObject.FindWithTag(GameRules.dungeonTag);
        if (dungeonObject != null) {
            dungeon = dungeonObject.GetComponent<Dungeon>();
        }
        GetComponent<CircleCollider2D>().isTrigger = true;
    }

    // Called when the attached collider intersects with another collider
    void OnTriggerEnter2D(Collider2D collider) {
        ScanExit(collider);
    }

    /* --- Methods --- */
    void ScanExit(Collider2D collider) {
        if (collider.GetComponent<Hurtbox>() != null) {
            Hurtbox hurtbox = collider.GetComponent<Hurtbox>();
            if (hurtbox.controller.tag == GameRules.playerTag) {
                OnExit(hurtbox);
            }
        }
    }

    // if colliding with a players hitbox, then exit
    void OnExit(Hurtbox hurtbox) {
        print("exit");

        // move the player
        Vector3 currPosition = hurtbox.controller.transform.position;

        // slightly different values along the x and y axis because of the rectangular shape
        // of the players hitbox
        float dist = 8.85f;
        Vector3 deltaPosition = new Vector3(-id[1] * dist, id[0] * dist, 0);
        hurtbox.controller.transform.position = currPosition + deltaPosition;

        // load the new room
        if (id[0] != 0 || id[1] != 0) {
            int[] newID = new int[] { dungeon.id[0] + id[0], dungeon.id[1] + id[1] };
            // dungeon.DeloadRoom();
            dungeon.LoadRoom(newID);
        }
    }

}
