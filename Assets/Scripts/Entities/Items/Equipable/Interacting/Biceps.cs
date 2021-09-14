/* --- Libraries --- */
using System.Collections;
using System.Collections.Generic;
using UnityEngine;

/* --- Enumerations --- */
using Action = State.Action;

/// <summary>
/// Used to push objects around.
/// </summary>
public class Biceps : Equipable
{
    /* --- Unity --- */
    // Runs once on initialisation.
    void Awake() {
        action = Action.Pushing;
    }

    protected override bool OnActivate(Controller controller) {
        Interactbox interactbox = controller.GetComponent<Player>()?.interactbox;
        Structure structure = interactbox?.LookFor(action);
        if (structure != null && structure.condition != Structure.Condition.Uninteractable) {
            return structure.Interact(controller.GetComponent<Player>());
        }
        return false;
    }

}
