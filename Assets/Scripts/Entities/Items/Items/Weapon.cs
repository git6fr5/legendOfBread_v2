using System.Collections;
using System.Collections.Generic;
using UnityEngine;

using ORIENTATION = Compass.ORIENTATION;
using ActionState = State.ActionState;

public class Weapon : Equipable {

    public Hitbox hitbox;

    void Awake() {
        action = ActionState.Attacking;
    }

    protected override void Act(float timeInterval) {
        AdjustHitbox(timeInterval);
    }

    protected override void OnActivate() {
        hitbox.gameObject.SetActive(true);
        hitbox.Reset();
    }

    protected override void OnDeactivate() {
        hitbox.gameObject.SetActive(false);
    }

    void AdjustHitbox(float timeInterval) {
        int index = ((int)Mathf.Floor(timeInterval * effect.effect.Length / actionBuffer) % effect.effect.Length); ;
        hitbox.transform.localRotation = Quaternion.Euler(0, 0, -(index * 180 / (effect.effect.Length - 1)));
    }

}
