using System.Collections;
using System.Collections.Generic;
using UnityEngine;
using UnityEngine.Tilemaps;

public class Environment : MonoBehaviour {

    // We'll use a rule tile for the border,
    // and  an array of sprites for the floor?
    // They both seem pretty compact.
    // public RuleTile border;
    public RuleTile borderTile;
    public Sprite[] floorSprites;
    // [HideInInspector] 
    [ReadOnly] public FloorTile floorTile;

    public Transform entityParentTransform;
    public List<Entity> entities;

    void Start() {
        GetEntitiesFromTransform();
    }

    public Entity GetEntityByVectorID(Vector2Int vectorID) {
        for (int i = 0; i < entities.Count; i++) {
            if (entities[i].vectorID == vectorID) {
                return entities[i];
            }
        }
        return null;
    }

    public class FloorTile : TileBase {

        Sprite[] sprites;
        public FloorTile(Sprite[] sprites) {
            this.sprites = sprites;
        }

        public override void GetTileData(Vector3Int position, ITilemap tilemap, ref TileData tileData) {
            tileData.sprite = this.sprites[0];
        }
    }

    public void SetFloorTile() {
        floorTile = new FloorTile(floorSprites);
    }

    public RuleTile GetBorderTile() {
        return borderTile;
    }

    void GetEntitiesFromTransform() {
        entities = new List<Entity>();
        foreach (Transform child in entityParentTransform) {
            FindAllEntities(child);
        }
    }

    void FindAllEntities(Transform parent) {
        // If we've found an entity, don't go any deeper.
        if (parent.GetComponent<Entity>() != null) {
            entities.Add(parent.GetComponent<Entity>());
        }
        else if (parent.childCount > 0) {
            foreach (Transform child in parent) {
                FindAllEntities(child);
            }
        }
    }

}
