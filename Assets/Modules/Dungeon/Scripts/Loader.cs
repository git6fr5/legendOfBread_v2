/* --- Libraries --- */
using System;
using System.Collections;
using System.Collections.Generic;
using UnityEngine;
using UnityEngine.Events;
using UnityEngine.Tilemaps;
using LDtkUnity;

/* --- Definitions --- */
using LDtkRoom = LDtkUnity.Level;
using LDtkMap = LDtkUnity.Level;

/// <summary>
/// Loads a level from the lDtk Data into the level using the environment.
/// </summary>
public class Loader : MonoBehaviour {

    /* --- Static Variable --- */
    // Layer Names
    public static string EntityLayer = "Entity";
    public static string DifficultLayer = "Difficult";
    public static string DirectionLayer = "Direction";

    // Grid Size
    public static int DefaultGridSize = 16;

    /* --- Events --- */
    public UnityEvent OnLoad = new UnityEvent();

    /* --- Structs --- */
    public struct LDtkTileData {

        public Vector2Int gridPosition;
        public Vector2Int vectorID;
        public int index;

        public LDtkTileData(Vector2Int gridPosition, Vector2Int vectorID, int index = 0) {
            this.gridPosition = gridPosition;
            this.vectorID = vectorID;
            this.index = index;
        }
    }

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
    public LDtkComponentProject lDtkData;
    public Room room;
    public Environment environment;
    public Stream stream;

    /* --- Unity --- */
    // Runs once before the first frame.
    public virtual void Start() {
        OpenRoom(0);
    }

    /* --- Methods --- */
    public void OpenRoom(string str_id) {
        OpenRoom(Int32.Parse(str_id));
    }

    public virtual void OpenRoom(int id) {
        LDtkRoom ldtkRoom = GetRoomByID(lDtkData, id);
        LoadRoom(ldtkRoom);
        SetStream();
        OnLoad.Invoke();
    }

    protected LDtkRoom GetRoomByID(LDtkComponentProject lDtkData, int id) {

        // Get the json file from the LDtk Data.
        LdtkJson json = lDtkData.FromJson();

        // Read the json data.
        DefaultGridSize = (int)json.DefaultGridSize;
        room.height = (int)(json.DefaultLevelHeight / json.DefaultGridSize);
        room.width = (int)(json.DefaultLevelWidth / json.DefaultGridSize);

        // Grab the level by the id.
        if (id < json.Levels.Length && id > 0) {
            return json.Levels[id];
        }
        Debug.Log("Could not find room");
        return null;
    }

    protected void LoadRoom(LDtkRoom ldtkRoom) {

        ResetRoom();
        if (ldtkRoom != null) {
            Debug.Log("Loading the next level.");

            // Load the entity data.
            List<LDtkTileData> entityData = LoadLayer(ldtkRoom, EntityLayer);
            if (room.isDifficult) { entityData = LoadLayer(ldtkRoom, DifficultLayer, entityData); }

            // Load the directional data.
            List<LDtkTileData> directionData = LoadLayer(ldtkRoom, DirectionLayer);

            // Instatiantate and set up the entities using the data.
            List<Entity> entities = LoadEntities(entityData);
            LoadDirections(entities, directionData);

            room.entities = entities;
        }

    }

    private void ResetRoom() {
        // Reset the entities.
        Debug.Log("Resetting the room.");
        if (room.entities != null) {
            for (int i = 0; i < room.entities.Count; i++) {
                if (room.entities[i] != null) {
                    Destroy(room.entities[i].gameObject);
                }
            }
            room.entities = new List<Entity>();
        }
        // Reset the exits.
        if (room.exits != null) {
            for (int i = 0; i < room.exits.Count; i++) {
                if (room.exits[i] != null) {
                    Destroy(room.exits[i].gameObject);
                }
            }
            room.exits = new List<Exit>();
        }

        // Load the tiles.
        environment.RefreshTiles();
        LoadTiles(room.borderMap, environment.borderTile, room.height, room.width);
        LoadTiles(room.floorMap, environment.floorTile, room.height, room.width);
    }

