using System.Collections;
using System.Collections.Generic;
using UnityEngine;

[RequireComponent(typeof(SpriteRenderer))]
[RequireComponent(typeof(BoxCollider2D))]
public class EnvironmentSelector : MonoBehaviour {

    SpriteRenderer spriteRenderer;
    public Loader loader;
    public RuleTile borderTile;
    public Sprite[] floorSprites;

    void Start() {
        spriteRenderer = GetComponent<SpriteRenderer>();
    }
    void OnMouseDown() {
        loader.environment.borderTile = borderTile;
        loader.environment.floorSprites = floorSprites;
        loader.Open(loader.id.ToString());
    }

    void Update() {
        if (loader.environment.borderTile == borderTile && loader.environment.floorSprites == floorSprites) {
            spriteRenderer.material.SetFloat("_OutlineWidth", 0.05f);
        }
        else {
            spriteRenderer.material.SetFloat("_OutlineWidth", 0f);
        }
    }
}
