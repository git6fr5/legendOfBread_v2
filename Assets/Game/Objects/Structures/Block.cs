/* --- Libraries --- */
using System.Collections;
using System.Collections.Generic;
using UnityEngine;

public class Block : MonoBehaviour {

    /* --- Components --- */
    public Raycast raycast;
    public Mesh mesh;

    /* --- Properties --- */
    public List<Block> group = new List<Block>();
    [HideInInspector] public List<Block> shadowGroup = new List<Block>();

    void Update() {
        Group();
        ShadowGroup();
    }

    private void Group() {
        group = new List<Block>();
        for (int i = 0; i < raycast.castedCollisions.Count; i++) {
            if (raycast.castedCollisions[i] != null && raycast.castedCollisions[i]?.transform.parent.GetComponent<Block>() != null) {
                Block adjacentBlock = raycast.castedCollisions[i].transform.parent.GetComponent<Block>();
                group.Add(adjacentBlock);
            }
        }

    }

    private void ShadowGroup() {
        for (int i = 0; i < shadowGroup.Count; i++) {
            if (shadowGroup[i] != null && !group.Contains(shadowGroup[i]) && shadowGroup[i] != this) {
                group.Add(shadowGroup[i]);
            }
        }
    }
    
}
