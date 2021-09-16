/* --- Libraries --- */
using System.Collections;
using System.Collections.Generic;
using UnityEngine;

/* --- Enumerations --- */
using Action = State.Action;

/// <summary>
/// A spike that extends out from a crush trap.
/// </summary>
public class Spike : Item {

    /* --- Components --- */
    public Hitbox hitbox;
    public Particle effect;

    /* --- Unity --- */
    // Runs once on initialisation.
    void Awake() {
        action = Action.Attacking;
    }

    /* --- Overridden --- */
    protected override void Act() {
        AdjustHitbox(effect.timeInterval);
        effect.InheritLayer(hitbox.controller.mesh.spriteRenderer.sortingLayerName, hitbox.controller.mesh.spriteRenderer.sortingOrder);
    }

    protected override bool OnActivate(Controller controller) {
        Crush crush = controller.GetComponent<Crush>();
        actionBuffer = (crush.travelDistance / (crush.state.baseSpeed * crush.chargeSpeed)) * 8f / 4f;

        effect.ControlledActivate(actionBuffer);

        hitbox.controller = controller;
        hitbox.gameObject.SetActive(true);
        hitbox.Reset();
        return true;
    }

    protected override void OnDeactivate() {
        hitbox.gameObject.SetActive(false);
        effect.Activate(false);

    }

    /* --- Methods --- */
    void AdjustHitbox(float timeInterval) {
        // TODO: Make the hitbox slowly extend out.
    }
}
