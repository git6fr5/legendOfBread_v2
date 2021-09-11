using System.Collections;
using System.Collections.Generic;
using UnityEngine;

using ORIENTATION = Compass.ORIENTATION;

public class Pushable : Structure {

    /* --- Variables --- */
    
    public float pushBuffer = 0.025f; // port this to structure?

    public Vector2 targetPoint; // port this to structure as well?
    public float speed = 3f;
    public float friction;

    public Vector3 origin;
    public float pushedTime = 0f;
    public float maxPushedTime = 2f;

    Rigidbody2D body;

    void Start() {
        body = GetComponent<Rigidbody2D>();
        body.constraints = RigidbodyConstraints2D.FreezeAll;
        body.gravityScale = 0f;
        body.angularDrag = 0f;
        origin = transform.position;
    }

    protected override void Interacting() {
        pushedTime += Time.deltaTime;
        body.velocity = body.velocity - (body.velocity * friction);
        if (Vector2.Distance(transform.position, targetPoint) < GameRules.movementPrecision || pushedTime >= maxPushedTime || body.velocity.magnitude < GameRules.movementPrecision) {
            condition = Condition.Interactable;
            body.velocity = Vector2.zero;
            transform.position = new Vector3(Mathf.Round(transform.position.x), Mathf.Round(transform.position.y), transform.position.z);
            body.constraints = RigidbodyConstraints2D.FreezeAll;
            origin = transform.position;
            pushedTime = 0f;
        }
    }

    protected override void Interact(Player player) {
        ORIENTATION orientation = player.state.orientation;
        Vector3 pusherPosition = player.transform.position;
        if (condition == Condition.Interactable) {
            condition = Condition.Interacting;
            body.constraints = RigidbodyConstraints2D.FreezeRotation;
            if (ValidPushDirection(orientation, pusherPosition)) {
                Vector2 direction = (Vector3)Compass.OrientationVectors[orientation];
                targetPoint = transform.position + (Vector3)direction;
                targetPoint = new Vector2((float)(int)targetPoint.x, (float)(int)targetPoint.y);
                body.velocity = speed * (Vector3)direction;
                friction = 2 * Vector2.Distance(targetPoint, transform.position) / body.velocity.magnitude * Time.deltaTime;
            }
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
