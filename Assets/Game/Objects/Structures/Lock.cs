using System.Collections;
using System.Collections.Generic;
using UnityEngine;

public class Lock : Structure {

    public Exit exit;

    void Awake() {
        interactAction = State.Action.Pushing;
    }

    /* --- Overridden Methods --- */
    public override bool Interact(Controller controller) {


        if (controller.GetComponent<Player>()) {
            Player player = controller.GetComponent<Player>();

            if (player.numKeys > 0) {
                player.numKeys -= 1;
                exit.isLocked = false;

                exit.map.unlockedExits.Add(exit.index);

            }
        }

        return true;

    }

}
