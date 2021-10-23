using System.Collections;
using System.Collections.Generic;
using UnityEngine;
using UnityEditor;

using Shape = Wave.Shape;

[RequireComponent(typeof(BoxCollider2D))]
public class Selector : MonoBehaviour
{
    public Wave wave;
    public Shape shape;

    void OnMouseDown() {
        wave.shape = shape;
    }

    void Update() {
        if (wave.shape == shape) {
            GetComponent<SpriteRenderer>().material.SetFloat("_OutlineWidth", 0.05f);
        }
        else {
            GetComponent<SpriteRenderer>().material.SetFloat("_OutlineWidth", 0f);
        }

    }

}
