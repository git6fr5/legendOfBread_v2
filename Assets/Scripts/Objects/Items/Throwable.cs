using System.Collections;
using System.Collections.Generic;
using UnityEngine;

using ORIENTATION = Compass.ORIENTATION;

[RequireComponent(typeof(Rigidbody2D))]
public class Throwable : Item {

    /* --- Variables --- */
    // NPC-Specific Components
    public Vision vision;
    public bool isCarried = false;
    public bool isThrown = false;
    public float throwBuffer = 0.025f;

    public Vector2 targetPosition;

    Rigidbody2D body;
    public Mesh mesh;

    void Start() {
        body = GetComponent<Rigidbody2D>();
        body.constraints = RigidbodyConstraints2D.FreezeAll;
        body.gravityScale = 0f;
        body.angularDrag = 0f;
    }

    void Update() {

        Interact();
        if (isCarried) { Carried(); }
        else if (isThrown) { Thrown(); }
        else {
            body.constraints = RigidbodyConstraints2D.FreezeAll;
            body.isKinematic = false;
        }
    }

    void Interact() {
        Player player = vision.LookFor(GameRules.playerTag)?.controller?.GetComponent<Player>();
        if (player != null && !player.state.isCarrying && Input.GetKeyDown(player.interactKey) && !isThrown) {
            // Short delay between being picked up, and being throwable.
            isCarried = false;
            StartCoroutine(IECarried(throwBuffer));
            player.Carry(this);
        }
    }

    void Carried() {
        body.isKinematic = true;
        body.constraints = RigidbodyConstraints2D.FreezeRotation;
    }

    void Thrown() {
        body.constraints = RigidbodyConstraints2D.FreezeRotation;
        if (transform.position.y > mesh.hull.position.y + 0.5f) {
            body.velocity = body.velocity - Vector2.up * Time.deltaTime;
        }
        else {
            body.velocity = new Vector3(body.velocity.x * 0.85f, 0f, 0f);
        }
        if (body.velocity.magnitude < GameRules.movementPrecision) {
            isThrown = false;
            body.velocity = Vector3.zero;
        }
        mesh.hull.position = new Vector3(mesh.hull.position.x, targetPosition.y - 0.5f, mesh.hull.position.z);
    }

    IEnumerator IECarried(float delay) {
        yield return new WaitForSeconds(delay);
        isCarried = true;
        yield return null;
    }

    public void Throw(ORIENTATION orientation, Vector3 position) {
        body.isKinematic = false;
        Vector2 direction = (Vector3)Compass.OrientationVectors[orientation];
        targetPosition = position + 2 * (Vector3)direction;
        body.velocity = 5 * (Vector3)direction;
        if ((int)orientation % 2 == 0) {

        }
        else {

        }
        mesh.GetComponent<Collider2D>().enabled = true;

        isCarried = false;
        isThrown = true;
    }

}
