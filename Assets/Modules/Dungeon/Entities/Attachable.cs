/* --- Libraries --- */
using System.Collections;
using System.Collections.Generic;
using UnityEngine;

/* --- Structs --- */
using Direction = Loader.Direction;

public class Attachable : Entity {

    public static int maxSearchDepth = 7;

    public override void ApplyDirection(Level level, Direction direction) {
        Fulcrum fulcrum = FindFulcrum(level, gridPosition, direction.direction);
        if (fulcrum != null) {
            fulcrum.Attach(this);
        }
    }

    private Fulcrum FindFulcrum(Level level, Vector2Int gridPosition, Vector2Int searchDirection, int searchDepth = 0) {

        Vector2Int nextGridPosition = gridPosition + searchDirection;

        // Needs to check it is the correct type of object that we run into.
        for (int i = 0; i < level.entities.Count; i++) {
            if (level.entities[i].MatchGridPosition(nextGridPosition) && level.entities[i].GetComponent<Fulcrum>() != null) {
                return level.entities[i].GetComponent<Fulcrum>();
            }
        }

        if (searchDepth < maxSearchDepth) {
            searchDepth = searchDepth + 1;
            return FindFulcrum(level, nextGridPosition, searchDirection, searchDepth);
        }

        return null;
    }

}
