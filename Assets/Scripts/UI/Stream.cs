using System.Collections;
using System.Collections.Generic;
using UnityEngine;

[RequireComponent(typeof(SpriteRenderer))]
public class Stream : MonoBehaviour {

    /* --- Components --- */
    public ASCII ascii;

    /* --- Variables --- */
    public bool isActive = false;
    public string text;
    
    /* --- Unity --- */
    void Start() {
        ascii.SetText(text);
    }

    void OnMouseDown() {
        isActive = true;
    }

    void Update() {
        // Get the input string if it's active.
        if (isActive) {
            GetInputText();
            ascii.SetText(text);
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
            else if (ascii.letters.ContainsKey(character)) {
                text = text + character;
            }
        }
        // Deactivate on a right click
        if (Input.GetMouseButtonDown(1)) {
            isActive = false;
        }
    }

}