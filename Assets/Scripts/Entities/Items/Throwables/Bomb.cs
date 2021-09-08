using System.Collections;
using System.Collections.Generic;
using UnityEngine;

public class Bomb : Throwable {

    public BombVision bombVision;
    public float explosionTickDuration = 0.5f;
    public int explosionTicks = 4;

    protected override void OnThrow() {
        StartCoroutine(IEExplode(explosionTickDuration));
    }
     
    IEnumerator IEExplode(float delay) {
        for (int i = 0; i < explosionTicks; i++) {
            mesh.GetComponent<SpriteRenderer>().enabled = !mesh.GetComponent<SpriteRenderer>().enabled;
            yield return new WaitForSeconds(delay);
        }
        Explode();
        yield return null;
    }

    void Explode() {
        for (int i = 0; i < bombVision.container.Count; i++) {
            bombVision.container[i].IncrementDamage();
        }
        Destroy(gameObject);
    }


}
