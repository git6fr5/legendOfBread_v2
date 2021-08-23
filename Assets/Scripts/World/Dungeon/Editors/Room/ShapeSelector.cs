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

    /* --- Components --- */
    public Sprite[] sprites;

    /* --- Variables --- */
    public SHAPE shape;

    SpriteRenderer spriteRenderer;
    Room room;

    /* --- Unity --- */

    void Awake() {
        room = GameObject.FindWithTag(GameRules.roomTag).GetComponent<Room>();
        spriteRenderer = GetComponent<SpriteRenderer>();
    }
    
    // Runs whenever the attached collider is clicked on.
    void OnMouseDown() {
        OnSelect.Invoke(shape);
    }


    void Update() {
        if ((int)room.shape < sprites.Length) {
            spriteRenderer.sprite = sprites[(int)room.shape];
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
