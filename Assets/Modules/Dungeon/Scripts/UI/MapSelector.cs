/* --- Libraries --- */
using System.Collections;
using System.Collections.Generic;
using UnityEngine;

[RequireComponent(typeof(SpriteRenderer))]
[RequireComponent(typeof(BoxCollider2D))]
public class MapSelector : MonoBehaviour {

    /* --- Variables --- */
    public Map map;
    public int increment;

    /* --- Unity --- */
    void OnMouseOver() {
        GetComponent<SpriteRenderer>().material.SetFloat("_OutlineWidth", 0.05f);
    }

    void OnMouseExit() {
        GetComponent<SpriteRenderer>().material.SetFloat("_OutlineWidth", 0f);
    }

    /* --- Override --- */
    void OnMouseDown() {
        map.id = map.id + increment;
        if (map.id < 0) { map.id = 0; }
        map.Open(map.id);
    }

}
