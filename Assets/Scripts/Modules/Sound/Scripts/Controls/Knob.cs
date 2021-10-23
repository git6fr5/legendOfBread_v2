using System.Collections;
using System.Collections.Generic;
using UnityEngine;

[RequireComponent(typeof(BoxCollider2D))]
public class Knob : MonoBehaviour {

    [SerializeField] [Range(0f, 3f)] protected float rotationFactor = 1f;

    [SerializeField] [Range(0f, 1f)] public float value = 0f;

    [SerializeField] [ReadOnly] [Range(0f, 180f)] protected float minimumRotation = -120f;
    [SerializeField] [ReadOnly] [Range(0f, 360f)] protected float rotationRange = 240f;
    [SerializeField] [ReadOnly] protected bool isTurning = false;
    [SerializeField] [ReadOnly] protected Vector2 mousePos;

    void Awake() {
        gameObject.layer = LayerMask.NameToLayer("UI");
        transform.eulerAngles = Vector3.forward * (-value * rotationRange - minimumRotation);
    }

    void Update() {
        if (isTurning) {
            if (Input.GetMouseButtonUp(0)) {
                isTurning = false;
            }
            else {
                Turn();
            }
        }
        transform.eulerAngles = Vector3.forward * (-value * rotationRange - minimumRotation);
    }

    void OnMouseDown() {
        isTurning = true;
        mousePos = Camera.main.ScreenToWorldPoint(Input.mousePosition);
    }

    void Turn() {

        Vector2 newMousePos = Camera.main.ScreenToWorldPoint(Input.mousePosition);
        float valueIncrement = newMousePos.y - mousePos.y;
        value += valueIncrement * rotationFactor;
        if (value > 1f) { value = 1f; }
        if (value < 0f) { value = 0f; }
        mousePos = newMousePos;

    }


}