    protected LDtkUnity.LayerInstance GetLayer(LDtkUnity.Level ldtkLevel, string layerName) {
        // Itterate through the layers in the level until we find the layer.
        for (int i = 0; i < ldtkLevel.LayerInstances.Length; i++) {
            LDtkUnity.LayerInstance layer = ldtkLevel.LayerInstances[i];
            if (layer.IsTilesLayer && layer.Identifier == layerName) {
                return layer;
            }
        }
        return null;
    }

    // Set all the tiles in a tilemap.
    public void LoadTiles(Tilemap tilemap, TileBase tile, int height, int width) {
        for (int i = -1; i < height + 1; i++) {
            for (int j = -1; j < width + 1; j++) {
                // Set the tile.
                tilemap.SetTile(new Vector3Int(j, i, 0), tile);
            }
        }
    }

    // Returns the vector ID's of all the tiles in the layer.
    private List<LDtkTileData> LoadLayer(LDtkUnity.Level ldtkLevel, string layerName, List<LDtkTileData> layerData = null) {

        if (layerData == null) { layerData = new List<LDtkTileData>(); }

        LDtkUnity.LayerInstance layer = GetLayer(ldtkLevel, layerName);
        if (layer != null) {
            // Itterate through the tiles in the layer and get the directions at each position.
            for (int index = 0; index < layer.GridTiles.Length; index++) {

                // Get the tile at this index.
                LDtkUnity.TileInstance tile = layer.GridTiles[index];

                // Get the position that this tile is at.
                Vector2Int gridPosition = tile.UnityPx / DefaultGridSize;
                Vector2Int vectorID = new Vector2Int((int)(tile.Src[0]), (int)(tile.Src[1])) / DefaultGridSize;

                // Construct the data piece.
                LDtkTileData tileData = new LDtkTileData(gridPosition, vectorID);
                layerData.Add(tileData);
            }

        }
        return layerData;
    }

    private List<Entity> LoadEntities(List<LDtkTileData> entityData, List<Entity> entities = null) {

        if (entities == null) { entities = new List<Entity>(); }

        for (int i = 0; i < entityData.Count; i++) {
            // Get the entity based on the environment.
            Entity entityBase = environment.GetEntityByVectorID(entityData[i].vectorID);
            if (entityBase != null) {

                // Instantiate the entity
                Entity newEntity = Instantiate(entityBase.gameObject, room.GridToWorld(entityData[i].gridPosition), Quaternion.identity, room.transform).GetComponent<Entity>();

                // Set up the entity.
                newEntity.gameObject.SetActive(true);
                newEntity.gridPosition = entityData[i].gridPosition;

                // Add the entity to the list
                entities.Add(newEntity);
            }
        }
        print("Loaded this many entities: " + entities.Count + " out of " + entityData.Count);
        return entities;
    }

    private void LoadDirections(List<Entity> entities, List<Loader.LDtkTileData> directionData) {
        for (int i = 0; i < entities.Count; i++) {
            // Itterate throught the directions.
            for (int j = 0; j < directionData.Count; j++) {
                // Check if a direction needs to be applied to this entity.
                if (entities[i].gridPosition == directionData[j].gridPosition && src_direction.ContainsKey(directionData[j].vectorID)) {
                    entities[i].ApplyDirection(room, directionData[j].gridPosition, src_direction[directionData[j].vectorID]);
                }
                // Check if a rotation needs to be applied to this entity.
                else if (entities[i].gridPosition == directionData[j].gridPosition && src_rotation.ContainsKey(directionData[j].vectorID)) {
                    entities[i].ApplyRotation(room, directionData[j].gridPosition, src_rotation[directionData[j].vectorID]);
                }
            }
        }
    }

    protected virtual void SetStream() {
        if (stream != null) {
            stream.SetText(room.id.ToString());
        }
    }

}
