using System.Collections;
using System.Collections.Generic;
using UnityEngine;

public class Lever : Structure {

    public Map map;

    void Awake() {
        interactAction = State.Action.Pushing;
    }

    /* --- Overridden Methods --- */
    public override bool Interact(Controller controller) {

        map.FlipSwitch();
        map.LoadMinimap();

        print("hello");
        return true;
        
    }

}
