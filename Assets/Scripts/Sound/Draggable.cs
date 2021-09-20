using System.Collections;
using System.Collections.Generic;
using UnityEngine;

[RequireComponent(typeof(LineRenderer))]
[RequireComponent(typeof(BoxCollider2D))]
public class Draggable : MonoBehaviour {

    [SerializeField] [Range(0f, 1f)] public float value = 0f;
    [HideInInspector] public LineRenderer lineRenderer;
    [HideInInspector] public BoxCollider2D boxCollider;

    public bool isDragging = false;
    [SerializeField] [ReadOnly] protected Vector2 mousePos;

    public GameObject point;

    public static float dragFactor = 0.2f;

    void Awake() {
        gameObject.layer = LayerMask.NameToLayer("UI");
        lineRenderer = GetComponent<LineRenderer>();
        boxCollider = GetComponent<BoxCollider2D>();
        boxCollider.size = new Vector2(0.25f, 0.25f);
    }

    void Update() {
        if (isDragging) {
            if (Input.GetMouseButtonUp(0)) {
                isDragging = false;
            }
            else {
                Drag();
            }
        }

        boxCollider.offset = (Vector2)lineRenderer.GetPosition(1) - (Vector2)transform.position;
        point.transform.position = lineRenderer.GetPosition(1);
    }

    void OnMouseDown() {
        isDragging = true;
        mousePos = Camera.main.ScreenToWorldPoint(Input.mousePosition);
    }

    void Drag() {

        Vector2 newMousePos = Camera.main.ScreenToWorldPoint(Input.mousePosition);
        float valueIncrement = newMousePos.y - mousePos.y;
        value += valueIncrement * dragFactor;
        // value = 3.5f * (newMousePos.y - transform.position.y);
        if (value > 1f) { value = 1f; }
        if (value < 0f) { value = 0f; }
        mousePos = newMousePos;

    }
}
