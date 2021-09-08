using System.Collections;
using System.Collections.Generic;
using UnityEngine;

public class Bombable : MonoBehaviour {

    public int damageInstances = 0;
    public int threshold = 1;

    public void IncrementDamage() {
        damageInstances++;
        if (damageInstances >= threshold) {
            Destroy(gameObject);
        }
    }

}
