using System.Collections;
using System.Collections.Generic;
using UnityEngine;

using CHANNEL = MapEditor.CHANNEL;

[RequireComponent(typeof(BoxCollider2D))]
[RequireComponent(typeof(SpriteRenderer))]
public class NodeSelector : MonoBehaviour {

    public enum ACCESS { 
        REGULAR,
        LOCKED,
        // LOCKED_0_1,
        // LOCKED_1_0,
        count
    }

    public MapEditor mapEditor;

    public int[] id0;
    public int[] id1;
    public int offset;
    public ACCESS access = ACCESS.REGULAR;
    public bool isActive = false;

    void Awake() {
        Deactivate();
    }

    public void SetActive() {
        isActive = true;
        GetComponent<SpriteRenderer>().material.SetFloat("_Opacity", 1f);
    }

    public void Deactivate() {
        GetComponent<SpriteRenderer>().material.SetFloat("_Opacity", 0.25f);
        isActive = false;
    }

    void OnMouseOver() {
        if (mapEditor.channel == CHANNEL.NODE) {
            if (Input.GetMouseButtonDown(0)) {
                SetActive();
            }
            if (Input.GetMouseButtonDown(1)){
                Deactivate();
            }
        }
    }

}
