/* --- Libraries --- */
using System.Collections;
using System.Collections.Generic;
using UnityEngine;
using UnityEngine.Tilemaps;

/// <summary>
/// Level.
/// </summary>
public class Level : MonoBehaviour {

    public Entity[] entities;
    public Tilemap border;

    public void SetBackground(Biome biome, int size) {

        for(int i = 0; i < size; i++) {
            for (int j = 0; j < size; j++) {
                // Get the position and tile.
                Vector3Int tilePosition = GridToTileMap(i, j);
                print(tilePosition);
                TileBase tile = biome.border;

                // Set the tile.
                print("Setting tile");
                border.SetTile(tilePosition, tile);
            }
        }

    }

    /* --- Tilemap Construction --- */
    // Converts a grid coordinate to tile map position.
    public static Vector3Int GridToTileMap(int i, int j) {
        return new Vector3Int(j, i, 0);
    }

}
