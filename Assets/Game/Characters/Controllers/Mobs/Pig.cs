﻿using System.Collections;
using System.Collections.Generic;
using UnityEngine;

public class Pig : Mob {

    /* --- Control Variables --- */
    [HideInInspector] protected Vector3 targetPoint; // The location this slime is travelling to.
    [SerializeField] protected float idleDistance = 3f; // The distance this slime travels during a single idle interval.
    [SerializeField] protected float idleInterval = 5f; // The duration of a single idle interval.
    [SerializeField] protected float idleTicks = 0f; // How long into an idle interval this slime is.

    [SerializeField] bool canAttack = false;
    [SerializeField] float attackRadius = 3f;
    [SerializeField] float attackInterval = 3f;

    public Raycast raycast;

    /* --- Unity --- */
    // Runs once before the first frame.
    void Start() {
        // Set these parameters.
        orientationVector = Vector2.right;
        canAttack = false;
        StartCoroutine(IEAttackCooldown(attackInterval));

        movementVector = Vector2.right;
    }

    /* --- Action Flow --- */
    protected override void Idle() {

        // Look for a target, but otherwise move randomly
        Hurtbox target = vision.LookFor(GameRules.playerTag);
        if (vision.LookFor(GameRules.playerTag) != null) {
            targetPoint = target.transform.position;
            if ((target.transform.position - transform.position).magnitude < attackRadius) {
                if (canAttack) {
                    Action();
                }
            }
        }
        else {
            idleTicks += Time.deltaTime;
            if (idleTicks >= idleInterval || targetPoint == Vector3.zero) {
                targetPoint = idleDistance * Random.insideUnitCircle + (Vector2)transform.position;
                idleTicks = 0f;
            }
        }

        moveSpeed *= state.activeItem ? state.activeItem.moveSpeed : 1f;
        moveSpeed *= canAttack ? 1f : -1;
        movementVector = targetPoint - transform.position;

        if (movementVector.magnitude < GameRules.movementPrecision) {
            movementVector = Vector2.zero;
        }
        else {
            orientationVector = Compass.SnapVector(movementVector);
        }

        movementVector = canAttack ? movementVector : new Vector2(-movementVector.y, movementVector.x);

        Debug.DrawRay(transform.position, movementVector, Color.green);

    }

    private Vector2 NewTargetPoint() {

        int index = Random.Range(0, (int)Compass.Orientation.count);

        // Check through the indices.
        bool foundValidIndex = false;
        for (int i = 0; i < (int)Compass.Orientation.count; i++) {
            index = (index + i) % ((int)Compass.Orientation.count);
            if (raycast.castCollisions.Contains(index)) {
                print("There is something in the way");
            }
            else {
                foundValidIndex = true;
                break;
            }
        }

        if (!foundValidIndex) {
            return (Vector2)transform.position;
        }
        else {
            Vector2 direction = Compass.OrientationVectors[(Compass.Orientation)index];
            return idleDistance * direction + (Vector2)transform.position;
        }
        

    }

    private Vector2 MoveAround(Vector2 targetMovementVector) {
        for (int i = 0; i < (int)Compass.Orientation.count; i++) {
            int index = ((int)Compass.VectorOrientations[targetMovementVector] + i) % ((int)Compass.Orientation.count);
            if (raycast.castCollisions.Contains(index)) {
                print("There is something in the way");
            }
            else {
                return Compass.OrientationVectors[(Compass.Orientation)index];
            }
        }
        return Vector2.zero;
    }

    protected override void OnAction(int index = 0) {
        canAttack = false;
        float cooldown = attackInterval;
        cooldown += state.activeItem ? state.activeItem.actionBuffer : 0.5f;
        StartCoroutine(IEAttackCooldown(cooldown));
    }

    IEnumerator IEAttackCooldown(float delay) {
        yield return new WaitForSeconds(delay);
        canAttack = true;
        yield return null;
    }

}
