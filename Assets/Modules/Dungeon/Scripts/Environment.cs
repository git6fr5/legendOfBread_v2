using System.Collections;
using System.Collections.Generic;
using UnityEngine;
using UnityEngine.Tilemaps;

/// <summary>
/// Stores specific data on how to generate the level.
/// </summary>
public class Environment : MonoBehaviour {

    /* --- Sub-classes --- */
    public class FloorTile : TileBase {

        Sprite[] sprites;
        public FloorTile(Sprite[] sprites) {
            this.sprites = sprites;
        }

        public override void GetTileData(Vector3Int position, ITilemap tilemap, ref TileData tileData) {
            tileData.sprite = this.sprites[0];
        }

    }

    /* --- Components --- */
    // Entities.
    [SerializeField] public Transform entityParentTransform; // The location to look for the entities.
    // Tiles.
    [SerializeField] public RuleTile borderTile; // The tile used for the border of the level.
    [SerializeField] public Sprite[] floorSprites; // A set of sprites used to tile the floor of the level.

    /* --- Properties --- */
    [SerializeField] [ReadOnly] public FloorTile floorTile; // The set of floor tiles generated from the floor sprites.
    [SerializeField] [ReadOnly] public List<Entity> entities; // The set of entities found from the parent transform.

    /* --- Unity --- */
    // Runs once before the first frame.
    void Start() {
        RefreshTiles();
        RefreshEntities();
    }

    /* --- Tile Methods --- */
    public void RefreshTiles() {
        floorTile = new FloorTile(floorSprites);
    }

    /* --- Entity Methods --- */
    // Refreshes the set of entities.
    void RefreshEntities() {
        entities = new List<Entity>();
        foreach (Transform child in entityParentTransform) {
            FindAllEntitiesInTransform(child);
        }
    }

    // Recursively searches through the transform for all entity components.
    void FindAllEntitiesInTransform(Transform parent) {
        // If we've found an entity, don't go any deeper.
        if (parent.GetComponent<Entity>() != null) {
            entities.Add(parent.GetComponent<Entity>());
        }
        else if (parent.childCount > 0) {
            foreach (Transform child in parent) {
                FindAllEntitiesInTransform(child);
            }
        }
    }

    // Returns the first found entity with a matching ID.
    public Entity GetEntityByVectorID(Vector2Int vectorID) {
        for (int i = 0; i < entities.Count; i++) {
            if (entities[i].vectorID == vectorID) {
                return entities[i];
            }
        }
        return null;
    }

}
