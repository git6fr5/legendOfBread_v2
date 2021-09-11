using System.Collections;
using System.Collections.Generic;
using UnityEngine;

public class NPC : Controller {

    public string npcFilename;

    /* --- Variables --- */
    // NPC-Specific Components
    public Vision vision;

    void Start() {
        body.mass = 1e9f;
    }

    /* --- Override --- */
    // Sets the action controls.
    protected override void Think() {
        //
        movementVector = Vector2.zero;
        Interact();
    }

    void Interact() {
        Player player = vision.LookFor(GameRules.playerTag)?.controller?.GetComponent<Player>();
        if (player != null && !player.state.isActive && Input.GetKeyDown(player.interactKey)) {
            player.Talk(npcFilename);
        }
    }

}
