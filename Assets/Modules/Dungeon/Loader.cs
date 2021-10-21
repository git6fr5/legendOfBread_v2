/* --- Libraries --- */
using System;
using System.Collections;
using System.Collections.Generic;
using UnityEngine;
using LDtkUnity;

/// <summary>
/// Loads a level from the lDtk Data into the level using the environment.
/// </summary>
public class Loader : MonoBehaviour {

    /* --- Static Variables --- */
    public static string EntityLayer = "Entity";
    public static string DirectionLayer = "Direction";

    /* --- Dictionaries --- */
    public Dictionary<Vector2Int, Vector2Int> src_direction = new Dictionary<Vector2Int, Vector2Int>() {
        { new Vector2Int(5, 9), Vector2Int.down },
        // Note: technically this is the up arrow, but the coordinates on the y axis are backwards.
        { new Vector2Int(4, 10), Vector2Int.left },
        { new Vector2Int(6, 10), Vector2Int.right },
        { new Vector2Int(5, 10), Vector2Int.up }
    };

    public Dictionary<Vector2Int, int> src_rotation = new Dictionary<Vector2Int, int>() {
        { new Vector2Int(6, 9), 1 },
        { new Vector2Int(4, 9), -1 }
    };


    /* --- Components --- */
    public LDtkComponentProject lDtkData;
    public Level level;
    public Environment environment;
    public Stream stream;

    /* --- Variables --- */
    [SerializeField] [ReadOnly] private LdtkJson json;
    [SerializeField] [ReadOnly] protected int height;
    [SerializeField] [ReadOnly] protected int width;

    public int id;
    public static string identifier = "Level_";


    /* --- Unity --- */
    // Runs once before the first frame.
    void Start() {
        LevelSettings();
        LDtkUnity.Level ldtkLevel = GetLevelByID(id);
        LoadLevel(ldtkLevel);
        SetStream();
    }

    public void Open(string str_id) {
        LevelSettings();
        int id = Int32.Parse(str_id);
        LDtkUnity.Level ldtkLevel = GetLevelByID(id);
        LoadLevel(ldtkLevel);
        SetStream();
    }

    // Gets the level settings.
    void LevelSettings() {
        // Get the json file from the LDtk Data.
        json = lDtkData.FromJson();

        // Read the json data.
        height = (int)(json.DefaultLevelHeight / json.DefaultGridSize);
        width = (int)(json.DefaultLevelWidth / json.DefaultGridSize);

    }

    void SetStream() {
        stream.text = id.ToString();
        stream.SetText(id.ToString());
    }

    private LDtkUnity.Level LoadLevelByName(string levelName) {

        // Get the id from the level name.
        string[] identifiers = levelName.Split('_');

        // If the id is valid then find the level.
        if (identifiers.Length > 1) {
            int id = Int32.Parse(identifiers[1]);
            return GetLevelByID(id);
        }
        print("Could not find level");
        return null;
    }

    private LDtkUnity.Level GetLevelByID(int id) {

        // Grab the level by the id.
        if (id < json.Levels.Length && id > 0) {
            print("Found level " + json.Levels[id].Identifier);
            return json.Levels[id];
        }
        print("Could not find level");
        stream.text = this.id.ToString();
        return null;
    }

    private void LoadLevel(LDtkUnity.Level ldtkLevel) {

        ClearLevel();

        // Load the background.
        level.SetBorder(environment, height);
        level.SetFloor(environment, height);

        if (ldtkLevel == null) {
            return;
        }

        // Load the entities.
        LDtkUnity.LayerInstance entityLayer = GetLayer(ldtkLevel, EntityLayer);
        if (entityLayer != null) {
            level.entities = LoadEntityLayer(entityLayer);
        }

        // Technically this can happen in the above conditional.
        // But it feels cleaner this way.
        LDtkUnity.LayerInstance directionLayer = GetLayer(ldtkLevel, DirectionLayer);
        List<Direction> directions = new List<Direction>();
        List<Rotation> rotations = new List<Rotation>();
        if (directionLayer != null) {
            directions = LoadDirectionLayer(directionLayer);
            rotations = LoadRotationLayer(directionLayer);
        }

        ApplyDirections(level.entities, directions);
        ApplyRotations(level.entities, rotations);

    }

    private void ClearLevel() {
        // Reset the entities.
        if (level.entities != null) {
            for (int i = 0; i < level.entities.Count; i++) {
                Destroy(level.entities[i].gameObject);
            }
            level.entities = new List<Entity>();
        }
    }

