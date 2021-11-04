/* --- Libraries --- */
using System;
using System.Collections;
using System.Collections.Generic;
using UnityEngine;
using UnityEngine.Events;
using UnityEngine.Tilemaps;
using LDtkUnity;

/// <summary>
/// Loads a level from the lDtk Data into the level using the environment.
/// </summary>
public class Loader : MonoBehaviour {

    // Grid Size
    public static int DefaultGridSize = 16;

    /* --- Events --- */
    public UnityEvent OnLoad = new UnityEvent();

    /* --- Structs --- */
    public struct LDtkTileData {

        /* --- Properties --- */
        public Vector2Int vectorID;
        public Vector2Int gridPosition;
        public Vector2Int offsetPosition;
        public int index;
        public int rotation;

        /* --- Constructor --- */
        public LDtkTileData(Vector2Int vectorID, Vector2Int gridPosition, Vector2Int offsetPosition, int index = 0, int rotation = 0) {
            this.gridPosition = gridPosition;
            this.vectorID = vectorID;
            this.index = index;
            this.rotation = rotation;
            this.offsetPosition = offsetPosition;
        }

    }

    public Stream stream;
    [SerializeField] [ReadOnly] public int id;

    /* --- Unity --- */
    // Runs once before the first frame.
    public virtual void Start() {

    }

    protected Level GetLevelByID(LDtkComponentProject lDtkData, int id) {
        // Get the json file from the LDtk Data.
        LdtkJson json = lDtkData.FromJson();

        // Grab the level by the id.
        if (id < json.Levels.Length && id >= 0) {
            return json.Levels[id];
        }
        Debug.Log("Could not find level");
        return null;
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
    protected void LoadTiles(Tilemap tilemap, TileBase tile, int height, int width) {
        for (int i = -1; i < height + 1; i++) {
            for (int j = -1; j < width + 1; j++) {
                // Set the tile.
                tilemap.SetTile(new Vector3Int(j, i, 0), tile);
            }
        }
    }

    // Returns the vector ID's of all the tiles in the layer.
    protected List<LDtkTileData> LoadLayer(LDtkUnity.Level ldtkLevel, string layerName, int gridSize, List<LDtkTileData> layerData = null) {

        if (layerData == null) { layerData = new List<LDtkTileData>(); }

        LDtkUnity.LayerInstance layer = GetLayer(ldtkLevel, layerName);
        if (layer != null) {
            // Itterate through the tiles in the layer and get the directions at each position.
            for (int index = 0; index < layer.GridTiles.Length; index++) {

                // Get the tile at this index.
                LDtkUnity.TileInstance tile = layer.GridTiles[index];

                // Get the position that this tile is at.
                Vector2Int gridPosition = tile.UnityPx / DefaultGridSize;
                Vector2Int vectorID = new Vector2Int((int)(tile.Src[0]), (int)(tile.Src[1])) / gridSize;
                Vector2Int offsetPosition = (tile.UnityPx / gridSize) - gridPosition * (DefaultGridSize / gridSize);

                // Construct the data piece.
                LDtkTileData tileData = new LDtkTileData(vectorID, gridPosition, offsetPosition, index);
                layerData.Add(tileData);
            }

        }
        return layerData;
    }

    protected Vector2Int? FindInLayerAtLocation(Vector2Int location, List<LDtkTileData> data) {
        for (int i = 0; i < data.Count; i++) {
            if (data[i].gridPosition == location) {
                return (Vector2Int?)data[i].vectorID;
            }
        }
        return null;
    }

    protected void SetStream() {
        if (stream != null) {
            stream.SetText(id.ToString());
        }
    }

}
