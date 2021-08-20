using System.Collections;
using System.Collections.Generic;
using UnityEngine;
using UnityEngine.Events;

using EXIT = Compass.EXIT;

[RequireComponent(typeof(BoxCollider2D))]
[RequireComponent(typeof(SpriteRenderer))]
public class ExitSelector : MonoBehaviour {

    /* --- Events --- */
    [System.Serializable] public class ExitEvent : UnityEvent<EXIT> { }
    public ExitEvent OnSelect;

    /* --- Variables --- */
    public EXIT exits;

    /* --- Unity --- */
    // Runs whenever the attached collider is clicked on.
    void OnMouseDown() {
        OnSelect.Invoke(exits);
    }
}
