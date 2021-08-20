using System.Collections;
using System.Collections.Generic;
using UnityEngine;
using UnityEngine.Events;

using CHALLENGE = Room.CHALLENGE;

[RequireComponent(typeof(BoxCollider2D))]
[RequireComponent(typeof(SpriteRenderer))]
public class ChannelSelector : MonoBehaviour {
    
    /* --- Events --- */
    [System.Serializable] public class ChannelEvent : UnityEvent<CHALLENGE> { }
    public ChannelEvent OnSelect;

    /* --- Variables --- */
    public CHALLENGE challenge;

    /* --- Unity --- */
    // Runs whenever the attached collider is clicked on.
    void OnMouseDown() {
        OnSelect.Invoke(challenge);
    }

}
