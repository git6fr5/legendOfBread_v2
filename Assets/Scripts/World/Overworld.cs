using System.Collections;
using System.Collections.Generic;
using UnityEngine;
using UnityEngine.Tilemaps;

public class Overworld : MonoBehaviour {

    public Transform[] scenes;

    void Awake() {
        //for (int i = 0; i < scenes.Length; i++) {
        //    foreach (Transform child in scenes[i]) {
        //        if (child.gameObject.name == "Top") {
        //            foreach (Transform grandchild in child) {
        //                if (grandchild.GetComponent<TilemapRenderer>() != null) {
        //                    print("true");
        //                    grandchild.GetComponent<TilemapRenderer>().sortingLayerName = GameRules.midGround;
        //                    grandchild.GetComponent<TilemapRenderer>().sortingOrder = 10;
        //                }
        //            }
        //        }
        //    }
        //}
    }


}
