using System.Collections;
using System.Collections.Generic;
using UnityEngine;

public class Number : Alphabet {
    
    /* --- Unity --- */
    protected override void Awake() {
    // set the dictionary
        ascii = "0123456789";
        letters = new Dictionary<char, Sprite>();
        int length = (int)Mathf.Min(ascii.Length, letterSprites.Length);
        for (int i = 0; i < length; i++) {
            print(ascii[i]);
            letters.Add(ascii[i], letterSprites[i]);
        }
    }
}
