﻿using System.Collections;
using System.Collections.Generic;
using UnityEngine;

public class Slime : Mob {

    /* --- Constructor --- */
    public Slime() {
        id = 1;
    }

    /* --- Controls --- */
    public Slime childSlime;
    public Slime parentSlime;
    public GameObject trailObject;
    [Range(0, 5)] public int damage;

    /* --- Variables --- */
    public bool isChild;
    float aliveInterval = 0f;
    float growTime = 3f;
    Vector3 targetPoint;
    float idleDistance = 0.75f;
    float idleTicks = 0f;
    float idleInterval = 1f;
    float trailTime;

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
            Janitor.LoadNewController(childSlime, transform.position);
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
        trailTime += Time.deltaTime;
        if (trailTime >= 0.5f) {
            trailTime = 0f;
            Vector3 offset = new Vector3(0, -0.2f, 0);
            GameObject newTrailObject = Instantiate(trailObject, transform.position + offset, Quaternion.identity, GameObject.FindWithTag(GameRules.roomTag)?.transform);
            newTrailObject.SetActive(true);
        }
    }

}