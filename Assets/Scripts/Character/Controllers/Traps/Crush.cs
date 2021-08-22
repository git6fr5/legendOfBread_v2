using System.Collections;
using System.Collections.Generic;
using UnityEngine;

public class Crush : Trap {

    /* --- Constructor --- */
    public Crush() {
        id = 1;
    }

    public Vision vision;
    public int damage;

    Vector3 targetPoint;
    float travelDistance = 3f;
    bool isCharging = false;
    float onSpeed = 5f;
    float onTicks = 0f;
    float onMaxTime = 5f;

    protected override void Off() {
        // Look for a target, otherwise do nothing
        Hurtbox target = vision.LookFor(GameRules.playerTag);
        if (vision.LookFor(GameRules.playerTag) != null) {
            targetPoint = target.transform.position - (Vector3)origin;
            if (Mathf.Abs(targetPoint.x) >= Mathf.Abs(targetPoint.y)) {
                targetPoint = (Vector3)origin + travelDistance * new Vector3(Mathf.Sign(targetPoint.x), 0);
            }
            else {
                targetPoint = (Vector3)origin + travelDistance * new Vector3(0, Mathf.Sign(targetPoint.y));
            }
            isCharging = true;
            button = BUTTON.ON;
            onTicks = 0f;
        }
    }

    protected override void On() {
        //

        movementVector = targetPoint - transform.position;
        onTicks += Time.deltaTime;
        if (onTicks >= onMaxTime) {
            onTicks = 0f;
            transform.position = origin;
            button = BUTTON.OFF;
        }
        if (isCharging) {
            moveSpeed = state.baseSpeed * onSpeed;
            if (Vector2.Distance(transform.position, targetPoint) < GameRules.movementPrecision) {
                targetPoint = origin;
                isCharging = false;
            }
        }
        else {
            if (Vector2.Distance(transform.position, origin) < GameRules.movementPrecision) {
                transform.position = origin;
                button = BUTTON.OFF;
            }
        }
    }

    /* --- Event Actions --- */
    protected override void OnHit(Hurtbox hurtbox) {
        hurtbox.controller.Hurt(damage);
    }

}
