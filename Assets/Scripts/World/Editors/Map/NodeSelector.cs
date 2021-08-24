using System.Collections;
using System.Collections.Generic;
using UnityEngine;

using CHANNEL = MapEditor.CHANNEL;

[RequireComponent(typeof(BoxCollider2D))]
[RequireComponent(typeof(SpriteRenderer))]
public class NodeSelector : MonoBehaviour {

    public MapEditor mapEditor;

    public int[] id0;
    public int[] id1;

    void OnMouseOver() {
        if (mapEditor.channel == CHANNEL.NODE) {
            if (Input.GetMouseButtonDown(0)) {
                GetComponent<SpriteRenderer>().material.SetFloat("_Opacity", 1f);
            }
            if (Input.GetMouseButtonDown(1)){
                GetComponent<SpriteRenderer>().material.SetFloat("_Opacity", 0.5f);
            }
        }
    }

}
