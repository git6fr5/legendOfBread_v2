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

    /* --- Components --- */
    public LDtkComponentProject lDtkData;
    public Level level;
    public Biome biome;

    /* --- Variables --- */
    [SerializeField] [ReadOnly] private LdtkJson json;
    [SerializeField] [ReadOnly] protected int height;
    [SerializeField] [ReadOnly] protected int width;


    /* --- Unity --- */
    // Runs once before the first frame.
    void Start() {

        SetUpDirectionDict();

        LevelSettings();
        LDtkUnity.Level ldtkLevel = GetLevelByID(id);
        LoadLevel(ldtkLevel);


    }

    // Gets the level settings.
    void LevelSettings() {
        // Get the json file from the LDtk Data.
        json = lDtkData.FromJson();

        // Read the json data.
        height = (int)(json.DefaultLevelHeight / json.DefaultGridSize);
        print(json.DefaultLevelHeight);
        width = (int)(json.DefaultLevelWidth / json.DefaultGridSize);

    }

    public int id;
    public string levelName; // Temporarily to be used in the inspector.
    public static string identifier = "Level_";

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
        if (id < json.Levels.Length) {
            print("Found level " + json.Levels[id].Identifier);
            return json.Levels[id];
        }
        print("Could not find level");
        return null;
    }

    private void LoadLevel(LDtkUnity.Level ldtkLevel) {

        // Load the background.
        // level.SetBackground(environment, height);

        LDtkUnity.LayerInstance directionLayer = null; // Cache this for later.

        // Itterate through the layers in the level and do the entities.
        for (int i = 0; i < ldtkLevel.LayerInstances.Length; i++) {
            LDtkUnity.LayerInstance layer = ldtkLevel.LayerInstances[i];
            if (layer.IsTilesLayer) {
                print("found tile layer");
                print(layer.Identifier);
                if (layer.Identifier == EntityLayer) {
                    LoadEntityLayer(layer);
                }
                else if (layer.Identifier == DirectionLayer) {
                    directionLayer = layer;
                }
            }
        }

        if (directionLayer != null) {

            for (int i = 0; i < entities.Count; i++) {
                if (entities[i].requiresDirection) {
                    print("requires direction");
                    GetDirection(entities[i], directionLayer);
                }
            }
            
        }

    }

    public static string EntityLayer = "Entity";
    public static string DirectionLayer = "Direction";

    public List<Entity> entities = new List<Entity>();

    private void LoadEntityLayer(LDtkUnity.LayerInstance layer) {

        print("Loading entities");

        // Itterate through the tiles in the layer.
        for (int index = 0; index < layer.GridTiles.Length; index++) {
            Entity newEntity = LoadEntity(layer, index);
            if (newEntity != null) {
                entities.Add(newEntity);
            }
        }

        return;
    }

    private Entity LoadEntity(LDtkUnity.LayerInstance layer, int index) {
        LDtkUnity.TileInstance tile = layer.GridTiles[index];

        // Get the source that this tile is pointing to.
        Vector2Int position = tile.UnityPx / (int)json.DefaultGridSize;
        Vector2Int vectorID = new Vector2Int((int)(tile.Src[0]), (int)(tile.Src[1])) / (int)json.DefaultGridSize;
        Entity entity = biome.GetEntityByVectorID(vectorID);

        if (entity != null) {
            print(entity.name);
            print(position);
            Entity newEntity = Instantiate(entity.gameObject, (Vector3)(Vector2)(position) + level.transform.position, Quaternion.identity, level.transform).GetComponent<Entity>();
            newEntity.gameObject.SetActive(true);
            newEntity.gridPosition = position;
            return newEntity;
        }

        return null;
    }

    public Dictionary<Vector2Int, Vector2Int> direction_sourceSprite = new Dictionary<Vector2Int, Vector2Int>();

    void SetUpDirectionDict() {
        direction_sourceSprite.Add(new Vector2Int(5, 9), Vector2Int.up);
        direction_sourceSprite.Add(new Vector2Int(4, 10), Vector2Int.right );
        direction_sourceSprite.Add(new Vector2Int(6, 10), Vector2Int.left );
        direction_sourceSprite.Add(new Vector2Int(5, 10), Vector2Int.down);
    }

    private void GetDirection(Entity entity, LDtkUnity.LayerInstance directionLayer) {

        print(entity.gridPosition);

        // Itterate through the tiles in the layer.
        for (int index = 0; index < directionLayer.GridTiles.Length; index++) {

            LDtkUnity.TileInstance tile = directionLayer.GridTiles[index];
           
            // Get the source that this tile is pointing to.
            Vector2Int position = tile.UnityPx / (int)json.DefaultGridSize;

            if (entity.gridPosition.x == position.x && entity.gridPosition.y == position.y) {
                print("found");

                Vector2Int vectorID = new Vector2Int((int)(tile.Src[0]), (int)(tile.Src[1])) / (int)json.DefaultGridSize;
                print("----- VectorID");
                print(vectorID);

                if (direction_sourceSprite.ContainsKey(vectorID)) {

                    print("contained key");
                    print(direction_sourceSprite[vectorID]);

                    entity.UseDirection(entities, direction_sourceSprite[vectorID]);

                }

            }


        }

    }

}
