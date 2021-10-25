using System.Collections;
using System.Collections.Generic;
using UnityEngine;

/* --- Structs --- */
using Direction = Loader.Direction;


public class Memory : Entity {

    public Vector2 direction;

    public override void ApplyDirection(Level level, Direction direction) {
        this.direction = (Vector2)direction.direction;
    }

}
