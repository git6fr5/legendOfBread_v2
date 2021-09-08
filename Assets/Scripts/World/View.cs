using System.Collections;
using System.Collections.Generic;
using UnityEngine;

public class View : MonoBehaviour {

    Vector3 origin;
    public Transform followTransform;
    public bool isOverworld;

    [Range(0f, 5f)] public float shakeStrength = 1f;
    public float shakeDuration = 0.5f;
    float elapsedTime = 0f;
    public bool shake;
    public AnimationCurve curve;

    void Awake() {
        origin = transform.position;
        followTransform.parent = transform;
    }

    void Update() {
        if (isOverworld == true) {
            Follow();
        }
        else {
            followTransform.parent = transform;
            transform.position = origin;
        }

        if (shake) {
            shake = Shake();
        }
    }

    void Follow() {
        if (followTransform.parent = transform) {
            followTransform.parent = null;
        }
        transform.position = new Vector3(followTransform.position.x, followTransform.position.y, origin.z);
    }

    bool Shake() {
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
