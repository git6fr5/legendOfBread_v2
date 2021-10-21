/* --- Libraries --- */
using System.Collections;
using System.Collections.Generic;
using UnityEngine;

/* --- Structs --- */
using Rotation = Loader.Rotation;

public class Fulcrum : Entity {

    public Transform arms;

    public void Attach(Attachable attachable) {
        attachable.transform.parent = arms;
        print("Attached to fulcrum");
    }

    public override void ApplyRotation(Level level, Rotation rotation) {
        // 
    }

}
