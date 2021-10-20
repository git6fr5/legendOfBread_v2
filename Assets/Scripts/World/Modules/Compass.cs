using System;
using System.Collections;
using System.Collections.Generic;
using System.Linq;
using UnityEngine;

public class Compass : MonoBehaviour {

    /* --- Enums --- */
    public enum Orientation {
        Right, Up, Left, Down, count
    }

    // The combinations of all the orientations
    public enum Direction {
        Empty,
        Right,
        Up, UpRight,
        Left, LeftRight, LeftUp, LeftUpRight,
        Down, DownRight, DownUp, DownUpRight, DownLeft, DownLeftRight, DownLeftUp,
        DownLeftRightUp,
        Center,
        count,
        ExitRight, ExitUp, ExitLeft, ExitDown,
        fullCount
    };

    /* --- Dictionaries --- */
    public static Dictionary<Orientation, Vector2> OrientationVectors = new Dictionary<Orientation, Vector2>() {
        {Orientation.Up, Vector2.up },
        {Orientation.Right, Vector2.right },
        {Orientation.Down, -Vector2.up },
        {Orientation.Left, -Vector2.right }
    };

    public static Dictionary<Orientation, Quaternion> OrientationAngles = new Dictionary<Orientation, Quaternion>() {
        {Orientation.Up, Quaternion.Euler(0, 0, 90) },
        {Orientation.Right, Quaternion.Euler(0, 0, 0) },
        {Orientation.Down, Quaternion.Euler(0, 0, 270) },
        {Orientation.Left, Quaternion.Euler(0, 180, 0) }
    };

    public static Dictionary<Vector2, Orientation> VectorOrientations = new Dictionary<Vector2, Orientation>() {
        { Vector2.up, Orientation.Up },
        { Vector2.right, Orientation.Right },
        { -Vector2.up, Orientation.Down },
        { -Vector2.right,Orientation.Left }
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
            Orientation orientation = VectorOrientations[vectorOrientation];
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
    public static List<Orientation> DirectionToOrientations(Direction direction) {
        List<Orientation> orientations = new List<Orientation>();
        for (int i = 0; i < (int)Orientation.count; i++) {
            int check = ((int)direction) % (int)Mathf.Pow(2, i + 1);
            if (check >= (int)Mathf.Pow(2, i)) {
                orientations.Add((Orientation)i);
            }
        }
        return orientations;
    }

    public static Vector2 SnapVector(Vector2 vector) {
        // Get rid of the the smaller value.
        if (Mathf.Abs(vector.x) > Mathf.Abs(vector.y)) {
            vector.y = 0f;
        }
        else {
            vector.x = 0f;
        }
        return vector.normalized;
    }

}
