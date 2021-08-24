using System.Collections;
using System.Collections.Generic;
using UnityEngine;

public class Alphabet : ASCII {

    /* --- Unity --- */
    protected override void Awake() {
        // set the dictionary
        ascii = "ABCDEFGHIJKLMNOPQRSTUVWXYZabcdefghijklmnopqrstuvwxyz";
        letters = new Dictionary<char, Sprite>();
        int length = (int)Mathf.Min(ascii.Length, letterSprites.Length);
        for (int i = 0; i < length; i++) {
            letters.Add(ascii[i], letterSprites[i]);
        }
    }

}
