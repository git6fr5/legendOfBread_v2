using System.Collections;
using System.Collections.Generic;
using UnityEngine;
using UnityEngine.Events;

[RequireComponent(typeof(BoxCollider2D))]
[RequireComponent(typeof(SpriteRenderer))]
public class ValueSelector : MonoBehaviour {

    /* --- Events --- */
    [System.Serializable] public class IntEvent : UnityEvent<int> { }
    public IntEvent OnSelect;

    /* --- Variables --- */
    public int index;
    [HideInInspector] public SpriteRenderer spriteRenderer;

    /* --- Unity --- */
    // Runs once on compilation.
    void Awake() {
        spriteRenderer = GetComponent<SpriteRenderer>();
    }

    // Runs whenever the attached collider is clicked on.
    void OnMouseDown() {
        OnSelect.Invoke(index);
    }

}
