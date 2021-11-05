using System.Collections;
using System.Collections.Generic;
using UnityEngine;

public class KeysDisplay : ValueDisplay {
    public Player player;

    protected override void GetValue() {
        value = player.numKeys;
    }
}
