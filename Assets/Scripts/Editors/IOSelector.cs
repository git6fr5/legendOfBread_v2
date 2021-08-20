using System.Collections;
using System.Collections.Generic;
using UnityEngine;
using UnityEngine.Events;

using SHAPE = Geometry.SHAPE;

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
}
