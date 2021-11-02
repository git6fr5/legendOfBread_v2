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

        map.mapData.doorSwitch = (Map.Switch)(((int)map.mapData.doorSwitch + 1) % (int)Map.Switch.Count);
        map.minimap.Refresh(map.location, map.mapData, map.height, Map.DefaultGridSize, Map.DoorGridSize);

        print("hello");
        return true;
        
    }

}
