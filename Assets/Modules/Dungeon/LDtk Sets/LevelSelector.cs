using System.Collections;
using System.Collections.Generic;
using UnityEngine;

[RequireComponent(typeof(SpriteRenderer))]
[RequireComponent(typeof(BoxCollider2D))]
public class LevelSelector : MonoBehaviour {

    SpriteRenderer spriteRenderer;
    public Loader loader;
    public int increment;

    void Start() {
        spriteRenderer = GetComponent<SpriteRenderer>();
    }
    void OnMouseDown() {
        loader.id = loader.id + increment;
        if (loader.id < 0) {
            loader.id = 0;
        }
        loader.Open(loader.id.ToString());
    }

    void OnMouseOver() {
        spriteRenderer.material.SetFloat("_OutlineWidth", 0.05f);
    }

    void OnMouseExit() {
        spriteRenderer.material.SetFloat("_OutlineWidth", 0f);
    }

}
