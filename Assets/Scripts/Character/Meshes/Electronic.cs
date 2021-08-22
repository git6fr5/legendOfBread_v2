using System.Collections;
using System.Collections.Generic;
using UnityEngine;

[RequireComponent(typeof(SpriteRenderer))]
[RequireComponent(typeof(BoxCollider2D))]
public class Electronic : Mesh {

    /* --- Components --- */
    public Trap trap;
    public Sprite[] off;
    public Sprite[] on;

    /* --- Variables --- */
    Sprite[] active;
    SpriteRenderer spriteRenderer;
    int frameRate = 8;
    float timeInterval = 0f;

    /* --- Unity --- */
    // Runs once before the first frame.
    void Start() {
        spriteRenderer = GetComponent<SpriteRenderer>();
        spriteRenderer.sortingLayerName = GameRules.midGround;
    }

    // The parameters to be rendered every frame
    public override void Render() {
        RenderSprite();
    }

    /* --- Parameters --- */
    // Renders the sprite based on the state.
    void RenderSprite() {
        timeInterval += Time.deltaTime;
        if (trap.button == Trap.BUTTON.ON) {
            if (active != on) {
                timeInterval = 0f;
                active = on;
            }
            int index = ((int)Mathf.Floor(timeInterval * frameRate) % on.Length);
            spriteRenderer.sprite = on[index];
        }
        else {
            if (active != off) {
                timeInterval = 0f;
                active = off;
            }
            int index = ((int)Mathf.Floor(timeInterval * frameRate) % off.Length);
            spriteRenderer.sprite = off[index];
        }
    }
}
