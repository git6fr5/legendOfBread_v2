using System.Collections;
using System.Collections.Generic;
using UnityEngine;
using UnityEngine.Tilemaps;

using CHANNEL = MapEditor.CHANNEL;
using SHAPE = Geometry.SHAPE;

public class Minimap : MonoBehaviour {

    public TileBase minimapTile;
    public TileBase playerTile;
    public Map map;
    public MapEditor mapEditor;
    public Dungeon dungeon;

    public Tilemap minimapMap;

    public float horOffset;
    public float vertOffset;

    void Start() {
        minimapMap.transform.localPosition = new Vector3(horOffset, vertOffset, 0);
        PrintMinimap(map);
    }

    void Update() {
        PrintMinimap(map);
        if (dungeon != null) {
            PrintMiniplayer(dungeon.id);
        }
    }

    public void PrintMinimap(Map map) {

        for (int i = 0; i < map.size; i++) {
            for (int j = 0; j < map.size; j++) {
                Vector3Int tilePosition = Geometry.GridToTileMap(i, j);

                if (map.shapeGrid[i][j] != (int)SHAPE.EMPTY) {
                    TileBase tile = minimapTile;
                    minimapMap.SetTile(tilePosition, tile);
                }
                else {
                    minimapMap.SetTile(tilePosition, null);
                }
            }
        }
    }

    public void PrintMiniplayer(int[] playerLocation) {
        Vector3Int playerPosition = Geometry.GridToTileMap(playerLocation[0], playerLocation[1]);
        minimapMap.SetTile(playerPosition, playerTile);
    }

}