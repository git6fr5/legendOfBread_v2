/* --- Libraries --- */
using System.Collections;
using System.Collections.Generic;
using UnityEngine;

[RequireComponent(typeof(BoxCollider2D))]
public class Hurtbox : MonoBehaviour {

    /* --- Components ---*/
    public Controller controller;

    /* --- Unity --- */
    void Awake() {
        GetComponent<BoxCollider2D>().isTrigger = true;
    }

}
