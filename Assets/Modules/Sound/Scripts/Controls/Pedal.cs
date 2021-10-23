using System.Collections;
using System.Collections.Generic;
using UnityEngine;
using UnityEditor;

[RequireComponent(typeof(BoxCollider2D))]
public class Pedal : MonoBehaviour {

    public int increment;

    public Wave wave;

    void OnMouseDown() {
        wave.octaveShift += increment;
    }

}
