using System.Collections;
using System.Collections.Generic;
using UnityEngine;

public class Block : MonoBehaviour {

    /* --- Components --- */
    public Raycast raycast;

    /* --- Properties --- */
    public List<Block> group = new List<Block>();

    void Update() {
        Group();
    }

    private void Group() {
        group = new List<Block>();
        for (int i = 0; i < raycast.castedCollisions.Count; i++) {
            if (raycast.castedCollisions[i].transform.parent.GetComponent<Block>() != null) {
                Block adjacentBlock = raycast.castedCollisions[i].transform.parent.GetComponent<Block>();
                group.Add(adjacentBlock);
            }
        }

    }
    
}
