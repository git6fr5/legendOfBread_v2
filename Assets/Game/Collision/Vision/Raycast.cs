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

    void Update() {

        castCollisions = new List<int>();

        for (int i = 0; i < rayVectors.Count; i++) {
            for (int j = 0; j < precision; j++) {
                NewRay(i, j, RayDirection.Forward);
                NewRay(i, j, RayDirection.Backward);
            }
        }
    }

    private void NewRay(int orientation, int index, RayDirection rayDirection) {
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

}
