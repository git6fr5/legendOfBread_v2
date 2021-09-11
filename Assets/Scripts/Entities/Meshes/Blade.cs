using System.Collections;
using System.Collections.Generic;
using UnityEngine;

[RequireComponent(typeof(SpriteRenderer))]
public class Blade : Mesh {

    ///* --- Components --- */
    //public Human holder;
    //public Hitbox hitbox;

    ///* --- Animations --- */
    //public Sprite[] swing;

    ///* --- Override --- */
    //// The parameters to be rendered every frame
    //public override void Render() {
    //    RenderSprite();
    //    RenderHitbox();
    //}

    ///* --- Parameters --- */
    //// Renders the sprite based on the state.
    //void RenderSprite() {
    //    if (holder.state.isAttacking) {
    //        int index = ((int)Mathf.Floor(holder.timeInterval * swing.Length / holder.state.attackBuffer) % swing.Length); ;
    //        spriteRenderer.sprite = swing[index];
    //    }
    //}

    //void RenderHitbox() {
    //    if (holder.state.isAttacking) {
    //        int index = ((int)Mathf.Floor(holder.timeInterval * swing.Length / holder.state.attackBuffer) % swing.Length); ;
    //        hitbox.transform.localRotation = Quaternion.Euler(0, 0, -(index * 180 / (swing.Length - 1)));
    //    }
    //}

}
