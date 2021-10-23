using System.Collections;
using System.Collections.Generic;
using UnityEngine;

public class View : MonoBehaviour {

    Vector3 origin;

    public Transform playerTransform;

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
        // Shake the camera
        if (shake) {
            shake = Shake();
        }
        else {
            transform.position = origin;
        }
    }

    public bool Shake() {
        elapsedTime += Time.deltaTime;
        if (elapsedTime >= shakeDuration) {
            elapsedTime = 0f;
            return false;
        }
        float strength = shakeStrength * curve.Evaluate(elapsedTime / shakeDuration);
        transform.position = origin + (Vector3)Random.insideUnitCircle * shakeStrength;
        return true;
    }

}
