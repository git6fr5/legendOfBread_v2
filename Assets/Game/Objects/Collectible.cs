using System.Collections;
using System.Collections.Generic;
using UnityEngine;

[RequireComponent(typeof(CircleCollider2D))]
[RequireComponent(typeof(Rigidbody2D))]
public class Collectible : MonoBehaviour {

    /* --- Components --- */
    protected CircleCollider2D area;
    protected Rigidbody2D body;
    public Transform shadow;

    /* --- Variables --- */
    [SerializeField] private float detectionRadius = 1.5f;
    [SerializeField] [ReadOnly] private float collectionRadius = 0.5f;
    [SerializeField] private float collectionSpeed = 3f;
    [SerializeField] [ReadOnly] protected float collectDelay = 0f;

    /* --- Properties --- */
    // Motion
    [SerializeField] [ReadOnly] private Vector2 floatVelocity = new Vector3(0f, 0.3f); // bob up
    [SerializeField] [ReadOnly] private float floatDirection = 1;
    [SerializeField] [ReadOnly] private float floatDuration = 0.75f;
    [HideInInspector] private Coroutine floatTimer;

    protected Vector2 origin;

    void Start() {
        area = GetComponent<CircleCollider2D>();
        area.isTrigger = true;
        area.radius = detectionRadius;

        body = GetComponent<Rigidbody2D>();
        body.isKinematic = false;
        body.gravityScale = 0f;

    }

    void OnEnable() {
        transform.position = origin;
        floatDirection = 1f;
        floatTimer = StartCoroutine(IEFloat(floatDuration));
    }

    void OnTriggerStay2D(Collider2D collider) {
        Player player = collider.GetComponent<Hurtbox>()?.controller?.GetComponent<Player>();
        if (player != null) {
            if (floatTimer != null) {
                StopCoroutine(floatTimer);
                floatTimer = null;
            }
            Vector2 direction = (player.transform.position - transform.position).normalized;
            float magnitude = Mathf.Max(0.25f, detectionRadius - (player.transform.position - transform.position).magnitude);
            // magnitude = magnitude + magnitude / 1.25f * Mathf.Sin(magnitude);
            // direction = (direction + (Vector2)(Quaternion.Euler(0f, 0f, 90f) * ((detectionRadius / magnitude) * direction)));
            body.velocity = direction * magnitude * collectionSpeed;
            body.constraints = RigidbodyConstraints2D.None;
            body.AddTorque(10f);
            shadow.gameObject.SetActive(false);
            transform.localScale = new Vector3(1f, 1f, 1f) / Mathf.Max(1f, magnitude);
            if ((player.transform.position - transform.position).magnitude < collectionRadius) {
                Collect(player);
                // StopAllCoroutines();
            }
        }
    }

    void OnTriggerExit2D(Collider2D collider) {
        Player player = collider.GetComponent<Hurtbox>()?.controller?.GetComponent<Player>();
        if (player != null) {
            if (floatTimer == null && gameObject.activeSelf) {
                floatTimer = StartCoroutine(IEFloat(0f));
                shadow.gameObject.SetActive(true);
            }
        }
    }

    protected virtual void Collect(Player player) {
        //
    }

    /* --- Coroutines --- */
    private IEnumerator IEFloat(float delay) {

        while (gameObject.activeSelf) {
            Float();
            yield return new WaitForSeconds(delay);
        }
        floatTimer = null;
        yield return null;
    }

    private void Float() {
        // Reset the components.
        GetComponent<Rigidbody2D>().constraints = RigidbodyConstraints2D.FreezeRotation;
        transform.eulerAngles = Vector3.zero;
        transform.localScale = new Vector3(1f, 1f, 1f);

        // Float in the opposite direction.
        floatDirection = -floatDirection;
        GetComponent<Rigidbody2D>().velocity = floatDirection * floatVelocity;
    }
}
