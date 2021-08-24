using System.Collections;
using System.Collections.Generic;
using UnityEngine;

public class NPC : Controller {

    public string npcFilename;

    /* --- Variables --- */
    // NPC-Specific Components
    public Vision vision;
    public bool isInteractable;

    void Start() {
        body.mass = 1e9f;
    }

    /* --- Override --- */
    // Sets the action controls.
    protected override void Think() {
        //
        movementVector = Vector2.zero;

        Player player = vision.LookFor(GameRules.playerTag)?.controller?.GetComponent<Player>();
        if (player != null) {
            isInteractable = true;
        }
        else {
            isInteractable = false;
        }

        if (isInteractable && !player.isInteracting && Input.GetKeyDown(player.interactKey)) {
            player.isInteracting = true;
            player.dialogue.Run(npcFilename);
        }
    }

}
