using System.Collections;
using System.Collections.Generic;
using UnityEngine;
using UnityEngine.Events;

using CHANNEL = MapEditor.CHANNEL;

[RequireComponent(typeof(BoxCollider2D))]
[RequireComponent(typeof(SpriteRenderer))]
public class MapChannelSelector : MonoBehaviour {

    /* --- Events --- */
    [System.Serializable] public class MapChannelEvent : UnityEvent<CHANNEL> { }
    public MapChannelEvent OnSelect;

    /* --- Variables --- */
    public CHANNEL channel;

    /* --- Unity --- */
    // Runs whenever the attached collider is clicked on.
    void OnMouseDown() {
        OnSelect.Invoke(channel);
    }

}
