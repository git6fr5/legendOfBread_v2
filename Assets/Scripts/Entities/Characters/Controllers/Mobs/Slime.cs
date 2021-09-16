using System.Collections;
using System.Collections.Generic;
using UnityEngine;

public class Slime : Mob {

    /* --- Constructor --- */
    public Slime() {
        id = 1;
    }

    /* --- Spawning Variables --- */
    [SerializeField] protected Slime childSlime;
    [SerializeField] protected Slime parentSlime;
    [SerializeField] protected bool isChild; // Flags if this slime is a child.
    [SerializeField] protected float growTime = 3f; // The time it takes for a child slime to grow into an adult.
    [HideInInspector] protected float aliveInterval = 0f; // The time that this slime has been alive for.

    /* --- Control Variables --- */
    [Range(0, 5)] public int damage; // The amount of damage this slime does.
    [HideInInspector] protected Vector3 targetPoint; // The location this slime is travelling to.
    [SerializeField] protected float idleDistance = 0.75f; // The distance this slime travels during a single idle interval.
    [SerializeField] protected float idleInterval = 1f; // The duration of a single idle interval.
    [HideInInspector] protected float idleTicks = 0f; // How long into an idle interval this slime is.

    /* --- Trail Variables --- */
    [SerializeField] protected GameObject trailObject;
    [SerializeField] protected float trailInterval; // The interval between leaving a trail.

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
        if (isChild) {
            aliveInterval += Time.deltaTime;
            if (aliveInterval >= growTime) {
                Grow();
            }
        }
        Trail();
    }
    
    /* --- Event Actions --- */
    protected override void OnHit(Hurtbox hurtbox) {
        hurtbox.controller.Hurt(damage);
    }

    protected override void OnDeath() {
        if (!isChild) {
            Split();
        }
    }

    /* --- Methods --- */
    // Splits into two child slimes
    void Split() {
        for (int i = 0; i < 2; i++) {
            Controller newSlime = Janitor.LoadNewController(childSlime, transform.position);
            newSlime.state.vitality = State.Vitality.Hurt;
        }
        Destroy(gameObject);
    }

    // Grows from a child slime to a parent slime.
    void Grow() {
        Janitor.LoadNewController(parentSlime, transform.position);
        Destroy(gameObject);
    }

    // Leaves a trail of goo behind.
    void Trail() {
        trailInterval += Time.deltaTime;
        if (trailInterval >= 0.5f) {
            trailInterval = 0f;
            Vector3 offset = new Vector3(0, -0.2f, 0);
            GameObject newTrailObject = Instantiate(trailObject, transform.position + offset, Quaternion.identity, GameObject.FindWithTag(GameRules.roomTag)?.transform);
            newTrailObject.SetActive(true);
        }
    }

}
