using System.Collections;
using System.Collections.Generic;
using UnityEngine;

public class Raycast : MonoBehaviour {

    public float distance;
    public float scale;

    public Vector2[] rayVectors;
    List<BoxCollider2D> rays = new List<BoxCollider2D>();
    Dictionary<Collider2D, Vector2> casts = new Dictionary<Collider2D, Vector2>();

    void Start() {
        for (int i = 0; i < rayVectors.Length; i++) {
            BoxCollider2D newRay = gameObject.AddComponent<BoxCollider2D>();
            newRay.offset = distance * rayVectors[i];
            newRay.isTrigger = true;
            newRay.size = new Vector2(Mathf.Abs(rayVectors[i].y) * scale + GameRules.movementPrecision, Mathf.Abs(rayVectors[i].x) * scale + GameRules.movementPrecision);
            rays.Add(newRay);
        }
    }

    void Update() {
        // print(casts.Count);
    }

    void OnTriggerStay2D(Collider2D collider) {

        if (!collider.isTrigger) {
            if (casts.ContainsKey(collider)) {
                casts[collider] = collider.transform.position - transform.position;
            }
            else {
                casts.Add(collider, collider.transform.position - transform.position);
            }
        }
    }

    void OnTriggerExit2D(Collider2D collider) {
        
        if (casts.ContainsKey(collider)) {
            casts.Remove(collider);
        }
    }

    private float threshold = 0.5f;

    public bool CheckCast(Vector2 vector) {
        foreach (KeyValuePair<Collider2D, Vector2> raycast in casts) {
            // print(Vector2.Dot(vector, raycast.Value));
            if (Vector2.Dot(vector, raycast.Value) > threshold) {
                print("there's something in the way");
                return true;
            }
        }
        return false;
    }

}
