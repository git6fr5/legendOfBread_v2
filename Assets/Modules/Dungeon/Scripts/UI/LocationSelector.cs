/* --- Libraries --- */
using System.Collections;
using System.Collections.Generic;
using UnityEngine;

[RequireComponent(typeof(SpriteRenderer))]
[RequireComponent(typeof(BoxCollider2D))]
public class LocationSelector : MonoBehaviour {

    public Map map;
    public Vector2Int gridPosition;
    SpriteRenderer spriteRenderer;

    /* --- Unity --- */
    void Start() {
        spriteRenderer = GetComponent<SpriteRenderer>();
    }

    void OnMouseOver() {
        spriteRenderer.material.SetFloat("_OutlineWidth", 0.05f);
    }

    void OnMouseExit() {
        spriteRenderer.material.SetFloat("_OutlineWidth", 0f);
    }

    void OnMouseDown() {
        map.OpenRoom(gridPosition);
    }

}
