using System.Collections;
using System.Collections.Generic;
using UnityEngine;

using Action = State.Action;

public class Boomerang : Item {

    public Projectile projectile;

    /* --- Unity --- */
    // Runs once on initialisation.
    void Awake() {
        action = Action.Attacking;
    }

    protected override bool OnActivate(Controller controller) {
        Projectile newProjectile = Instantiate(projectile, Vector3.zero, Quaternion.identity, null).GetComponent<Projectile>();
        newProjectile.gameObject.SetActive(true);
        newProjectile.Carry(controller);
        newProjectile.projectilebox.controller = controller;
        newProjectile.Throw(controller);
        newProjectile.target = controller.transform;
        return true;
    }


}
