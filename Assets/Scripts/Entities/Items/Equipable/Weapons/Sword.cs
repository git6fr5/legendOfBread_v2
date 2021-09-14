/* --- Libraries --- */
using System.Collections;
using System.Collections.Generic;
using UnityEngine;

/* --- Enumerations --- */
using Action = State.Action;

/// <summary>
/// A sweeping blade attack.
/// </summary>
public class Sword : Equipable {

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
        int index = ((int)Mathf.Floor(timeInterval * effect.effect.Length / actionBuffer) % effect.effect.Length); ;
        hitbox.transform.localRotation = Quaternion.Euler(0, 0, -(index * 180 / (effect.effect.Length - 1)));
    }

}
