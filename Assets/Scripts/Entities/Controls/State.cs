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

    [HideInInspector] public Equipable activeItem = null;
    public List<Equipable> equipment = new List<Equipable>();

    // Movement Flags
    public enum Movement {
        Idle,
        Moving
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
            activeItem.Activate(orientation);
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
        yield return new WaitForSeconds(buffer);
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
