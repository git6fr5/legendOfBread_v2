using System.Collections;
using System.Collections.Generic;
using UnityEngine;

[RequireComponent(typeof(SpriteRenderer))]
public class Particle : MonoBehaviour {

    /* --- Components --- */
    [HideInInspector] public SpriteRenderer spriteRenderer;

    /* --- Animations --- */
    public Sprite[] effect;

    /* --- Rendering Variables --- */
    public bool inFront = true;
    public bool isLoop = true;
    public bool isDisposable = true;
    public float frameRate = 8f;
    public float timeInterval = 0f;

    /* --- Pause Variables --- */
    public bool isPaused = false;
    public int pauseFrame = -1;


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
        if (gameObject.activeSelf && !isPaused) {
            Render();
        }
    }

    public void Activate(bool activate) {
        timeInterval = 0f;
        gameObject.SetActive(activate);
        isPaused = false;
        isLoop = true;
    }

    public void Fire() {
        Activate(true);
        isLoop = false;
    }

    public void FireAndDestroy() {
        Fire();
        isDisposable = true;
    }

    public void ControlledFire(float duration) {
        frameRate = (float)(effect.Length - 1) / duration;
        Fire();
    }

    public void ControlledActivate(float duration) {
        frameRate = (float)(effect.Length) / duration;
        Activate(true);
    }

    public void InheritLayer(string layerName, int index, int discrepancy = 1) {
        spriteRenderer.sortingLayerName = layerName;
        spriteRenderer.sortingOrder = index + discrepancy;
    }

    public void PauseAtFrame(int _pauseFrame) {
        pauseFrame = _pauseFrame;
    }

    public void Pause(bool pause) {
        isPaused = pause;
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
        else if (index == pauseFrame) {
            Pause(true);
        }

        spriteRenderer.sprite = effect[index];
    }

}
