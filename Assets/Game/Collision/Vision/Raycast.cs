/* --- Libraries --- */
using System.Collections;
using System.Collections.Generic;
using UnityEngine;

/// <summary>
/// 
/// </summary>
public class Raycast : MonoBehaviour {

    /* --- Enumerations --- */
    public enum RayDirection {
        Forward,
        Backward
    }

    /* --- Components --- */
    public Collider2D selfCollider;

    /* --- Variables --- */
    [SerializeField] private int precision = 4; // The number of rays.
    [SerializeField] private float length = 1f; // The length of each ray.
    [SerializeField] private float outset = 0.35f; // The offset from the center of each ray.

    /* --- Properties ---*/
    [SerializeField] [ReadOnly] public List<Vector2> rayVectors;
    [SerializeField] [ReadOnly] public List<Vector2> castedVectors = new List<Vector2>();
    [SerializeField] [ReadOnly] public List<Collider2D> castedCollisions = new List<Collider2D>();

    void Start() {
        // Set up the list of rays.
        rayVectors = new List<Vector2>();
        for (int i = 0; i < precision; i++) {
            // Rotate by the appropriate amount.
            Vector2 newVector = Quaternion.Euler(0, 0, i * 360f / (float)precision) * Vector2.right;
            rayVectors.Add(newVector);
        }
    }

    void Update() {
        // Get the ray cast collisions.
        castedVectors = new List<Vector2>();
        castedCollisions = new List<Collider2D>();
        for (int i = 0; i < rayVectors.Count; i++) {
            CircularRay(i);
        }
    }

    private void CircularRay(int orientation) {

        // Set the ray properties.
        Vector3 origin = transform.position + (Vector3)rayVectors[orientation] * outset;
        Vector2 direction = rayVectors[orientation];
        float distance = length - outset;

        // Check if the ray is colliding with anything.
        Color col = Color.green;
        RaycastHit2D[] hits = Physics2D.RaycastAll(origin, direction, distance);
        for (int i = 0; i < hits.Length; i++) {
            if (hits[i].collider != null && hits[i].collider != selfCollider && !hits[i].collider.isTrigger) {
                col = Color.red;
                if (!castedVectors.Contains(rayVectors[orientation])) {
                    castedVectors.Add(rayVectors[orientation]);
                }
                if (!castedCollisions.Contains(hits[i].collider)) {
                    castedCollisions.Add(hits[i].collider);
                }
                break;
            }
        }
        
        Debug.DrawRay(origin, direction * distance, col);

    }

    public Vector2 AdjustMovement(Vector2 vector) {

        Vector2 direction = vector.normalized;

        for (int i = 0; i < castedVectors.Count; i++) {
            if (Vector2.Dot(castedVectors[i], direction) > 0.95f) {
                direction = Quaternion.Euler(0f, 0f, -90f) * direction;
            }
            else if (Vector2.Dot(castedVectors[i], direction) > 0f) {
                direction -= castedVectors[i];
            }
        }

        Vector2 adjustedVector = direction.normalized;
        Debug.DrawRay(transform.position, adjustedVector, Color.white);

        return adjustedVector;

    }

    public static Vector2 SmartPath(Vector2 origin, Vector2 vector, Collider2D selfCollider, Collider2D targetCollider, int depth = 0, int maxDepth = 5) {

        if (depth > maxDepth) {
            return vector;
        }

        // Draw a line between the target and us.
        RaycastHit2D[] hits = Physics2D.RaycastAll(origin, vector.normalized, vector.magnitude);
        Color col = Color.green;
        for (int i = 0; i < hits.Length; i++) {
            if (hits[i].collider != null && hits[i].collider != selfCollider && hits[i].collider != targetCollider && !hits[i].collider.isTrigger) {
                col = Color.red;
            }
        }
        Debug.DrawRay(origin, vector, col);

        if (col == Color.green) {
            return vector;
        }

        Vector2 normal = (Quaternion.Euler(0f, 0f, 90f) * vector).normalized;
        Vector2 firstHalf = vector * 0.5f;

        // Adjust the end of the first half slightly.
        firstHalf += normal * 3f;
        firstHalf = SmartPath(origin, firstHalf, selfCollider, targetCollider, depth + 1, maxDepth);

        Vector2 secondHalf = (vector - firstHalf);
        SmartPath(origin + firstHalf, secondHalf, selfCollider, targetCollider, depth + 1, maxDepth);

        return firstHalf;
    }

}
