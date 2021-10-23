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

    /* --- Components --- */
    public MapEditor mapEditor;

    /* --- Variables --- */
    public CHANNEL channel;
    SpriteRenderer spriteRenderer;

    /* --- Unity --- */
    void Awake() {
        spriteRenderer = GetComponent<SpriteRenderer>();
    }

    void Update() {
        if (mapEditor.channel == channel) {
            GetComponent<SpriteRenderer>().material.SetFloat("_OutlineWidth", 0.05f);
        }
        else {
            GetComponent<SpriteRenderer>().material.SetFloat("_OutlineWidth", 0f);
        }
    }

    // Runs whenever the attached collider is clicked on.
    void OnMouseDown() {
        OnSelect.Invoke(channel);
    }

}
