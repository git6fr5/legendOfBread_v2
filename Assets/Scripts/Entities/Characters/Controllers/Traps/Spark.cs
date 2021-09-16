using System.Collections;
using System.Collections.Generic;
using UnityEngine;

using DIRECTION = Compass.DIRECTION;

public class Spark : Trap
{

    /* --- Constructor --- */
    public Spark() {
        id = 3;
    }

    public int damage;

    protected override void Off() {
        
    }

    /* --- Event Actions --- */
    protected override void OnHit(Hurtbox hurtbox) {
        hurtbox.controller.Hurt(damage);
    }

}
