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
    [SerializeField] private int precision = 6; // The number of rays.
    [SerializeField] private float length = 1f; // The length of each ray.
    [SerializeField] private float outset = 0.65f; // The offset from the center of each ray.

    /* --- Properties ---*/
    [SerializeField] [ReadOnly] public List<Vector2> rayVectors;
    [SerializeField] [ReadOnly] public List<int> castCollisions = new List<int>();

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
        castCollisions = new List<int>();
        for (int i = 0; i < rayVectors.Count; i++) {
            CircularRay(i);
        }

        NextBestDirection(Vector2.right);
    }

    private void CircularRay(int orientation) {

        // Set the ray properties.
        Vector3 origin = transform.position + (Vector3)rayVectors[orientation] * outset;
        Vector2 direction = rayVectors[orientation];

        // Check if the ray is colliding with anything.
        Color col = Color.green;
        RaycastHit2D hit = Physics2D.Raycast(origin, direction, length);
        if (hit.collider != null && hit.collider != selfCollider && !hit.collider.isTrigger) {
            col = Color.red;
            castCollisions.Add(orientation);
        }
        Debug.DrawRay(origin, direction * length, col);

    }

    private void AdjustMovement(Vector2 vector) {

        float magnitude = vector.magnitude;
        Vector2 direction = vector.normalized;

        Vector2 adjustedVector = new Vector2(direction.x, direction.y);

        for (int i = 0; i < rayVectors.Count; i++) {
            if (castCollisions.Contains(i)) {
                if (Vector2.Dot(rayVectors[i], direction) > 0f) {

                    adjustedVector = adjustedVector - rayVectors[i] / castCollisions.Count;

                }
            }
        }

        adjustedVector = adjustedVector.normalized;
        Debug.DrawRay(transform.position, adjustedVector, Color.white);


    }

    public Vector2 NextBestDirection(Vector2 vector) {

        Vector2 direction = vector.normalized;

        float min = -20f;
        int index = -1;

        for (int i = 0; i < rayVectors.Count; i++) {
            if (!castCollisions.Contains(i)) {
                if (Vector2.Dot(rayVectors[i], direction) > min) {
                    index = i;
                    min = Vector2.Dot(rayVectors[i], direction);
                }
            }
        }

        if (index == -1) {
            return Vector2.zero;
        }
        Vector2 adjustedVector = rayVectors[index].normalized;
        Debug.DrawRay(transform.position, adjustedVector, Color.white);

        return adjustedVector;

    }

}
