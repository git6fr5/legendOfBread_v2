using System.Collections;
using System.Collections.Generic;
using UnityEngine;

public class Lever : Structure {

    public Map map;

    public Sprite on;
    public Sprite off;

    void Awake() {
        interactAction = State.Action.Pushing;

    }

    /* --- Overridden Methods --- */
    protected override void Interactable() {
        if (map.mapData.doorSwitch == Map.Switch.On) {
            mesh.spriteRenderer.sprite = on;
        }
        else {
            mesh.spriteRenderer.sprite = off;
        }
    }

    public override bool Interact(Controller controller) {

        map.mapData.doorSwitch = (Map.Switch)(((int)map.mapData.doorSwitch + 1) % (int)Map.Switch.Count);
        print(map.mapData.doorSwitch);
        map.minimap.Refresh(map.location, map.mapData, map.height, Map.DefaultGridSize, Map.DoorGridSize);
        return true;
        
    }

}
