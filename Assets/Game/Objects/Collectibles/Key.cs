using System.Collections;
using System.Collections.Generic;
using UnityEngine;

public class Key : Collectible {

    /* --- Overridden Methods --- */
    protected override void Collect(Player player) {

        // player.keys.Add(this);
        player.numKeys += 1;
        gameObject.SetActive(false);

    }

    void Awake() {
        origin = transform.position;
    }

}
