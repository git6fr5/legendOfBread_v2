using System.Collections;
using System.Collections.Generic;
using UnityEngine;

public class Crush : Trap {

    /* --- Constructor --- */
    public Crush() {
        id = 1;
    }

    /* --- Components --- */
    public Vision vision;
    public AnimationCurve curve;

    /* --- Control Variables --- */
    [Range(0, 5)] public int damage; // The amount of damage this trap does.
    [HideInInspector] protected Vector3 targetPoint; // The location this slime is travelling to.
    [SerializeField] public float travelDistance = 2.5f; // The distance the trap travels before stopping.
    [SerializeField] protected bool isCharging = false; // Flags if this trap is charging (while on, it can either be charging or withdrawing).
    [Range(0f, 5f)] public float chargeSpeed = 5f; // The speed at which this trap charges.
    [Range(0f, 5f)] protected float onInterval = 2f; // The maximum interval this trap can be on for until it hard resets (for edge cases).
    [HideInInspector] protected float onTicks = 0f; // The duration this trap has been on for.
    
    /* --- Shake Variables --- */
    [Range(0f, 5f)] protected float shakeStrength = 0.05f;
    [Range(0f, 5f)] protected float shakeDuration = 0.5f;
    [Range(0f, 5f)] protected float elapsedTime = 0f;

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
            button = BUTTON.POWER_UP;
        }
        else {
            elapsedTime = 0f;
        }
    }

    protected override void PowerUp() {
        elapsedTime += Time.deltaTime;
        if (elapsedTime >= shakeDuration) {
            elapsedTime = 0f;
            isCharging = true;
            onTicks = 0f;
            Action();
            button = BUTTON.ON;
            return;
        }
        float strength = shakeStrength * curve.Evaluate(elapsedTime / shakeDuration);
        transform.position = (Vector3)(origin + Random.insideUnitCircle * shakeStrength);
    }

    protected override void On() {

        // Activate the action.

        moveSpeed = state.baseSpeed * chargeSpeed;
        movementVector = targetPoint - transform.position;

        onTicks += Time.deltaTime;

        if (onTicks >= onInterval) {
            button = BUTTON.POWER_DOWN;
        }
        if (Vector2.Distance(transform.position, targetPoint) < GameRules.movementPrecision) {
            movementVector = Vector2.zero;
            transform.position = transform.position;
        }
    }

    protected override void PowerDown() {

        moveSpeed = state.baseSpeed;
        movementVector = origin - (Vector2)transform.position;

        if (Vector2.Distance(transform.position, origin) < GameRules.movementPrecision) {
            transform.position = origin;
            button = BUTTON.OFF;
        }

    }


    /* --- Event Actions --- */
    protected override void OnHit(Hurtbox hurtbox) {
        // This is now done by the spikes.
        hurtbox.controller.Hurt(damage); 
    }

}
