using System.Collections;
using System.Collections.Generic;
using UnityEngine;

/// <summary>
/// A bomb structure that can be thrown.
/// </summary>
public class Bomb : Throwable {

    /* --- Variables --- */
    public Vision bombVision; // The radius that this bomb will have an effect.
    public float explosionTickDuration = 0.25f; // The duration of a counting tick.
    public int explosionTicks = 16; // The amount of ticks before the bomb explodes.

    public Sequence bombSequence;
    bool isCharging = false;

    /* --- Event Actions --- */
    // Runs when this object is thrown.
    protected override void OnThrow() {
        mesh.spriteRenderer.enabled = false;
        bombSequence.Activate(true);
        StartCoroutine(IEExplode(explosionTickDuration));
    }

    /* --- Internal Actions --- */
    // Explodes and effects the bombable structures in vision radius.
    void Explode() {
        for (int i = 0; i < bombVision.container.Count; i++) {
            if (bombVision.container[i].tag == GameRules.bombableTag) {
                bombVision.container[i].GetComponent<Bombable>()?.Blast();
            }
        }
        bombSequence.transform.parent = null;
        bombSequence.NextAndLast();
        Destroy(gameObject);
    }

    /* --- Coroutines --- */
    // Counts down until the explosion.
    IEnumerator IEExplode(float delay) {
        for (int i = 0; i < explosionTicks; i++) {
            if (i >= explosionTicks * 2f / 3f && !isCharging) {
                bombSequence.Next();
                isCharging = true;
            }
            yield return new WaitForSeconds(delay);
            bombSequence.InheritLayer(mesh.spriteRenderer.sortingLayerName, mesh.spriteRenderer.sortingOrder, 0);
        }
        Explode();
        yield return null;
    }


}
