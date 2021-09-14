using System.Collections;
using System.Collections.Generic;
using UnityEngine;
using Orientation = Compass.ORIENTATION;

public class State : MonoBehaviour {

    /* --- Variables --- */
    // Stats
    [Range(0, 10)] public int maxHealth;
    [SerializeField] public int health; // The health 
    [Range(0f, 10f)] public float height; // The height of the floor this entity is at.
    [Range(0.05f, 20f)] public float baseSpeed; // How fast the entity moves.
    [Range(0.05f, 20f)] public float jumpPulse; // How high the entity jumps.

    public List<Equipable> equipment = new List<Equipable>();
    [HideInInspector] public Equipable activeItem = null;
    public Throwable carryingStructure = null;

    // Movement Flags
    public enum Movement {
        Idle,
        Moving,
        CarryingIdle,
        CarryingMoving,
        Sliding,
        Talking,
    }

    // Danger Flags(?)
    public enum Vitality {
        Healthy,
        Hurt,
        Dead
    }

    // Action Flags
    public enum Action {
        Inactive,
        Attacking, // Attack etc. as "ing" should be for movement and actions should just be verbs
        Jumping, // Jump
        Carrying, // Carry
        Throwing,
        Pushing,
    }

    public Orientation orientation = Orientation.RIGHT;
    public Movement movement = Movement.Idle;
    public Vitality vitality = Vitality.Healthy;
    public Action action = Action.Inactive;

    // Timed Action Buffer
    [HideInInspector] Coroutine actionTimer;
    [HideInInspector] public float actionBuffer;

    // Timed State Buffers
    [HideInInspector] Coroutine vitalityTimer;
    [Range(0.05f, 1f)] public float vitalityBuffer = 0.4f; // The interval between dying and despawning.

    /* --- Unity --- */
    // Runs once on instantiation.
    void Awake() {
        // Set the required state..
        health = maxHealth;
    }

    // Runs every frame.
    void Update() {
        ActionFlag();
        VitalityFlag();
    }

    /* --- Flags --- */
    // Flags if this state is performing an action
    void ActionFlag() {
        if (activeItem != null && actionTimer == null) {
            float buffer = activeItem.actionBuffer;
            actionTimer = StartCoroutine(IEActionFlag(buffer));
        }
    }

    // Flags if this state is hurt
    void VitalityFlag() {
        if (vitality != Vitality.Healthy && vitalityTimer == null) {
            vitalityTimer = StartCoroutine(IEVitalityTimer(vitalityBuffer));
        }
    }

    /* --- Coroutines --- */
    // Unflags this state as doing an action
    IEnumerator IEActionFlag(float buffer) {
        // This is to adjust for the 1 frame difference.
        yield return new WaitForSeconds(buffer - Time.deltaTime);
        activeItem.Deactivate();
        activeItem = null;
        actionTimer = null;
        yield return null;
    }

    // Unflags this state as being hurt or dead
    IEnumerator IEVitalityTimer(float buffer) {
        yield return new WaitForSeconds(buffer);
        if (vitality == Vitality.Dead) {
            gameObject.SetActive(false);
        }
        else {
            vitality = Vitality.Healthy;
            vitalityTimer = null;
        }
        yield return null;
    }

}
