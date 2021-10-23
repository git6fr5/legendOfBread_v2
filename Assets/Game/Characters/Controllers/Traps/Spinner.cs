using System.Collections;
using System.Collections.Generic;
using UnityEngine;

public class Spinner : Trap {

    /* --- Constructor --- */
    public Spinner() {
        id = 2;
    }

    public float rotationSpeed;
    public int damage;
    public Transform arms;

    protected override void On() {
        float deltaRotation = rotationSpeed * Time.fixedDeltaTime;
        arms.eulerAngles = arms.eulerAngles + Vector3.forward * deltaRotation;

        foreach (Transform child in arms) {
            child.eulerAngles = Vector3.zero;
        }

    }

    /* --- Event Actions --- */
    protected override void OnHit(Hurtbox hurtbox) {
        hurtbox.controller.Hurt(damage);
    }

}
