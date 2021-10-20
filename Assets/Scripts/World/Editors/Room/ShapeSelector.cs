using System.Collections;
using System.Collections.Generic;
using UnityEngine;
using UnityEngine.Events;

using SHAPE = Geometry.Shape;

[RequireComponent(typeof(BoxCollider2D))]
[RequireComponent(typeof(SpriteRenderer))]
public class ShapeSelector : MonoBehaviour {

    /* --- Events --- */
    public UnityEvent OnSelect;

    /* --- Variables --- */
    SpriteRenderer spriteRenderer;
    Room room;

    /* --- Unity --- */

    void Awake() {
        room = GameObject.FindWithTag(GameRules.roomTag).GetComponent<Room>();
        spriteRenderer = GetComponent<SpriteRenderer>();
    }
    
    // Runs whenever the attached collider is clicked on.
    void OnMouseDown() {
        OnSelect.Invoke();
    }

    void OnMouseOver() {
        GetComponent<SpriteRenderer>().material.SetFloat("_OutlineWidth", 0.05f);
    }

    void OnMouseExit() {
        GetComponent<SpriteRenderer>().material.SetFloat("_OutlineWidth", 0f);
    }

}
