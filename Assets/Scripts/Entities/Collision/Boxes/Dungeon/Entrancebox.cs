using System.Collections;
using System.Collections.Generic;
using UnityEngine;
using UnityEngine.SceneManagement;

/// <summary>
/// A collider to detect when the player is leaving a room.
/// </summary>
[RequireComponent(typeof(CircleCollider2D))]
public class Entrancebox : MonoBehaviour {

    /* --- Components --- */
    [HideInInspector] protected Collider2D area;

    /* --- Variables --- */
    [SerializeField] public string mapFile;

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
        LoadDungeon(collider);
    }

    /* --- Methods --- */
    // Scans for whether to trigger an exit event.
    void LoadDungeon(Collider2D collider) {
        Dungeon.mapfile = mapFile;
        SceneManager.LoadScene(GameRules.DungeonScene, LoadSceneMode.Single);
    }

}
