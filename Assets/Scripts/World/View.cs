using System.Collections;
using System.Collections.Generic;
using UnityEngine;

public class View : MonoBehaviour {

    Vector3 origin;
    public bool isOverworld;

    public Overworld overworld;
    public Transform playerTransform;

    [SerializeField] [ReadOnly] Vector2 offset = new Vector2(16f / 2f, 9f / 2f);


    [Range(0f, 5f)] public float shakeStrength = 1f;
    public float shakeDuration = 0.5f;
    float elapsedTime = 0f;
    public bool shake;
    public AnimationCurve curve;

    void Awake() {
        origin = transform.position;
        playerTransform.parent = null;
    }


    void Update() {
        // Check for the overworld
        if (overworld != null) {
            SnapToScene();
        }
        // Shake the camera
        if (shake) {
            shake = Shake();
        }
    }

    void SnapToScene() {

        Transform scene = null;
        float minLength = Mathf.Infinity;
        for (int i = 0; i < overworld.scenes.Length; i++) {
            float length = (playerTransform.position - (overworld.scenes[i].position + (Vector3)offset)).magnitude;
            if (length < minLength) {
                scene = overworld.scenes[i];
                minLength = length;
            }
        }

        for (int i = 0; i < overworld.scenes.Length; i++) {
            overworld.scenes[i].gameObject.SetActive(false);
        }
        scene.gameObject.SetActive(true);
        transform.position = new Vector3(scene.position.x + offset.x, scene.position.y + offset.y, transform.position.z);

    }

    public bool Shake() {
        elapsedTime += Time.deltaTime;
        if (elapsedTime >= shakeDuration) {
            elapsedTime = 0f;
            return false;
        }
        float strength = shakeStrength * curve.Evaluate(elapsedTime / shakeDuration);
        transform.position = transform.position + (Vector3)Random.insideUnitCircle * shakeStrength;
        return true;
    }

}
