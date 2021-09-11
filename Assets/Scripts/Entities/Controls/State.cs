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

    [HideInInspector] public Equipable activeItem = null;
    public List<Equipable> equipment = new List<Equipable>();

    // State Flags.
    public ORIENTATION orientation;
    public bool isMoving;
    public bool isHurt;
    public bool isDead;
    public bool isActive;
    // Action Enumerations
    public enum ActionState {
        Inactive,
        // Timed Actions.
        Attacking,
        Jumping,
        Throwing,
        timedActionCount,
        // Continous Actions.
        Sliding,
        Talking,
        Carrying,
        continiousActionCount
    }
    public ActionState actionState = ActionState.Inactive;

    // Timed Action Buffer
    [HideInInspector] Coroutine actionTimer;
    [HideInInspector] public float actionBuffer;

    // Timed State Buffers
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
        ActionFlag();
        HurtFlag();
        DeathFlag();
    }

    /* --- Flags --- */
    // Flags if this state is performing an action
    void ActionFlag() {
        if (activeItem != null && actionTimer == null) {
            activeItem.Activate(orientation);
            float buffer = activeItem.actionBuffer;
            actionTimer = StartCoroutine(IEActionFlag(buffer));
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
    // Unflags this state as doing an action
    IEnumerator IEActionFlag(float buffer) {
        yield return new WaitForSeconds(buffer);
        activeItem.Deactivate();
        activeItem = null;
        actionTimer = null;
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
