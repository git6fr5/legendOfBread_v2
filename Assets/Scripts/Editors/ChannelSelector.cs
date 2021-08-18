using System.Collections;
using System.Collections.Generic;
using UnityEngine;
using UnityEngine.Events;

using CHANNEL = RoomEditor.CHANNEL;

[RequireComponent(typeof(BoxCollider2D))]
[RequireComponent(typeof(SpriteRenderer))]
public class ChannelSelector : MonoBehaviour {
    
    /* --- Events --- */
    [System.Serializable] public class ChannelEvent : UnityEvent<CHANNEL> { }
    public ChannelEvent OnSelect;

    /* --- Variables --- */
    public CHANNEL channel;

    /* --- Unity --- */
    // Runs whenever the attached collider is clicked on.
    void OnMouseDown() {
        OnSelect.Invoke(channel);
    }

}
