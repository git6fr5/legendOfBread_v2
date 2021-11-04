using System.Collections;
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

    private float targetTicks = 0f;
    private float maxTargetTicks = 100f;

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
        bool shuffle = false;
        if (target != null) {
            targetPoint = target.transform.position;
            if ((target.transform.position - transform.position).magnitude < attackRadius) {
                if (canAttack) {
                    Action();
                }
            }
            if ((target.transform.position - transform.position).magnitude < attackRadius * 0.75f) {
                shuffle = true;
            }
            targetTicks += Time.deltaTime;
            targetTicks = targetTicks % maxTargetTicks;
        }
        else {
            idleTicks += Time.deltaTime;
            if (idleTicks >= idleInterval || targetPoint == Vector3.zero) {
                targetPoint = idleDistance * Random.insideUnitCircle + (Vector2)transform.position;
                idleTicks = 0f;
            }
        }

        moveSpeed *= state.activeItem ? state.activeItem.moveSpeed : 1f;
        movementVector = targetPoint - transform.position;
        // movementVector = raycast.AdjustMovement(movementVector);

        if (movementVector.magnitude < GameRules.movementPrecision) {
            movementVector = Vector2.zero;
        }
        else {
            orientationVector = Compass.SnapVector(movementVector);
        }

        movementVector = (canAttack && shuffle) ? movementVector : -1f * new Vector2(-movementVector.y, movementVector.x);

        Vector2 offset = new Vector2(0.5f * Mathf.Cos(3f * targetTicks), 0.5f * Mathf.Sin(3f * targetTicks));
        Vector2 origin = transform.position + (Vector3)offset;
        Collider2D targetCollider = (target != null) ? target.controller.mesh.frame : null;
        movementVector = Raycast.SmartPath(origin, movementVector, mesh.frame, targetCollider, 0, 3);

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

    protected override void OnDeath() {
        DropSkrit();
    }

}
