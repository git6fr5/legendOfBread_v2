using System.Collections;
using System.Collections.Generic;
using UnityEngine;

[RequireComponent(typeof(SpriteRenderer))]
public class Spike : Mesh {

    /* --- Components --- */
    public Crush controller;
    public Hitbox hitbox;
    public Sprite[] swing;

    /* --- Variables --- */
    SpriteRenderer spriteRenderer;

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
        RenderHitbox();
    }

    /* --- Parameters --- */
    // Renders the sprite based on the state.
    void RenderSprite() {
        if (controller.state.isAttacking) {
            float normDist = Vector2.Distance((Vector2)controller.origin, (Vector2)controller.transform.position) / controller.travelDistance;
            int index = ((int)Mathf.Floor(normDist * swing.Length) % swing.Length); ;
            spriteRenderer.sprite = swing[index];
        }
    }

    void RenderHitbox() {
        //if (holder.state.isAttacking) {
        //    int index = ((int)Mathf.Floor(holder.timeInterval * swing.Length / holder.state.attackBuffer) % swing.Length); ;
        //    hitbox.transform.localRotation = Quaternion.Euler(0, 0, -(index * 180 / (swing.Length - 1)));
        //}
    }

}
