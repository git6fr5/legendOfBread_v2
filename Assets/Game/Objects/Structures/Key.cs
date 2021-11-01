using System.Collections;
using System.Collections.Generic;
using UnityEngine;

public class Key : Structure {

    void Awake() {
        interactAction = State.Action.Pushing;
    }

    /* --- Overridden Methods --- */
    public override bool Interact(Controller controller) {

        
        if (controller.GetComponent<Player>()) {
            print("hello");
            Player player = controller.GetComponent<Player>();
            // player.keys.Add(this);
            player.numKeys += 1;
            gameObject.SetActive(false);
        }

        return true;

    }

}
