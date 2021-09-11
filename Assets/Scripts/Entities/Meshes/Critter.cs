using System.Collections;
using System.Collections.Generic;
using UnityEngine;

[RequireComponent(typeof(SpriteRenderer))]
[RequireComponent(typeof(CapsuleCollider2D))]
public class Critter : Mesh {
    
    /* --- Components --- */
    public State state;

    /* --- Animations --- */
    public Sprite[] idle;

    /* --- Materials --- */
    public Material defaultMaterial;
    public Material hurtMaterial;
    public Material deathMaterial;

    /* --- Variables --- */
    int frameRate = 8;
    float timeInterval = 0f;

    /* --- Override --- */
    // The parameters to be rendered every frame
    public override void Render() {
        RenderSprite();
        RenderMaterial();
    }

    /* --- Parameters --- */
    // Renders the sprite based on the state.
    void RenderSprite() {
        timeInterval += Time.deltaTime;
        transform.localRotation = Compass.OrientationAngles[state.orientation];
        int index = ((int)Mathf.Floor(timeInterval * frameRate) % idle.Length); ;
        spriteRenderer.sprite = idle[index];
    }


    // Renders the material based on the state.
    void RenderMaterial() {
        if (state.isDead) {
            spriteRenderer.material = deathMaterial;
        }
        else if (state.isHurt) {
            spriteRenderer.material = hurtMaterial;
        }
        else {
            spriteRenderer.material = defaultMaterial;
        }
    }

}
