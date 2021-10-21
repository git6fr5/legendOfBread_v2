using System.Collections;
using System.Collections.Generic;
using UnityEngine;
using UnityEngine.Tilemaps;

public class Biome : MonoBehaviour {

    // public RuleTile border;
    public TileBase border;

    public Entity[] entities;

    public Entity GetEntityByVectorID(Vector2Int vectorID) {
        for (int i = 0; i < entities.Length; i++) {
            if (entities[i].vectorID == vectorID) {
                print("Found entity");
                return entities[i];
            }
        }
        print("Could not find entity");
        return null;
    }

}
