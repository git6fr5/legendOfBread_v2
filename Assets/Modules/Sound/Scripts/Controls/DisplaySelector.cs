using System.Collections;
using System.Collections.Generic;
using UnityEngine;
using UnityEditor;

[RequireComponent(typeof(BoxCollider2D))]
public class DisplaySelector : MonoBehaviour
{
    public Display display;

    public enum Type {
        Period,
        Interval
    }
    public Type type;

    void Update() {
        if (type == Type.Period && display.periodDisplay == true) {
            GetComponent<SpriteRenderer>().material.SetFloat("_OutlineWidth", 0.05f);
        }
        else if (type == Type.Interval && display.periodDisplay == false) {
            GetComponent<SpriteRenderer>().material.SetFloat("_OutlineWidth", 0.05f);
        }
        else {
            GetComponent<SpriteRenderer>().material.SetFloat("_OutlineWidth", 0f);
        }
    }

    void OnMouseDown() {
        if (type == Type.Period) {
            display.periodDisplay = true;
        }
        else {
            display.periodDisplay = false;
        }
    }

}
