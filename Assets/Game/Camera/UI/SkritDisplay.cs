using System.Collections;
using System.Collections.Generic;
using UnityEngine;

public class SkritDisplay : ValueDisplay {

    public Player player;

    protected override void GetValue() {
        value = player.skrit;
    }
}
