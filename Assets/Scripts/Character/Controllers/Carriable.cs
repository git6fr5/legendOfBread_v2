using System.Collections;
using System.Collections.Generic;
using UnityEngine;

using ORIENTATION = Compass.ORIENTATION;

public class Carriable : Controller {

    /* --- Variables --- */
    // NPC-Specific Components
    public Vision vision;
    public bool isThrowable = false;
    public float throwBuffer = 0.025f;

    public Vector2 position;
    public Vector2 target;
    public float throwTime;
    public float t;
    public float arcHeight = -1f;
    public static float throwDistance = 5f;
    public static float throwSpeed = 5f;
    float jiggleTicks = 0f;

    public bool isThrown = false;
    public bool isJiggling = false;

    void Start() {
        body.mass = 1e9f;
    }

    /* --- Override --- */
    // Sets the action controls.
    protected override void Think() {
        //
        movementVector = Vector2.zero;
        Interact();
        if (isThrown) {
            ThrowArc();
        }
        if (isJiggling) {
            Jiggle();
        }
    }

    void FixedUpdate() {
        
    }

    void OnCollisionEnter2D() {
        if (isThrown) {
            Bounce();
        }
        if (isJiggling) {
            state.orientation = (ORIENTATION)(((int)state.orientation + 2) % ((int)ORIENTATION.count));
        }
    }

    void Interact() {
        Player player = vision.LookFor(GameRules.playerTag)?.controller?.GetComponent<Player>();
        if (player != null && !player.state.isCarrying && Input.GetKeyDown(player.interactKey) && !isThrown) {
            isThrowable = false;
            player.Carry(this);
            StartCoroutine(IEThrowable(throwBuffer));
        }
    }

    IEnumerator IEThrowable(float delay) {
        yield return new WaitForSeconds(delay);
        isThrowable = true;
        yield return null;
    }

    public void Throw(ORIENTATION orientation, Vector3 _position) {
        origin = transform.position;
        position = (Vector2)_position;
        target = position + throwDistance * Compass.OrientationVectors[orientation];
        state.orientation = orientation;
        print(target);
        arcHeight = 0.25f * Mathf.Abs(target.x - origin.x) / 5f;
        throwTime = (Vector2.Distance(target, origin + new Vector2(0, arcHeight)) + Vector2.Distance(origin, origin + new Vector2(0, arcHeight))) / throwSpeed;
        t = 0f;
        isThrown = true;
        StartCoroutine(IEThrown(throwBuffer));
    }

    IEnumerator IEThrown(float delay) {
        yield return new WaitForSeconds(delay);
        GetComponent<Rigidbody2D>().isKinematic = false;
        foreach (Transform child in transform) {
            if (child.tag == GameRules.meshTag) {
                child.GetComponent<Collider2D>().enabled = true;
            }
        }
        yield return null;
    }


    private void ThrowArc() {
        isThrowable = false;
        float T = throwTime;

        Vector2 A = target;
        Vector2 B = origin;

        Vector2[] v = new Vector2[] { A, B, new Vector2((A.x + B.x) / 2, B.y + arcHeight) };
        t = t + Time.deltaTime;
        float x = A.x * t / T + (1 - t / T) * B.x;

        transform.position = new Vector3(x, LagrangeInterpolation(x, v), 0);
        if (Vector2.Distance(transform.position, target) < GameRules.movementPrecision) {
            isThrown = false;
            jiggleTicks = 0f;
            isJiggling = true;
        }
        // transform.RotateAround(transform.position, Vector3.forward, 2f);
        foreach (Transform child in transform) {
            if (child.tag == GameRules.meshTag) {
                Transform hull = child.GetComponent<Mesh>().hull;
                hull.localPosition = new Vector3(0, position.y - transform.position.y - 0.5f, 0);
                float dist = Mathf.Abs(hull.localPosition.y + 0.5f);
                // at dist = 0.5, scale = 1
                // e^ -(d -0.5)
                float scaleX = 2* Mathf.Exp(-Mathf.Log10(dist + 1));
                hull.localScale = new Vector3(scaleX, 1f,1f);
            }
        }
    }

    void Jiggle() {
        jiggleTicks += Time.deltaTime;
        transform.position = transform.position + throwSpeed / 4f * (1-jiggleTicks/ 0.5f) * (Vector3)Compass.OrientationVectors[state.orientation] * Time.deltaTime;
        if (jiggleTicks >= 0.5f) {
            isJiggling = false;
        }
    }

    void Bounce() {
        state.orientation = (ORIENTATION)(((int)state.orientation + 2) % ((int)ORIENTATION.count));
        origin = transform.position;
        print(Mathf.Abs(target.x - origin.x));
        print(Compass.OrientationVectors[state.orientation]);
        target = new Vector2(origin.x, position.y) + Vector2.Distance(target, origin) * Compass.OrientationVectors[state.orientation];
        arcHeight = 0.25f * Mathf.Abs(target.x - origin.x) / 5f;
        throwTime = (Vector2.Distance(target, origin + new Vector2(0, arcHeight)) + Vector2.Distance(origin, origin + new Vector2(0, arcHeight))) / throwSpeed;
        t = 0;
    }

    float LagrangeInterpolation(float x, Vector2[] v) {
        float y = 0f;
        for (int i = 0; i < v.Length; i++) {
            float num = v[i].y;
            float denom = 1f;
            for (int j = 0; j < v.Length; j++) {
                if (i != j) {
                    num = num * (x - v[j].x);
                    denom = denom * (v[i].x - v[j].x);
                }
            }
            y = y + num / denom;
        }
        return y;
    }

}
