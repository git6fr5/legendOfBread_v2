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
    public Sprite[] sprites;
    public MapEditor mapEditor;

    /* --- Variables --- */
    public CHANNEL channel;
    SpriteRenderer spriteRenderer;

    /* --- Unity --- */
    void Awake() {
        spriteRenderer = GetComponent<SpriteRenderer>();
    }


    // Runs whenever the attached collider is clicked on.
    void OnMouseDown() {
        OnSelect.Invoke(channel);
    }

    void Update() {
        if ((int)mapEditor.channel < sprites.Length) {
            spriteRenderer.sprite = sprites[(int)mapEditor.channel];
        }
        else {
            spriteRenderer.sprite = sprites[0];
        }
    }

    void OnMouseOver() {
        GetComponent<SpriteRenderer>().material.SetFloat("_OutlineWidth", 0.05f);
    }

    void OnMouseExit() {
        GetComponent<SpriteRenderer>().material.SetFloat("_OutlineWidth", 0f);
    }

}
