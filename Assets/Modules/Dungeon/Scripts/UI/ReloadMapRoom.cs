/* --- Libraries --- */
using System.Collections;
using System.Collections.Generic;
using UnityEngine;

[RequireComponent(typeof(SpriteRenderer))]
[RequireComponent(typeof(BoxCollider2D))]
public class ReloadMapRoom : MonoBehaviour {

    public Map map;
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
        Room room = map.mapData.loc_room[map.location];
        map.OpenRoom(room.id, map.location);
    }

}
