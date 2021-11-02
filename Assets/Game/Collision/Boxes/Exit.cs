/* --- Libraries --- */
using System.Collections;
using System.Collections.Generic;
using UnityEngine;

/// <summary>
/// A collider to detect when the player is leaving a room.
/// </summary>
[RequireComponent(typeof(CircleCollider2D))]
public class Exit : MonoBehaviour {

    /* --- Components --- */
    public Map map;
    public bool isLocked;
    public Lock @lock;
    [HideInInspector] protected Collider2D area;

    public Loader.LDtkTileData exitData;
    public int index;

    /* --- Variables --- */
    [SerializeField] [ReadOnly] public Vector2Int id = Vector2Int.zero; // Used to indicate which direction this exit faces.
    [HideInInspector] public static float offset = 7.75f; // The value to offset the player position by.

    /* --- Unity --- */
    // Runs once on instantiation
    void Awake() {
        // Cache these references.
        area = GetComponent<Collider2D>();
        // Set up the attached components.
        area.isTrigger = true;
    }

    void Update() {
        if (isLocked) {
            @lock.gameObject.SetActive(true);
            area.enabled = false;
        }
        else {
            @lock.gameObject.SetActive(false);
            area.enabled = true;
        }
    }

    void OnColliderEnter2D(Collision2D collision) {
        if (collision.collider.GetComponent<Player>() != null) {
            print(true);
        }
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
        // Move the player.
        Vector3 currPosition = hurtbox.controller.transform.position;
        Vector3 deltaPosition = new Vector3(-id[0] * offset, id[1] * offset, 0);
        // print("Delta Position for exit: " + deltaPosition.ToString());
        hurtbox.controller.transform.position = currPosition + deltaPosition;

        // Load the new room.
        if (id[0] != 0 || id[1] != 0) {
            // Get the new room id.
            Vector2Int newLocation = new Vector2Int( map.location[0] + id[0], map.location[1] + id[1] );
            map.OpenRoom(newLocation);
        }
    }

}
