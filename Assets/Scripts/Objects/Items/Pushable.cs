using System.Collections;
using System.Collections.Generic;
using UnityEngine;

using ORIENTATION = Compass.ORIENTATION;

public class Pushable : MonoBehaviour
{

    /* --- Variables --- */
    // NPC-Specific Components
    public Vision vision;
    public bool isPushed = false;
    public float pushBuffer = 0.025f;

    public Vector2 targetPoint;
    public float speed = 3f;
    public float friction;

    Rigidbody2D body;
    public Mesh mesh;

    void Start() {
        body = GetComponent<Rigidbody2D>();
        body.constraints = RigidbodyConstraints2D.FreezeAll;
        body.gravityScale = 0f;
        body.angularDrag = 0f;
    }

    void Update() {
        body.mass = 1e-9f;
        Interact();
        if (isPushed) { Pushed(); }
    }

    void OnCollisionStay2D(Collision2D collision) {

    }

    void Interact() {
        body.velocity = body.velocity - (body.velocity * friction);
        Player player = vision.LookFor(GameRules.playerTag)?.controller?.GetComponent<Player>();
        if (player != null && !player.state.isCarrying && Input.GetKeyDown(player.interactKey) && !isPushed) {
            player.Push(this);
        }
    }

    void Pushed() {
        if (Vector2.Distance(transform.position, targetPoint) < GameRules.movementPrecision) {
            isPushed = false;
            body.velocity = Vector2.zero;
            transform.position = targetPoint;
            body.constraints = RigidbodyConstraints2D.FreezeAll;
        }
    }

    public void Push(ORIENTATION orientation, Vector3 pusherPosition) {
        isPushed = true;
        body.constraints = RigidbodyConstraints2D.FreezeRotation;
        if (ValidPushDirection(orientation, pusherPosition)) {
            Vector2 direction = (Vector3)Compass.OrientationVectors[orientation];
            targetPoint = transform.position + (Vector3)direction;
            targetPoint = new Vector2((float)(int)targetPoint.x, (float)(int)targetPoint.y);
            body.velocity = speed * (Vector3)direction;
            friction = 2 * Vector2.Distance(targetPoint, transform.position) / body.velocity.magnitude * Time.deltaTime;
        }
    }

    bool ValidPushDirection(ORIENTATION orientation, Vector3 pusherPosition) {
        Vector2 pushDirection = (Vector3)Compass.OrientationVectors[orientation];
        Vector2 pusherDirection = transform.position - pusherPosition;
        print(pushDirection);
        print(pusherDirection);
        if (Mathf.Abs(pusherDirection.x) > Mathf.Abs(pusherDirection.y)) {
            pusherDirection.y = 0f;
        }
        else {
            pusherDirection.x = 0f;
        }
        if (Vector2.Dot(pushDirection, pusherDirection) > 0) {
            return true;
        }
        return false;
    }

}
