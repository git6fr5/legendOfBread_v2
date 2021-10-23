using System.Collections;
using System.Collections.Generic;
using UnityEngine;

[RequireComponent(typeof(BoxCollider2D))]
public class Pedal : MonoBehaviour {

    public int increment;

    public Synth synth;

    void OnMouseDown() {
        synth.octaveShift += increment;
    }

}
