using System.Collections;
using System.Collections.Generic;
using UnityEngine;
using ORIENTATION = Compass.ORIENTATION;
using ActionState = State.ActionState;

public class Human : Mesh {

    /* --- Components --- */
    public State state;

    /* --- Animations --- */
    public Sprite[] idle;
    public Sprite[] carryIdle;
    public Sprite[] walk;
    public Sprite[] carryWalk;
    public Sprite[] throwing;
    public Sprite[] attack;

    /* --- Materials --- */
    public Material defaultMaterial;
    public Material hurtMaterial;
    public Material deathMaterial;

    /* --- Variables --- */
    Sprite[] active; // This is solely to be used as a switch
    int walkCycle = 4;
    int attackCycle = 6;
    int throwCycle = 2;
    int frameRate = 8;
    [HideInInspector] public float timeInterval = 0f;

    public Dictionary<ActionState, (Sprite[], int)> actionAnim;

    void Start() {
        actionAnim = new Dictionary<ActionState, (Sprite[], int)>() {
            {ActionState.Attacking, (attack, attackCycle) },
            {ActionState.Throwing, (throwing, throwCycle) }
        };
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
        transform.localPosition = Vector3.zero;
        if (state.activeItem != null && actionAnim.ContainsKey(state.activeItem.action)) {
            Sprite[] action = actionAnim[state.activeItem.action].Item1;
            int cycle = actionAnim[state.activeItem.action].Item2;
            if (active != action) {
                timeInterval = 0f;
                active = action;
            }
            int index = cycle * (int)state.orientation + ((int)Mathf.Floor(timeInterval * cycle / state.activeItem.actionBuffer) % cycle);
            spriteRenderer.sprite = action[index];
        }
        else if (state.isMoving) {
            int index;
            if (state.actionState == ActionState.Carrying) {
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
            if (state.actionState == ActionState.Carrying) {
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
