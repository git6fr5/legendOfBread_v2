using System.Collections;
using System.Collections.Generic;
using UnityEngine;

public class Spark : Trap {

    public int damage;
    public Raycast raycast;

    public int memory;

    protected override void Off() {

    }

    protected override void On() {
        print("On");

        int index = -1;

        // Check through the indices.
        for (int i = 0; i < (int)Compass.Orientation.count; i++) {
            int _index = (i) % ((int)Compass.Orientation.count);
            // Find where the wall is.
            if (raycast.castCollisions.Contains(_index)) {
                index = _index;
                break;
            }
        }

        if (index == 1 || index == 3) {
            memory = index;
        }

        if (index == -1) {
            print("floating");
            index = memory;
        }

        print(index);
        int rotate = ( (index - 1) % (int)Compass.Orientation.count );
        if (rotate < 0) {
            rotate = (int)Compass.Orientation.count + rotate;
        }

        if (raycast.castCollisions.Contains(rotate)) {
            rotate = ((rotate - 1) % (int)Compass.Orientation.count);
        }

        print(rotate);
        Vector2 direction = Compass.OrientationVectors[(Compass.Orientation)rotate];
        movementVector = direction;

    }

    /* --- Event Actions --- */
    protected override void OnHit(Hurtbox hurtbox) {
        hurtbox.controller.Hurt(damage);
        Destroy(gameObject);
    }

}
