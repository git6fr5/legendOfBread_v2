/* --- Libraries --- */
using System;
using System.Collections;
using System.Collections.Generic;
using UnityEngine;
using UnityEngine.Tilemaps;
    using LDtkUnity;

/* --- Definitions --- */
using LDtkRoom = LDtkUnity.Level;
using LDtkMap = LDtkUnity.Level;
using MapData = Map.MapData;

/* --- Enumerations --- */
using MapSwitch = Map.Switch;



/// <summary>
/// Room.
/// </summary>
public class Room : Loader {

    /* --- Static Variable --- */
    // Layer Names
    public static string EntityLayer = "Entity";
    public static string DifficultLayer = "Difficult";
    public static string DirectionLayer = "Direction";

    /* --- Dictionaries --- */
    // The source sprite to the direction to it points.
    public Dictionary<Vector2Int, Vector2Int> src_direction = new Dictionary<Vector2Int, Vector2Int>() {
        { new Vector2Int(5, 9), Vector2Int.down },
        // Note: technically this is the up arrow, but the coordinates on the y axis are backwards.
        { new Vector2Int(4, 10), Vector2Int.left },
        { new Vector2Int(6, 10), Vector2Int.right },
        { new Vector2Int(5, 10), Vector2Int.up }
    };

    // The source sprite to the direction it rotates.
    public Dictionary<Vector2Int, int> src_rotation = new Dictionary<Vector2Int, int>() {
        { new Vector2Int(6, 9), 1 },
        { new Vector2Int(4, 9), -1 }
    };

    /* --- Components --- */
    [SerializeField] public LDtkComponentProject lDtkData;
    [SerializeField] public Tilemap floorMap; // The map that will display the floor tiles.
    [SerializeField] public Tilemap borderMap; // The map that will display the borders.
    [SerializeField] public Environment environment;
    [SerializeField] public Transform entityParent;

    /* --- Properties --- */
    // Dimensions.
    [SerializeField] [ReadOnly] public int height;
    [SerializeField] [ReadOnly] public int width;
    [SerializeField] [ReadOnly] public Vector2 offset; // Stores the transforms offset for grid-snapping.
    // Settings.
    [SerializeField] [ReadOnly] public bool isDifficult;
    [SerializeField] [ReadOnly] public List<Entity> entities; // The list of currently loaded entities.

    void OnEnable() {
        offset = new Vector2( (Mathf.Abs(transform.position.x + 0.5f)) % 1f, Mathf.Abs((transform.position.y - 0.5f)) % 1f );
        height = 7;
        width = 7;
    }

    /* --- Methods --- */
    public void Open(string str_id) {
        Open(Int32.Parse(str_id));
    }

    public void Open(int id) {

        LDtkRoom ldtkRoom = GetLevelByID(lDtkData, id);
        Reset();
        Refresh();
        Load(ldtkRoom); 
        SetStream();
    }

    public void Reset() {
        // Reset the entities.
        if (entities != null) {
            for (int i = 0; i < entities.Count; i++) {
                if (entities[i] != null) {
                    Destroy(entities[i].gameObject);
                }
            }
            entities = new List<Entity>();
        }
    }

    public void Refresh() {
        // Load the tiles.
        environment.RefreshTiles();
        LoadTiles(borderMap, environment.borderTile, height, width);
        LoadTiles(floorMap, environment.floorTile, height, width);
    }


    public void Load(LDtkRoom ldtkRoom) {

        if (ldtkRoom != null) {

            // Load the entity data.
            List<LDtkTileData> entityData = LoadLayer(ldtkRoom, EntityLayer, DefaultGridSize);
            if (isDifficult) { entityData = LoadLayer(ldtkRoom, DifficultLayer, DefaultGridSize, entityData); }

            // Load the directional data.
            List<LDtkTileData> directionData = LoadLayer(ldtkRoom, DirectionLayer, DefaultGridSize);

            // Instatiantate and set up the entities using the data.
            LoadEntities(entityData);
            LoadDirections(entities, directionData);

        }

    }

    /* --- Methods --- */
    private List<Entity> LoadEntities(List<LDtkTileData> entityData) {

        if (entities == null) { entities = new List<Entity>(); }
        environment.RefreshEntities();

        for (int i = 0; i < entityData.Count; i++) {
            // Get the entity based on the environment.
            Entity entityBase = environment.GetEntityByVectorID(entityData[i].vectorID);
            if (entityBase != null) {

                // Instantiate the entity
                Entity newEntity = Instantiate(entityBase.gameObject, GridToRoom(entityData[i].gridPosition), Quaternion.identity, entityParent).GetComponent<Entity>();

                // Set up the entity.
                newEntity.gameObject.SetActive(true);
                newEntity.gridPosition = entityData[i].gridPosition;

                // Add the entity to the list
                entities.Add(newEntity);
            }
        }
        // print("Loaded this many entities: " + entities.Count + " out of " + entityData.Count);
        return entities;
    }

    private void LoadDirections(List<Entity> entities, List<LDtkTileData> directionData) {
        for (int i = 0; i < entities.Count; i++) {
            // Itterate throught the directions.
            for (int j = 0; j < directionData.Count; j++) {
                // Check if a direction needs to be applied to this entity.
                if (entities[i].gridPosition == directionData[j].gridPosition && src_direction.ContainsKey(directionData[j].vectorID)) {
                    entities[i].ApplyDirection(this, directionData[j].gridPosition, src_direction[directionData[j].vectorID]);
                }
                // Check if a rotation needs to be applied to this entity.
                else if (entities[i].gridPosition == directionData[j].gridPosition && src_rotation.ContainsKey(directionData[j].vectorID)) {
                    entities[i].ApplyRotation(this, directionData[j].gridPosition, src_rotation[directionData[j].vectorID]);
                }
            }
        }
    }

    /* --- Grid Methods --- */
    public Vector3 GridToRoom(Vector2Int gridPosition) {
        return new Vector3(gridPosition.x + offset.x, height - (gridPosition.y + offset.y + 0.5f), 0f) + transform.position;
    }

    public Vector3 SnapToGrid(Vector3 position) {
        float snappedX = Mathf.Round(position.x) + offset.x;
        float snappedY = Mathf.Round(position.y) + offset.y;
        return new Vector3(snappedX, snappedY, 0f);
    }

}
