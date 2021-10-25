using System.Collections;
using System.Collections.Generic;
using UnityEngine;

public class Raycast : MonoBehaviour {

    public float distance;
    public float scale;

    List<Vector2> rayVectors = new List<Vector2>() { Vector2.right, Vector2.up, Vector2.left, Vector2.down };
    public List<int> castCollisions = new List<int>();

    public int precision = 6;
    public float length = 1f;
    public float outset = 0.65f;

    public Collider2D selfCollider;

    public enum RayDirection {
        Forward,
        Backward
    }

    void Start() {
        rayVectors = new List<Vector2>();
        for (int i = 0; i < precision; i++) {
            Vector2 newVector = Quaternion.Euler(0, 0, i * 360f / (float)precision) * Vector2.right;
            rayVectors.Add(newVector);
        }
    }

    void Update() {

        castCollisions = new List<int>();

        //for (int i = 0; i < rayVectors.Count; i++) {
        //    for (int j = 0; j < precision; j++) {
        //        LinearRay(i, j, RayDirection.Forward);
        //        LinearRay(i, j, RayDirection.Backward);
        //    }
        //}

        for (int i = 0; i < rayVectors.Count; i++) {
            CircularRay(i);
        }
    }

    private void LinearRay(int orientation, int index, RayDirection rayDirection) {
        float offset = length * ((float)index / (float)(precision -1) - 1f / 2f);
        if (rayDirection == RayDirection.Backward) {
            offset += GameRules.movementPrecision;
        }

        Vector3 origin = transform.position + (Vector3)rayVectors[orientation] * outset + (Vector3)(Vector2.Perpendicular(rayVectors[orientation]) * offset);
        Vector2 direction = rayVectors[orientation];
        float distance = 0.3f;

        Color col = Color.blue;
        if (rayDirection == RayDirection.Backward) {
            col = Color.yellow;
            origin = origin + (Vector3)(direction * distance);
            direction *= -1;
        }

        Debug.DrawRay(origin, direction * distance, col);
        RaycastHit2D hit = Physics2D.Raycast(origin, direction, distance);
        if (hit.collider != selfCollider && hit.collider != null && !hit.collider.isTrigger) {
            castCollisions.Add(orientation);
        }
    }

    private void CircularRay(int orientation) {

        Vector3 origin = transform.position + (Vector3)rayVectors[orientation] * outset;
        Vector2 direction = rayVectors[orientation];

        Color col = Color.green;

        RaycastHit2D hit = Physics2D.Raycast(origin, direction, distance);
        if (hit.collider != selfCollider && hit.collider != null && !hit.collider.isTrigger) {
            col = Color.red;
            castCollisions.Add(orientation);
        }
        Debug.DrawRay(origin, direction * distance, col);


    }

}
