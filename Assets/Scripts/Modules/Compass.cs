using System;
using System.Collections;
using System.Collections.Generic;
using System.Linq;
using UnityEngine;

public class Compass : MonoBehaviour {

    /* --- Enums --- */
    public enum ORIENTATION {
        RIGHT, UP, LEFT, DOWN, count
    }

    // The combinations of all the orientations
    public enum DIRECTION {
        EMPTY,
        RIGHT,
        UP, UP_RIGHT,
        LEFT, LEFT_RIGHT, LEFT_UP, LEFT_UP_RIGHT,
        DOWN, DOWN_RIGHT, DOWN_UP, DOWN_UP_RIGHT, DOWN_LEFT, DOWN_LEFT_RIGHT, DOWN_LEFT_UP,
        DOWN_LEFT_UP_RIGHT,
        CENTER,
        count,
        EXIT_RIGHT, EXIT_UP, EXIT_LEFT, EXIT_DOWN,
        fullCount
    };

    /* --- Dictionaries --- */
    public static Dictionary<ORIENTATION, Vector2> OrientationVectors = new Dictionary<ORIENTATION, Vector2>() {
        {ORIENTATION.UP, Vector2.up },
        {ORIENTATION.RIGHT, Vector2.right },
        {ORIENTATION.DOWN, -Vector2.up },
        {ORIENTATION.LEFT, -Vector2.right }
    };

    public static Dictionary<ORIENTATION, Quaternion> OrientationAngles = new Dictionary<ORIENTATION, Quaternion>() {
        {ORIENTATION.UP, Quaternion.Euler(0, 0, 90) },
        {ORIENTATION.RIGHT, Quaternion.Euler(0, 0, 0) },
        {ORIENTATION.DOWN, Quaternion.Euler(0, 0, 270) },
        {ORIENTATION.LEFT, Quaternion.Euler(0, 180, 0) }
    };

    public static Dictionary<Vector2, ORIENTATION> VectorOrientations = new Dictionary<Vector2, ORIENTATION>() {
        { Vector2.up, ORIENTATION.UP },
        { Vector2.right, ORIENTATION.RIGHT },
        { -Vector2.up, ORIENTATION.DOWN },
        { -Vector2.right,ORIENTATION.LEFT }
    };

    /* --- Exit Parsing --- */
    public static int ManhattanDistance(int[] origin, int[] dest) {
        int yDistance = Mathf.Abs(origin[0] - dest[0]);
        int xDistance = Mathf.Abs(origin[1] - dest[1]);
        return yDistance + xDistance;
    }

    public static int GetNewPath(int currIndex, int[] origin, int[] dest) {

        // In the case of drawing a path thats too long.
        if (ManhattanDistance(origin, dest) != 1) {
            return currIndex;
        }

        // Get the direction and edit accordingly.
        // Note that the y-values are swapped to account for "down" being positive
        Vector2 vectorOrientation = new Vector2(dest[1] - origin[1], origin[0] - dest[0]);
        if (VectorOrientations.ContainsKey(vectorOrientation)) {
            ORIENTATION orientation = VectorOrientations[vectorOrientation];
            int direction = (int)Mathf.Pow(2, (int)orientation);
            return EditPath(currIndex, direction, true);
        }

        return currIndex;
    }

    static int EditPath(int currIndex, int direction, bool canAppend = false) {
        if (CheckPath(currIndex, direction)) {
            currIndex -= direction;
        }
        else if (canAppend) {
            currIndex += direction;
        }
        return currIndex;
    }

    public static bool CheckPath(int currIndex, int direction) {
        int check = currIndex % (direction * 2);
        if (check >= direction) {
            return true;
        }
        return false;
    }

    /* --- Transformations --- */
    public static List<ORIENTATION> DirectionToOrientations(DIRECTION direction) {
        List<ORIENTATION> orientations = new List<ORIENTATION>();
        for (int i = 0; i < (int)ORIENTATION.count; i++) {
            int check = ((int)direction) % (int)Mathf.Pow(2, i + 1);
            if (check >= (int)Mathf.Pow(2, i)) {
                orientations.Add((ORIENTATION)i);
            }
        }
        return orientations;
    }

}
