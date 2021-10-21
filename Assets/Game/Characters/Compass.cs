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
