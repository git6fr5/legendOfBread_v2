﻿using System.Collections;
using System.Collections.Generic;
using UnityEngine;
using ORIENTATION = Compass.ORIENTATION;

[RequireComponent(typeof(SpriteRenderer))]
public class Human : Mesh {

    /* --- Components --- */
    public State state;
    public Sprite[] idle;
    public Sprite[] carryIdle;
    public Sprite[] walk;
    public Sprite[] carryWalk;
    public Sprite[] throwing;
    public Sprite[] attack;
    public Material defaultMaterial;
    public Material hurtMaterial;
    public Material deathMaterial;

    /* --- Variables --- */
    SpriteRenderer spriteRenderer;
    Sprite[] active; // This is solely to be used as a switch
    int walkCycle = 4;
    int attackCycle = 6;
    int throwCycle = 2;
    int frameRate = 8;
    [HideInInspector] public float timeInterval = 0f;

    /* --- Unity --- */
    // Runs once before the first frame.
    void Start() {
        spriteRenderer = GetComponent<SpriteRenderer>();
        spriteRenderer.sortingLayerName = GameRules.midGround;
        //
    }

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
        transform.localPosition = groundPosition + new Vector3(0, state.height * Mathf.Sin(GameRules.perspectiveAngle), 0f);
        if (state.isAttacking) {
            if (active != attack) {
                timeInterval = 0f;
                active = attack;
            }
            int index = attackCycle * (int)state.orientation + ((int)Mathf.Floor(timeInterval * attackCycle / state.attackBuffer) % attackCycle); ;
            spriteRenderer.sprite = attack[index];
        }
        else if (state.isThrowing) {
            if (active != throwing) {
                timeInterval = 0f;
                active = throwing;
            }
            int index = throwCycle * (int)state.orientation + ((int)Mathf.Floor(timeInterval * throwCycle / state.throwBuffer) % throwCycle); ;
            spriteRenderer.sprite = throwing[index];
        }
        else if (state.isMoving) {
            int index;
            if (state.isCarrying) {
                if (active != carryWalk) {
                    timeInterval = 0f;
                    active = carryWalk;
                }
                index = walkCycle * (int)state.orientation + ((int)Mathf.Floor(timeInterval * frameRate) % walkCycle);
                spriteRenderer.sprite = carryWalk[index];
            }
            else {
                if (active != walk) {
                    timeInterval = 0f;
                    active = walk;
                }
                index = walkCycle * (int)state.orientation + ((int)Mathf.Floor(timeInterval * frameRate) % walkCycle);
                spriteRenderer.sprite = walk[index];
                if (index % 2 == 0) {
                    transform.localPosition += new Vector3(0, 0.05f, 0);
                }
            }         
        }
        else {
            if (state.isCarrying) {
                active = carryIdle;
                spriteRenderer.sprite = carryIdle[(int)state.orientation];
            }
            else {
                active = idle;
                spriteRenderer.sprite = idle[(int)state.orientation];
            }
        }
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
