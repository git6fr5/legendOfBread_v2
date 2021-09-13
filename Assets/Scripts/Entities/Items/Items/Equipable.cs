using System.Collections;
using System.Collections.Generic;
using UnityEngine;

using Action = State.Action;
using Orientation = Compass.ORIENTATION;

public class Equipable : MonoBehaviour {

    /* --- Components --- */
    public Particle effect;

    [Range(0.025f, 2f)] public float actionBuffer = 0.4f;
    [SerializeField] protected float timeInterval = 0f;

    public Action action;
    public bool isActive;

    /* --- Variables --- */
    [Range(0, 5)] public int damage;

    void Update() {
        if (isActive) {
            Act(effect.timeInterval);
        }
    }

    protected virtual void Act(float timeInterval) {

    }

    /* --- Methods --- */
    public Action Activate(Orientation orientation) {
        OnActivate();
        isActive = true;
        timeInterval = 0f;
        effect.ControlledActivate(actionBuffer);
        transform.localRotation = Compass.OrientationAngles[orientation];
        return action;
    }
    
    protected virtual void OnActivate() {

    }

    protected virtual void OnDeactivate() {
    }

    public Action Deactivate() {
        OnDeactivate();
        isActive = false;
        effect.Activate(false);
        return Action.Inactive;
    }

    public void SetRotation() {
    }

    public void OnHit(Hurtbox hurtbox) {
        hurtbox.controller.Hurt(damage);
    }
}
