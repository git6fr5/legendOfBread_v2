using System.Collections;
using System.Collections.Generic;
using UnityEngine;
using UnityEditor;

public class Distribution : MonoBehaviour
{

    public Draggable[] draggables;
    [SerializeField] protected float lineWidth = 0.1f; // The width of the lines.

    public static float size = 3f;
    public static float bound = 0.5f;

    public Vector2 scale;
    public Vector3 offset;


    void Awake() {

        scale = new Vector2( (size - bound) /5f, (size - bound));
        offset = new Vector3(- 2f * scale.x, -(size - 2f/16f)/ 2f, -1f);

        for (int i = 0; i < draggables.Length; i++) {
            draggables[i].value = Mathf.Exp(-i);
            draggables[i].gameObject.SetActive(true);
            draggables[i].lineRenderer.startWidth = lineWidth;
            draggables[i].lineRenderer.endWidth = lineWidth;
        }
    }

    void Update() {
        Vector3[] linePositions = new Vector3[2];
        for (int i = 0; i < draggables.Length; i++) {
            draggables[i].transform.localPosition = new Vector3(offset.x + i * scale.x, offset.y, offset.z);
            linePositions[0] = draggables[i].transform.position;
            linePositions[1] = draggables[i].transform.position + Vector3.up * draggables[i].value * scale.y;
            draggables[i].lineRenderer.SetPositions(linePositions);
        }
    }

    public float[] GetValues() {
        float[] values = new float[draggables.Length];
        for (int i = 0; i < draggables.Length; i++) {
            values[i] = draggables[i].value;
        }
        return values;
    }

    public void SetValues(float[] values) {
        for (int i = 0; i < draggables.Length; i++) {
            draggables[i].value = values[i];
        }
    }
}
