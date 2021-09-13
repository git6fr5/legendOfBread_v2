using System.Collections;
using System.Collections.Generic;
using UnityEngine;

using Action = State.Action;

public class Structure : MonoBehaviour {

    /* --- Enumerations --- */
    // Tracks the condition of this structure.
    public enum Condition {
        Interactable,
        Interacting,
        Uninteractable
    }

    public Condition condition;
    public Vision interactVision;
    public Mesh mesh;

    void Update() {
        // Check for interactions if this is interactable.
        if (condition != Condition.Uninteractable){
            InteractFlag();
        }
        // Check if this is currently being interacted with.
        if (condition == Condition.Interacting) {
            Interacting();
        }
    }

    void InteractFlag() {
        Player player = interactVision.LookFor(GameRules.playerTag)?.controller?.GetComponent<Player>();
        if (player != null && player.state.action == Action.Inactive && Input.GetKeyDown(player.interactKey)) {
            Interact(player);
        }
    }

    protected virtual void Interact(Player player) {
        //
    }

    protected virtual void Interacting() {
        //
    }

}