    private LDtkUnity.LayerInstance GetLayer(LDtkUnity.Level ldtkLevel, string layerName) {
        // Itterate through the layers in the level until we find the layer.
        for (int i = 0; i < ldtkLevel.LayerInstances.Length; i++) {
            LDtkUnity.LayerInstance layer = ldtkLevel.LayerInstances[i];
            if (layer.IsTilesLayer && layer.Identifier == layerName) {
                return layer;
            }
        }
        return null;
    }

    private List<Entity> LoadEntityLayer(LDtkUnity.LayerInstance layer) {
        // Instantiate a new list of entities
        List<Entity> entities = new List<Entity>();

        // Itterate through the tiles in the layer and loading the appropriate entities.
        for (int index = 0; index < layer.GridTiles.Length; index++) {
            Entity newEntity = LoadEntity(layer, index);
            if (newEntity != null) {
                entities.Add(newEntity);
            }
        }
        return entities;
    }

    private Entity LoadEntity(LDtkUnity.LayerInstance layer, int index) {
        LDtkUnity.TileInstance tile = layer.GridTiles[index];

        // Get the source that this tile is pointing to.
        Vector2Int gridPosition = tile.UnityPx / (int)json.DefaultGridSize;
        Vector2Int vectorID = new Vector2Int((int)(tile.Src[0]), (int)(tile.Src[1])) / (int)json.DefaultGridSize;
        Entity entityBase = environment.GetEntityByVectorID(vectorID);

        if (entityBase != null) {
            Entity newEntity = Instantiate(entityBase.gameObject, GridToWorld(gridPosition) + level.transform.position, Quaternion.identity, level.transform).GetComponent<Entity>();
            newEntity.OnSpawn(gridPosition);
            return newEntity;
        }

        return null;
    }


    public struct Direction {
        public Vector2Int gridPosition;
        public Vector2Int direction;

        public Direction(Vector2Int gridPosition, Vector2Int direction) {
            this.gridPosition = gridPosition;
            this.direction = direction;
        }
    }

    public struct Rotation {
        public Vector2Int gridPosition;
        public int rotation;

        public Rotation(Vector2Int gridPosition, int rotation) {
            this.gridPosition = gridPosition;
            this.rotation = rotation;
        }
    }

    private List<Direction> LoadDirectionLayer(LDtkUnity.LayerInstance layer) {

        List<Direction> directions = new List<Direction>();

        // Itterate through the tiles in the layer and get the directions at each position.
        for (int index = 0; index < layer.GridTiles.Length; index++) {
            LDtkUnity.TileInstance tile = layer.GridTiles[index];

            // Get the position that this tile is at.
            Vector2Int gridPosition = tile.UnityPx / (int)json.DefaultGridSize;
            Vector2Int vectorID = new Vector2Int((int)(tile.Src[0]), (int)(tile.Src[1])) / (int)json.DefaultGridSize;
            if (src_direction.ContainsKey(vectorID)) {
                Direction direction = new Direction(gridPosition, src_direction[vectorID]);
                directions.Add(direction);
            }
            
        }
        return directions;
    }

    private List<Rotation> LoadRotationLayer(LDtkUnity.LayerInstance layer) {

        List<Rotation> rotations = new List<Rotation>();

        // Itterate through the tiles in the layer and get the directions at each position.
        for (int index = 0; index < layer.GridTiles.Length; index++) {
            LDtkUnity.TileInstance tile = layer.GridTiles[index];

            // Get the position that this tile is at.
            Vector2Int gridPosition = tile.UnityPx / (int)json.DefaultGridSize;
            Vector2Int vectorID = new Vector2Int((int)(tile.Src[0]), (int)(tile.Src[1])) / (int)json.DefaultGridSize;
            if (src_rotation.ContainsKey(vectorID)) {
                Rotation rotation = new Rotation(gridPosition, src_rotation[vectorID]);
                rotations.Add(rotation);
            }

        }
        return rotations;
    }

    private void ApplyDirections(List<Entity> entities, List<Direction> directions) {
        for (int i = 0; i < entities.Count; i++) {
            for (int j = 0; j < directions.Count; j++) {
                if (entities[i].MatchGridPosition(directions[j].gridPosition)) {
                    entities[i].ApplyDirection(level, directions[j]);
                }
            }
        }
    }

    private void ApplyRotations(List<Entity> entities, List<Rotation> rotations) {
        for (int i = 0; i < entities.Count; i++) {
            for (int j = 0; j < rotations.Count; j++) {
                if (entities[i].MatchGridPosition(rotations[j].gridPosition)) {
                    entities[i].ApplyRotation(level, rotations[j]);
                }
            }
        }
    }

    public Vector3 GridToWorld(Vector2Int gridPosition) {
        return new Vector3(gridPosition.x + 0.5f, height - gridPosition.y - 0.5f, 0f);
    }
}
