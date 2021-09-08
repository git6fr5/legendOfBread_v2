using System.Collections;
using System.Collections.Generic;
using UnityEngine;

using ORIENTATION = Compass.ORIENTATION;

public class Weapon : Item {
    
    /* --- Components --- */
    public State state;
    public Blade blade;
    public Spike spike;
    public Hitbox hitbox;

    /* --- Variables --- */
    [Range(0, 5)] public int damage;

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
        if (blade) { blade.GetComponent<SpriteRenderer>().sprite = blade.swing[0]; }
        else if (spike) { spike.GetComponent<SpriteRenderer>().sprite = spike.swing[0]; }
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
