/* --- Libraries --- */
using System.Collections;
using System.Collections.Generic;
using UnityEngine;

/* --- Enumerations --- */
using Action = State.Action;

/// <summary>
/// Used to pick up and throw objects.
/// </summary>
public class Glove : Equipable
{
    /* --- Unity --- */
    // Runs once on initialisation.
    void Awake() {
        action = Action.Carrying;
    }

    protected override bool OnActivate(Controller controller) {
        if (controller.state.carryingStructure != null) {
            action = Action.Throwing;
            controller.state.carryingStructure.Throw(controller);
            return true;
        }
        action = Action.Carrying;
        Interactbox interactbox = controller.GetComponent<Player>()?.interactbox;
        Structure structure = interactbox?.LookFor(action);
        if (structure != null && structure.condition != Structure.Condition.Uninteractable) {
            return structure.Interact(controller);
        }
        return false;
    }
}
