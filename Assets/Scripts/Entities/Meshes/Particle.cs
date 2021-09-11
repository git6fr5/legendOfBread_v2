using System.Collections;
using System.Collections.Generic;
using UnityEngine;

[RequireComponent(typeof(SpriteRenderer))]
public class Particle : MonoBehaviour {

    /* --- Components --- */
    [HideInInspector] public SpriteRenderer spriteRenderer;

    /* --- Animations --- */
    public Sprite[] effect;

    /* --- Variables --- */
    public bool inFront = true;
    public bool isLoop = true;
    public bool isDisposable = true;
    public float frameRate = 8f;
    public float timeInterval = 0f;

    /* --- Unity --- */
    // Runs once before the first frame.
    void Awake() {
        spriteRenderer = GetComponent<SpriteRenderer>();
        // Set the layer of this mesh;
        if (inFront) {
            spriteRenderer.sortingLayerName = GameRules.particleFront;
        }
        else {
            spriteRenderer.sortingLayerName = GameRules.particleBehind;
        }
    }

    // Runs every frame.
    void Update() {
        if (gameObject.activeSelf) {
            Render();
        }
    }

    public void Activate(bool activate) {
        timeInterval = 0f;
        gameObject.SetActive(activate);
        isLoop = true;
    }

    public void Fire() {
        Activate(true);
        isLoop = false;
    }

    public void ControlledFire(float duration) {
        frameRate = (float)(effect.Length - 1) / duration;
        Fire();
    }

    public void ControlledActivate(float duration) {
        frameRate = (float)(effect.Length) / duration;
        Activate(true);
    }

    /* --- Override --- */
    // The parameters to be rendered every frame
    void Render() {
        timeInterval += Time.deltaTime;
        int index = ((int)Mathf.Floor(timeInterval * frameRate) % effect.Length);
        if (index == effect.Length - 1 && !isLoop) {
            if (isDisposable) {
                Destroy(gameObject);
            }
            else {
                Activate(false);
            }
        }
        spriteRenderer.sprite = effect[index];
    }

}
