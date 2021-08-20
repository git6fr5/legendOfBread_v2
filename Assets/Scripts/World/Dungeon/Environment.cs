using System.Collections;
using System.Collections.Generic;
using UnityEngine;
using UnityEngine.Tilemaps;

public class Environment : MonoBehaviour {

    /* --- Components --- */
    // Preset TileBases
    public TileBase[] floor;
    public TileBase[] border;
    // Controllers
    public Mob[] mobs;
    public Trap[] traps;
    // Exit
    public Exit exit;

    /* --- Unity --- */
    // Runs once on compilation.
    void Awake() {
        border = Janitor.BorderOrder(border);
    }

    /* --- Methods --- */
    // Converts an array of controllers to be used as tiles.
    // Has to be sprite tiles to be able to access the sprite later on.
    public SpriteTile[] ControllersToTileBase(Controller[] controllers) {
        // Get the max ID in the array of controllers.
        int maxID = 0;
        for (int i = 0; i < controllers.Length; i++) {
            if (controllers[i].id > maxID) {
                maxID = controllers[i].id;
            }
        }
        // Create the array of tiles.
        SpriteTile[] tiles = new SpriteTile[maxID + 1];
        for (int i = 0; i < controllers.Length; i++) {
            SpriteTile newTile = ScriptableObject.CreateInstance<SpriteTile>();
            // Find the mesh on the controller.
            foreach (Transform child in controllers[i].transform) {
                if (child.tag == GameRules.meshTag) {
                    newTile.newSprite = child.GetComponent<SpriteRenderer>().sprite;
                    tiles[controllers[i].id] = newTile;
                }
            }
        }
        return tiles;
    }

    // Converts an array of controllers to be used as tiles.
    // Has to be sprite tiles to be able to access the sprite later on.
    public Controller[] OrderedControllers(Controller[] controllers) {
        // Get the max ID in the array of controllers.
        int maxID = 0;
        for (int i = 0; i < controllers.Length; i++) {
            if (controllers[i].id > maxID) {
                maxID = controllers[i].id;
            }
        }
        // Create the array of tiles.
        Controller[] orderedControllers = new Controller[maxID + 1];
        for (int i = 0; i < controllers.Length; i++) {
            orderedControllers[controllers[i].id] = controllers[i];
        }
        return orderedControllers;
    }

}
