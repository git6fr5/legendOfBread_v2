using System.Collections;
using System.Collections.Generic;
using UnityEngine;

public class Scorpion : Mob {

    /* --- Constructor --- */
    public Scorpion() {
        id = 2;
    }

    /* --- Controls --- */
    [Range(0, 5)] public int damage;

    /* --- Variables --- */
    Room room;
    Vector3 targetPoint;
    int syncTicks = 300;

    /* --- Unity --- */
    // Runs once before the first frame.
    void Start() {
        // Cache these references.
        room = GameObject.FindWithTag(GameRules.roomTag)?.GetComponent<Room>();
        // Set these parameters.
        orientationVector = Vector2.right;
    }

    /* --- Action Flow --- */
    protected override void Idle() {
        // Move to an adjacent grid in the room
        if (room != null) {
            // Sync the scorpions movement
            if (GameRules.gameTicks % syncTicks == 0 || targetPoint == Vector3.zero) {
                print("syncing");
                int[] coordinate = Geometry.PointToGrid(transform.position, room.transform);
                List<int[]> adjacentCoordinates = Geometry.AdjacentEmptyTiles(coordinate, room.borderGrid, true);
                int[] targetCoordinate = adjacentCoordinates[Random.Range(0, adjacentCoordinates.Count)];
                targetPoint = Geometry.GridToPosition(targetCoordinate, room.transform);
            }
        }
        movementVector = targetPoint - transform.position;
        if (movementVector.magnitude < GameRules.movementPrecision) {
            movementVector = Vector2.zero;
        }
        else {
            orientationVector = new Vector2(Mathf.Sign(movementVector.x), 0);
        }
    }

    /* --- Event Actions --- */
    protected override void OnHit(Hurtbox hurtbox) {
        hurtbox.controller.Hurt(damage);
        // Reset the target point so it moves again.
        targetPoint = Vector3.zero;
    }

}
