using System.Collections;
using System.Collections.Generic;
using UnityEngine;
using UnityEngine.Events;

using TILE = Janitor.TILE;

[RequireComponent(typeof(BoxCollider2D))]
[RequireComponent(typeof(SpriteRenderer))]
public class ExitSelector : MonoBehaviour {

    /* --- Events --- */
    [System.Serializable] public class ExitEvent : UnityEvent<TILE> { }
    public ExitEvent OnSelect;

    /* --- Variables --- */
    public TILE exits;

    /* --- Unity --- */
    // Runs whenever the attached collider is clicked on.
    void OnMouseDown() {
        OnSelect.Invoke(exits);
    }
}
