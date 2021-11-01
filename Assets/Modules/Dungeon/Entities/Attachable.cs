/* --- Libraries --- */
using System.Collections;
using System.Collections.Generic;
using UnityEngine;

public class Attachable : Entity {

    public static int maxSearchDepth = 7;

    public override void ApplyDirection(Room room, Vector2Int gridPosition, Vector2Int direction) {
        Fulcrum fulcrum = FindFulcrum(room, gridPosition, direction);
        if (fulcrum != null) {
            fulcrum.Attach(this);
        }
    }

    private Fulcrum FindFulcrum(Room room, Vector2Int gridPosition, Vector2Int searchDirection, int searchDepth = 0) {

        Vector2Int nextGridPosition = gridPosition + searchDirection;

        // Needs to check it is the correct type of object that we run into.
        for (int i = 0; i < room.entities.Count; i++) {
            if (room.entities[i].gridPosition == nextGridPosition && room.entities[i].GetComponent<Fulcrum>() != null) {
                return room.entities[i].GetComponent<Fulcrum>();
            }
        }

        if (searchDepth < maxSearchDepth) {
            searchDepth = searchDepth + 1;
            return FindFulcrum(room, nextGridPosition, searchDirection, searchDepth);
        }

        return null;
    }

}
