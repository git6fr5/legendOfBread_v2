/* --- Libraries --- */
using System.Collections;
using System.Collections.Generic;
using UnityEngine;

public class Fulcrum : Entity {

    public Transform arms;

    public void Attach(Attachable attachable) {
        attachable.transform.parent = arms;
        print("Attached to fulcrum");
    }

    public override void ApplyRotation(Room room, Vector2Int gridPosition, int rotation) {
        // 
    }

}
