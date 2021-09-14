using System.Collections;
using System.Collections.Generic;
using UnityEngine;

/// <summary>
/// A bomb structure that can be thrown.
/// </summary>
public class Bomb : Throwable {

    ///* --- Variables --- */
    //public Vision bombVision; // The radius that this bomb will have an effect.
    //public float explosionTickDuration = 0.5f; // The duration of a counting tick.
    //public int explosionTicks = 4; // The amount of ticks before the bomb explodes.

    ///* --- Event Actions --- */
    //// Runs when this object is thrown.
    //protected override void OnThrow() {
    //    StartCoroutine(IEExplode(explosionTickDuration));
    //}

    ///* --- Internal Actions --- */
    //// Explodes and effects the bombable structures in vision radius.
    //void Explode() {
    //    for (int i = 0; i < bombVision.container.Count; i++) {
    //        if (bombVision.container[i].tag == GameRules.bombableTag) {
    //            bombVision.container[i].GetComponent<Bombable>()?.Blast();
    //        }
    //    }
    //    Destroy(gameObject);
    //}

    ///* --- Coroutines --- */
    //// Counts down until the explosion.
    //IEnumerator IEExplode(float delay) {
    //    for (int i = 0; i < explosionTicks; i++) {
    //        mesh.spriteRenderer.enabled = !mesh.spriteRenderer.enabled;
    //        yield return new WaitForSeconds(delay);
    //    }
    //    Explode();
    //    yield return null;
    //}


}
