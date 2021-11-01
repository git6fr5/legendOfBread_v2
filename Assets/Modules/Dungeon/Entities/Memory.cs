using System.Collections;
using System.Collections.Generic;
using UnityEngine;

public class Memory : Entity {

    public Vector2 direction;

    public override void ApplyDirection(Room room, Vector2Int gridPosition, Vector2Int direction) {
        this.direction = (Vector2)direction;
    }

}
