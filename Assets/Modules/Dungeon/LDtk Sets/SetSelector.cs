using System.Collections;
using System.Collections.Generic;
using UnityEngine;
using LDtkUnity;

[RequireComponent(typeof(SpriteRenderer))]
[RequireComponent(typeof(BoxCollider2D))]
public class SetSelector : MonoBehaviour {

    SpriteRenderer spriteRenderer;
    public LDtkComponentProject lDtkData;
    public Loader loader;

    void Start() {
        spriteRenderer = GetComponent<SpriteRenderer>();
    }
    void OnMouseDown() {
        loader.lDtkData = lDtkData;
        loader.Open(loader.id.ToString());
    }

    void Update() {
        if (loader.lDtkData == lDtkData) {
            spriteRenderer.material.SetFloat("_OutlineWidth", 0.05f);
        } 
        else {
            spriteRenderer.material.SetFloat("_OutlineWidth", 0f);
        }
    }

}
