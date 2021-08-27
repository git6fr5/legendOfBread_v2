using System.Collections;
using System.Collections.Generic;
using UnityEngine;
using ORIENTATION = Compass.ORIENTATION;

public class State : MonoBehaviour {

    /* --- Variables --- */
    // Health
    [Range(0, 10)] public int maxHealth;
    public int health;

    // Stats
    [Range(0f, 5f)] public float baseSpeed;

    // Switches
    public ORIENTATION orientation;
    public bool isMoving;
    public bool isAttacking;
    public bool isTalking;
    public bool isCarrying;
    public bool isThrowing;
    public bool isHurt;
    public bool isDead;

    // Flags
    Coroutine attackFlag;
    public float attackBuffer = 0.4f;
    Coroutine throwFlag;
    public float throwBuffer = 0.2f;
    Coroutine hurtFlag;
    public float hurtBuffer = 0.4f;
    Coroutine deathFlag;
    public float deathBuffer = 0.6f;

    /* --- Unity --- */
    // Runs every frame.
    void Update() {
        AttackFlag();
        ThrowFlag();
        HurtFlag();
        DeathFlag();
    }

    /* --- Flags --- */
    // Flags if this state is attacking
    void AttackFlag() {
        if (isAttacking && attackFlag == null) {
            attackFlag = StartCoroutine(IEAttackFlag(attackBuffer));
        }
    }

    // Flags if this state is throwing
    void ThrowFlag() {
        if (isThrowing && throwFlag == null) {
            throwFlag = StartCoroutine(IEThrowFlag(throwBuffer));
        }
    }

    // Flags if this state is hurt
    void HurtFlag() {
        if (isHurt && hurtFlag == null) {
            hurtFlag = StartCoroutine(IEHurtFlag(hurtBuffer));
        }
    }

    // Flags if this state is hurt
    void DeathFlag() {
        if (isDead && deathFlag == null) {
            deathFlag = StartCoroutine(IEDeathFlag(deathBuffer));
        }
    }

    /* --- Coroutines --- */
    // Unflags this state as attacking
    IEnumerator IEAttackFlag(float buffer) {
        yield return new WaitForSeconds(buffer);
        isAttacking = false;
        attackFlag = null;
        yield return null;
    }

    // Unflags this state as throwing
    IEnumerator IEThrowFlag(float buffer) {
        yield return new WaitForSeconds(buffer);
        isThrowing = false;
        throwFlag = null;
        yield return null;
    }

    // Unflags this state as being hurt
    IEnumerator IEHurtFlag(float buffer) {
        yield return new WaitForSeconds(buffer);
        isHurt = false;
        hurtFlag = null;
        yield return null;
    }

    // Deactivates the Game Object
    IEnumerator IEDeathFlag(float buffer) {
        yield return new WaitForSeconds(buffer);
        gameObject.SetActive(false);
        yield return null;
    }

}
