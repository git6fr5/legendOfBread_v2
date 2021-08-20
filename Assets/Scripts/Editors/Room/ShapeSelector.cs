using System.Collections;
using System.Collections.Generic;
using UnityEngine;
using UnityEngine.Events;

using SHAPE = Geometry.SHAPE;

[RequireComponent(typeof(BoxCollider2D))]
[RequireComponent(typeof(SpriteRenderer))]
public class ShapeSelector : MonoBehaviour {

    /* --- Events --- */
    [System.Serializable] public class ShapeEvent : UnityEvent<SHAPE> { }
    public ShapeEvent OnSelect;

    /* --- Variables --- */
    public SHAPE shape;

    /* --- Unity --- */
    // Runs whenever the attached collider is clicked on.
    void OnMouseDown() {
        OnSelect.Invoke(shape);
    }

}
