using System.Collections;
using System.Collections.Generic;
using UnityEngine;

public class Pig : Mob {

    /* --- Control Variables --- */
    [HideInInspector] protected Vector3 targetPoint; // The location this slime is travelling to.
    [SerializeField] protected float idleDistance = 3f; // The distance this slime travels during a single idle interval.
    [SerializeField] protected float idleInterval = 5f; // The duration of a single idle interval.
    [SerializeField] protected float idleTicks = 0f; // How long into an idle interval this slime is.
    protected Vector3 previousPosition;
    protected float stuckTime = 0f;
    protected float stuckInterval = 0.25f;

    /* --- Components --- */
    Room room;

    /* --- Unity --- */
    // Runs once before the first frame.
    void Start() {
        // Cache these references.
        room = GameObject.FindWithTag(GameRules.roomTag)?.GetComponent<Room>();
        // Set these parameters.
        orientationVector = Vector2.right;
        previousPosition = transform.position - Vector3.right;
    }

    /* --- Action Flow --- */
    protected override void Idle() {
        // Look for a target, but otherwise move randomly
        Hurtbox target = vision.LookFor(GameRules.playerTag);
        if (vision.LookFor(GameRules.playerTag) != null) {
            // currently don't do anything.
        }

        idleTicks += Time.deltaTime;

        if (idleTicks >= idleInterval || targetPoint == Vector3.zero) {
            orientationVector = Vector2.right; // Compass.OrientationVectors[(Compass.ORIENTATION)Random.Range(0, (int)Compass.ORIENTATION.count)];
            targetPoint = idleDistance * orientationVector + new Vector2((int)transform.position.x, (int)(int)transform.position.y);
            idleTicks = 0f;
        }

        movementVector = targetPoint - transform.position;
        if ((transform.position - previousPosition).magnitude == 0f) {
            stuckTime += Time.deltaTime;
            if (stuckTime >= stuckInterval) {
                NewDirection();
            }
        }
        else {
            stuckTime = 0f;
        }
        if (movementVector.magnitude < GameRules.movementPrecision) {
            movementVector = Vector2.zero;
            transform.position = targetPoint;
            NewDirection();
        }

        previousPosition = transform.position;

    }

    // IEnumerator  

    void NewDirection() {
        orientationVector = Compass.OrientationVectors[(Compass.ORIENTATION)Random.Range(0, (int)Compass.ORIENTATION.count)];

        float length = idleDistance;
        targetPoint = length * orientationVector + new Vector2((int)transform.position.x, (int)(int)transform.position.y);
        if (room != null) {
            print("hello");
            int[] coordinate = Geometry.PointToGrid(targetPoint, room.transform);
            while (!Geometry.WithinBorder(coordinate, room.borderGrid, room.border)) {
                length -= 1f;
                targetPoint = length * orientationVector + new Vector2((int)transform.position.x, (int)(int)transform.position.y);
                coordinate = Geometry.PointToGrid(targetPoint, room.transform);
            }
        }

        idleTicks = 0f;
    }



}
