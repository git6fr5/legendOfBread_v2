/* --- Libraries --- */
using System.Collections;
using System.Collections.Generic;
using UnityEngine;

/* --- Enumerations --- */
using Action = State.Action;
using Orientation = Compass.Orientation;

/// <summary>
/// Any item that is equippable.
/// </summary>
public class Item : MonoBehaviour {

    /* --- Variables --- */
    [SerializeField] public Action action;
    [Range(0.025f, 2f)] public float actionBuffer = 0.4f;
    [HideInInspector] protected float timeInterval = 0f;
    [Range(0f, 5f)] public float moveSpeed = 1f;
    [SerializeField] protected bool isActive;

    /* --- Variables --- */
    [Range(0, 5)] public int damage;

    /* --- Unity --- */
    // Runs once every frame.
    void Update() {
        if (isActive) {
            Act();
        }
    }

    protected virtual void Act() {

    }

    /* --- Methods --- */
    public Action Activate(Controller controller) {
        isActive = OnActivate(controller);
        if (isActive) {
            transform.localRotation = Compass.OrientationAngles[controller.state.orientation];
            controller.state.activeItem = this;
            timeInterval = 0f;
            return action;
        }
        return Action.Inactive;
    }

    protected virtual bool OnActivate(Controller controller) {
        return true;
    }

    protected virtual void OnDeactivate() {
    }

    public Action Deactivate() {
        OnDeactivate();
        isActive = false;
        return Action.Inactive;
    }

    public void SetRotation() {
    }

    public void OnHit(Hurtbox hurtbox) {
        hurtbox.controller.Hurt(damage);
    }
}
