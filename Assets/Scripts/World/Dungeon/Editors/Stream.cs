using System.Collections;
using System.Collections.Generic;
using UnityEngine;

[RequireComponent(typeof(SpriteRenderer))]
public class Stream : MonoBehaviour {

    /* --- Components --- */
    public Alphabet alphabet;

    /* --- Variables --- */
    public bool isActive = false;
    public string text;
    
    /* --- Unity --- */
    void Start() {
        alphabet.SetText(text);
    }

    void OnMouseDown() {
        isActive = true;
    }

    void Update() {
        // Get the input string if it's active.
        if (isActive) {
            GetInputText();
            alphabet.SetText(text);
            GetComponent<SpriteRenderer>().material.SetFloat("_OutlineWidth", 0.05f);
        }
        else {
            GetComponent<SpriteRenderer>().material.SetFloat("_OutlineWidth", 0f);
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