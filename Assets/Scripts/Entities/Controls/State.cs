using System.Collections;
using System.Collections.Generic;
using UnityEngine;
using ORIENTATION = Compass.ORIENTATION;

public class State : MonoBehaviour {

    /* --- Variables --- */
    // Stats
    [Range(0, 10)] public int maxHealth;
    [SerializeField] public int health; // The health 
    [Range(0f, 10f)] public float height; // The height of the floor this entity is at.
    [Range(0.05f, 20f)] public float baseSpeed; // How fast the entity moves.
    [Range(0.05f, 20f)] public float jumpPulse; // How high the entity jumps.

    // State Flags.
    public ORIENTATION orientation;
    public bool isMoving;
    public bool isSliding;
    public bool isAttacking;
    public bool isTalking;
    public bool isCarrying;
    public bool isThrowing;
    public bool isJumping;
    public bool isFalling;
    public bool isHurt;
    public bool isDead;

    // Timers.
    [HideInInspector] Coroutine attackTimer;
    [Range(0.05f, 1f)] public float attackBuffer = 0.4f;
    [HideInInspector] Coroutine throwTimer;
    [Range(0.05f, 1f)] public float throwBuffer = 0.2f;
    [HideInInspector] Coroutine hurtTimer;
    [Range(0.05f, 1f)] public float hurtBuffer = 0.4f; // The interval an entity becomes invulnerable after being hurt.
    [HideInInspector] Coroutine deathTimer;
    [Range(0.05f, 1f)] public float deathBuffer = 0.6f; // The interval between dying and despawning.

    /* --- Unity --- */
    // Runs once on instantiation.
    void Awake() {
        // Set the required state..
        health = maxHealth;
    }

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
        if (isAttacking && attackTimer == null) {
            attackTimer = StartCoroutine(IEAttackFlag(attackBuffer));
        }
    }

    // Flags if this state is throwing
    void ThrowFlag() {
        if (isThrowing && throwTimer == null) {
            throwTimer = StartCoroutine(IEThrowFlag(throwBuffer));
        }
    }

    // Flags if this state is hurt
    void HurtFlag() {
        if (isHurt && hurtTimer == null) {
            hurtTimer = StartCoroutine(IEHurtFlag(hurtBuffer));
        }
    }

    // Flags if this state is hurt
    void DeathFlag() {
        if (isDead && deathTimer == null) {
            deathTimer = StartCoroutine(IEDeathFlag(deathBuffer));
        }
    }

    /* --- Coroutines --- */
    // Unflags this state as attacking
    IEnumerator IEAttackFlag(float buffer) {
        yield return new WaitForSeconds(buffer);
        isAttacking = false;
        attackTimer = null;
        yield return null;
    }

    // Unflags this state as throwing
    IEnumerator IEThrowFlag(float buffer) {
        yield return new WaitForSeconds(buffer);
        isThrowing = false;
        throwTimer = null;
        yield return null;
    }

    // Unflags this state as being hurt
    IEnumerator IEHurtFlag(float buffer) {
        yield return new WaitForSeconds(buffer);
        isHurt = false;
        hurtTimer = null;
        yield return null;
    }

    // Deactivates the Game Object
    IEnumerator IEDeathFlag(float buffer) {
        yield return new WaitForSeconds(buffer);
        gameObject.SetActive(false);
        yield return null;
    }

}
