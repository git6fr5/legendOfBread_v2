using System.Collections;
using System.Collections.Generic;
using UnityEngine;

public class Stream : MonoBehaviour {

    /* --- Components --- */
    public Alphabet alphabet;

    /* --- Variables --- */
    public bool isActive = false;
    public string text;
    
    /* --- Unity --- */
    void OnMouseDown() {
        isActive = true;
    }

    void Update() {
        // Get the input string if it's active.
        if (isActive) {
            GetInputText();
            alphabet.SetText(text);
        }
    }

    /* --- Methods --- */
    void GetInputText() {
        foreach (char character in Input.inputString) {
            if (character == '\b' && text.Length != 0) {
                text = text.Substring(0, text.Length - 1);
            }
            else if (alphabet.letters.ContainsKey(character)) {
                text = text + character;
            }
        }
        // Deactivate on a right click
        if (Input.GetMouseButtonDown(1)) {
            isActive = false;
        }
    }

}