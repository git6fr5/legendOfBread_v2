/* --- Libraries --- */
using System.Collections;
using System.Collections.Generic;
using UnityEngine;

/* --- Structs --- */
using Direction = Loader.Direction;
using Rotation = Loader.Rotation;

public class Entity : MonoBehaviour {

    public Vector2Int vectorID;
    [ReadOnly] public Vector2Int gridPosition; // The position it is spawned at.

    public void OnSpawn(Vector2Int gridPosition) {
        gameObject.SetActive(true);
        this.gridPosition = gridPosition;
    }

    public bool MatchGridPosition(Vector2Int gridPosition) {
        return (this.gridPosition.x == gridPosition.x && this.gridPosition.y == gridPosition.y);
    }

    public virtual void ApplyDirection(Level level, Direction direction) {

    }

    public virtual void ApplyRotation(Level level, Rotation rotation) {

    }

}
