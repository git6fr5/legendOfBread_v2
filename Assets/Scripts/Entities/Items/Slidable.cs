using System.Collections;
using System.Collections.Generic;
using UnityEngine;

[RequireComponent(typeof(Collider2D))]
public class Slidable : MonoBehaviour {

    public MeshVision meshVision;
    public List<Controller> prevContainer;

    void Awake() {
        prevContainer = new List<Controller>();
    }

    void Update() {
        // For controllers that were just removed.
        for (int i = 0; i < prevContainer.Count; i++) {
            // If the current container is missing an element from the previous container.
            if (!meshVision.container.Contains(prevContainer[i])) {
                prevContainer[i].state.isSliding = false;
            }
        }
        prevContainer = new List<Controller>();
        for (int i = 0; i < meshVision.container.Count; i++) {
            meshVision.container[i].state.isSliding = true;
            prevContainer.Add(meshVision.container[i]);
        }
    }

}
