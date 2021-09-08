using System.Collections;
using System.Collections.Generic;
using UnityEngine;

public class Particle : Mesh
{
    /* --- Components --- */
    public Sprite[] effect;

    /* --- Variables --- */
    SpriteRenderer spriteRenderer;
    public bool isLoop = true;
    public int frameRate = 8;
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
        RenderSprite();
    }

    /* --- Parameters --- */
    // Renders the sprite based on the state.
    void RenderSprite() {
        timeInterval += Time.deltaTime;
        int index = ((int)Mathf.Floor(timeInterval * frameRate) % effect.Length);
        if (index == effect.Length - 1 && !isLoop) {
            Destroy(gameObject);
        }
        spriteRenderer.sprite = effect[index];
    }
}
