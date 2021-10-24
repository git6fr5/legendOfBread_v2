using System.Collections;
using System.Collections.Generic;
using UnityEngine;

public class DifficultySelector : MonoBehaviour
{
    SpriteRenderer spriteRenderer;
    public Loader loader;

    void Start() {
        spriteRenderer = GetComponent<SpriteRenderer>();
    }

    void Update() {
        if (loader.level.difficult) {
            spriteRenderer.material.SetFloat("_OutlineWidth", 0.05f);
        }
        else {
            spriteRenderer.material.SetFloat("_OutlineWidth", 0f);
        }
    }
    void OnMouseDown() {
        loader.level.difficult = !loader.level.difficult;
        loader.Open(loader.id.ToString());
    }
}
