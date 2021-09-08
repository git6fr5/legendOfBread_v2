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

    public Weapon spike;

    Vector3 targetPoint;
    [HideInInspector] public float travelDistance = 2.5f;
    bool isCharging = false;
    float onSpeed = 5f;
    float onTicks = 0f;
    float onMaxTime = 5f;

    [Range(0f, 5f)] public float shakeStrength = 0.05f;
    public float shakeDuration = 0.5f;
    float elapsedTime = 0f;
    public AnimationCurve curve;

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
            orientationVector = ((Vector2)targetPoint - origin).normalized;
            // Shake until it's ready to launch.
            Shake();
        }
        else {
            elapsedTime = 0f;
        }
    }

    protected override void On() {
        //
        Attack();

        movementVector = targetPoint - transform.position;
        onTicks += Time.deltaTime;
        if (onTicks >= onMaxTime) {
            targetPoint = origin;
            spike.Activate(false);
            isCharging = false;
        }
        if (isCharging) {
            moveSpeed = state.baseSpeed * onSpeed;
            if (Vector2.Distance(transform.position, targetPoint) < GameRules.movementPrecision) {
                targetPoint = origin;
                spike.Activate(false);
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
    void Shake() {
        elapsedTime += Time.deltaTime;
        if (elapsedTime >= shakeDuration) {
            elapsedTime = 0f;
            TurnOn();
            return;
        }
        float strength = shakeStrength * curve.Evaluate(elapsedTime / shakeDuration);
        transform.position = (Vector3)(origin + Random.insideUnitCircle * shakeStrength);
    }

    void TurnOn() {
        isCharging = true;
        button = BUTTON.ON;
        onTicks = 0f;
    }

    /* --- Event Actions --- */
    protected override void OnHit(Hurtbox hurtbox) {
        // This is now done by the spikes.
        hurtbox.controller.Hurt(damage); 
    }

    protected override void OnAttack() {
        spike.Activate(true);
    }

}
