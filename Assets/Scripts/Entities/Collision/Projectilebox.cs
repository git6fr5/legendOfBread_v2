using System.Collections;
using System.Collections.Generic;
using UnityEngine;

public class Projectilebox : Hitbox {

    public Projectile projectile;

    protected override void OnAdd(Hurtbox hurtbox) {
        projectile.Hit(hurtbox);
    }

}
