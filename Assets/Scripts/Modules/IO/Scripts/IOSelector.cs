using System.Collections;
using System.Collections.Generic;
using UnityEngine;
using UnityEngine.Events;

[RequireComponent(typeof(BoxCollider2D))]
[RequireComponent(typeof(SpriteRenderer))]
public class IOSelector : MonoBehaviour  {
    /* --- Events --- */
    [System.Serializable] public class IOEvent : UnityEvent<string> { }
    public IOEvent OnSelect;

    /* --- Components --- */
    public Stream stream;

    /* --- Unity --- */
    // Runs whenever the attached collider is clicked on.
    void OnMouseDown() {
        OnSelect.Invoke(stream.text);
    }

    void OnMouseOver() {
        GetComponent<SpriteRenderer>().material.SetFloat("_OutlineWidth", 0.05f);
    }

    void OnMouseExit() {
        GetComponent<SpriteRenderer>().material.SetFloat("_OutlineWidth", 0f);
    }
}
