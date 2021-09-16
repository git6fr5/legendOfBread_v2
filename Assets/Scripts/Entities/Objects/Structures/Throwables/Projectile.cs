using System.Collections;
using System.Collections.Generic;
using UnityEngine;

/// <summary>
/// A projectile that does damage to the enemy.
/// </summary>
public class Projectile : Throwable {

    /* --- Components --- */
    [SerializeField] protected Particle effect;
    [SerializeField] public Projectilebox projectilebox;
    [HideInInspector] public Transform target = null;

    /* --- Variables --- */
    [SerializeField] [Range(0, 10)] public int damage = 1; // The amount of damage this does on hit.
    [SerializeField] protected bool DestroyOnHit = true; // Controls whether this entity should be destroyed on hitting something.
    [SerializeField] protected bool DestroyOnReachingTarget = false;
    [HideInInspector] public Coroutine despawn = null; // Timer for when this object despawns.
    [HideInInspector] [Range(0f, 1f)] public float offset = 0.5f; // Offsets the position from which this is thrown.
    

    /* --- Event Actions --- */
    // Runs when this object is thrown.
    protected override void OnThrow() {
        mesh.transform.localPosition = new Vector3(0f, groundHeight, 0f);
        transform.localPosition = Vector3.zero;
        transform.position += (Vector3)Compass.OrientationVectors[projectilebox.controller.state.orientation] * offset;
        groundHeight = 0f;
        mesh.spriteRenderer.enabled = false;
        mesh.frame.isTrigger = true;
        projectilebox.projectile = this;
        effect.Activate(true);
        despawn = StartCoroutine(IEDespawn(3f));
    }

    protected override void Falling() {
        PointToDirection();
        if (target != null) {
            AccelerateToTarget();
        }
    }

    void PointToDirection() {
        // Points the mesh in the direction of it's movement.
        Vector2 dir = body.velocity;
        float angle = 3 * Mathf.Round(Mathf.Atan(dir.y / dir.x) * 180f / Mathf.PI / 3);
        int flip = (body.velocity.x < 0) ? 1 : 0;
        angle = (flip == 1) ? -angle : angle;
        mesh.transform.eulerAngles = Vector3.forward * angle + flip * Vector3.up * 180f;
    }

    void AccelerateToTarget() {
        body.velocity += (throwSpeed * 2) * (Vector2)(target.position - mesh.transform.position).normalized * Time.deltaTime;
        print((Vector2)(target.position - transform.position).normalized);
        if (DestroyOnReachingTarget && (target.position - mesh.transform.position).magnitude < offset / 2f + GameRules.movementPrecision) {
            if (despawn != null) { StopCoroutine(despawn); }
            Destroy(gameObject);
        }
    }

    /* --- Coroutines --- */
    // Despawns the object after a delay.
    IEnumerator IEDespawn(float delay) {
        yield return new WaitForSeconds(delay);
        Destroy(gameObject);
        yield return null;
    }

    // Runs when the projectile has hit something.
    public void Hit(Hurtbox hurtbox) {
        hurtbox.controller.Hurt(damage);
        if (DestroyOnHit) {
            if (despawn != null) { StopCoroutine(despawn); }
            Destroy(gameObject);
        }
    }

}
