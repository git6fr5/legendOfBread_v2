using System.Collections;
using System.Collections.Generic;
using UnityEngine;

public class Slidable : Structure {

    public List<Controller> prevContainer;

    void Awake() {
        mesh.frame.enabled = false;
    }

    void Update() {
        
    }

}
