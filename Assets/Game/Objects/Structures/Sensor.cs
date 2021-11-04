using System.Collections;
using System.Collections.Generic;
using UnityEngine;

public class Sensor : Block {

    public bool isActivated = false;
    protected override void OnUpdate() {
        if (group.Count != 0) {
            isActivated = true;
        }
        else {
            isActivated = false;
        }
    }

}
