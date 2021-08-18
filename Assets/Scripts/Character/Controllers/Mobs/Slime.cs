using System.Collections;
using System.Collections.Generic;
using UnityEngine;

public class Slime : Mob {

    /* --- Constructor --- */
    public Slime() {
        id = 1;
    }

    /* --- Components --- */
    public Slime childSlime;
    public Slime parentSlime;
    [Range(0F, 5F)] public double damage;

    /* --- Variables --- */
    Vector3 targetPoint;
    float idleDistance = 0.75f;
    float idleTicks = 0f;
    float idleInterval = 1f;

    /* --- Action Flow --- */
    protected override void Idle() {
        // Look for a target, but otherwise move randomly
        Hurtbox target = vision.LookFor(GameRules.playerTag);
        if (vision.LookFor(GameRules.playerTag) != null) {
            moveSpeed = state.baseSpeed;
            targetPoint = target.transform.position;
        }
        else {
            idleTicks += Time.deltaTime;
            if (idleTicks >= idleInterval || targetPoint == Vector3.zero) {
                targetPoint = idleDistance * Random.insideUnitCircle + (Vector2)transform.position;
                idleTicks = 0f;
            }
        }
        movementVector = targetPoint - transform.position;
        if (movementVector.magnitude < GameRules.movementPrecision) {
            movementVector = Vector2.zero;
        }
        else {
            orientationVector = new Vector2(Mathf.Sign(movementVector.x), 0);
        }
    }
    
    /* --- Event Actions --- */
    protected override void OnHit(Hurtbox hurtbox) {
        hurtbox.controller.Hurt(damage);
    }

    protected override void OnDeath() {
        // Load a child slime
    }
}
