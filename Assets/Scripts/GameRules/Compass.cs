using System.Collections;
using System.Collections.Generic;
using UnityEngine;

public class Compass : MonoBehaviour {

    /* --- Enums --- */
    public enum ORIENTATION {
        RIGHT, UP, LEFT, DOWN, count
    }

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

}
