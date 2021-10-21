/* --- Libraries --- */
using System.Collections;
using System.Collections.Generic;
using UnityEngine;
using UnityEngine.Tilemaps;

/// <summary>
/// Level.
/// </summary>
public class Level : MonoBehaviour {

    public List<Entity> entities;
    public Tilemap floorMap;
    public Tilemap borderMap;

    public void SetFloor(Environment environment, int size) {

        environment.SetFloorTile();

        for (int i = 0; i < size; i++) {
            for (int j = 0; j < size; j++) {
                // Get the position and tile.
                Vector3Int tilePosition = GridToTileMap(i, j);
                // print(tilePosition);
                TileBase tile = environment.floorTile;

                // Set the tile.
                // print("Setting tile");
                floorMap.SetTile(tilePosition, tile);
            }
        }

    }

    public void SetBorder(Environment environment, int size) {

        for (int i = -1; i < size+1; i++) {
            for (int j = -1; j < size+1; j++) {
                // Get the position and tile.
                Vector3Int tilePosition = GridToTileMap(i, j);
                // print(tilePosition);
                RuleTile tile = environment.borderTile;

                // Set the tile.
                // print("Setting tile");
                borderMap.SetTile(tilePosition, tile);
                
            }
        }

    }

    /* --- Tilemap Construction --- */
    // Converts a grid coordinate to tile map position.
    public static Vector3Int GridToTileMap(int i, int j) {
        return new Vector3Int(j, i, 0);
    }

}
