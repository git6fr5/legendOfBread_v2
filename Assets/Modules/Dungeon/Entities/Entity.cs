/* --- Libraries --- */
using System.Collections;
using System.Collections.Generic;
using UnityEngine;

public class Entity : MonoBehaviour {

    public Vector2Int vectorID;
    [ReadOnly] public Vector2Int gridPosition; // The position it is spawned at.

    public void OnSpawn(Vector2Int gridPosition) {
        gameObject.SetActive(true);
        this.gridPosition = gridPosition;
    }

    public virtual void ApplyDirection(Room room, Vector2Int gridPosition, Vector2Int direction) {

    }

    public virtual void ApplyRotation(Room room, Vector2Int gridPosition, int rotation) {

    }

}
