/* --- Libraries --- */
using System.Collections;
using System.Collections.Generic;
using UnityEngine;

/* --- Enumerations --- */
using Action = State.Action;

/// <summary>
/// Used to pick up objects.
/// </summary>
public class Glove : Item
{
    /* --- Unity --- */
    // Runs once on initialisation.
    void Awake() {
        action = Action.Carrying;
    }

    protected override bool OnActivate(Controller controller) {
        action = Action.Carrying;
        Interactbox interactbox = controller.GetComponent<Player>()?.interactbox;
        Structure structure = interactbox?.LookFor(action);
        if (structure != null && structure.condition != Structure.Condition.Uninteractable) {
            return structure.Interact(controller);
        }
        return false;
    }

}
