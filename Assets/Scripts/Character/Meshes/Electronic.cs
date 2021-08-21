using System.Collections;
using System.Collections.Generic;
using UnityEngine;

[RequireComponent(typeof(SpriteRenderer))]
[RequireComponent(typeof(BoxCollider2D))]
public class Electronic : Mesh {

    /* --- Components --- */
    public State state;
    public Sprite[] off;
    public Sprite[] on;

    /* --- Variables --- */
    SpriteRenderer spriteRenderer;
    int frameRate = 8;
    float timeInterval = 0f;

    /* --- Unity --- */
    // Runs once before the first frame.
    void Start() {
        spriteRenderer = GetComponent<SpriteRenderer>();
        spriteRenderer.sortingLayerName = GameRules.midGround;
    }

    /* --- Override --- */
    // The parameters to be rendered every frame
    public override void Render() {
        //
    }
}
