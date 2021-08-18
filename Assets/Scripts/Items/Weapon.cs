using System.Collections;
using System.Collections.Generic;
using UnityEngine;

using ORIENTATION = Compass.ORIENTATION;

public class Weapon : Item {
    
    /* --- Components --- */
    public State state;
    public Blade blade;
    public Hitbox hitbox;

    /* --- Variables --- */
    [Range(0F, 5F)] public double damage;

    /* --- Unity --- */
    void Awake() {
    }

    void Update() {
        if (!state.isAttacking) {
            Activate(false);
        }
    }

    /* --- Methods --- */
    public void Activate(bool active) {
        gameObject.SetActive(active);
        SetRotation();
        hitbox.Reset();
    }

    public void SetRotation() {
        transform.localRotation = Compass.OrientationAngles[state.orientation];
    }

    public void OnHit(Hurtbox hurtbox) {
        hurtbox.controller.Hurt(damage);
    }

}
