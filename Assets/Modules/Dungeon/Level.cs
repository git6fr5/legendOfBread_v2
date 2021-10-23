/* --- Libraries --- */
using System.Collections;
using System.Collections.Generic;
using UnityEngine;
using UnityEngine.Tilemaps;

/// <summary>
/// Level.
/// </summary>
public class Level : MonoBehaviour {

    [SerializeField] [ReadOnly] public int height;
    [SerializeField] [ReadOnly] public int width;
    public bool difficult;

    public List<Entity> entities;
    public Tilemap floorMap;
    public Tilemap borderMap;

    public void SetFloor(Environment environment) {

        environment.SetFloorTile();

        for (int i = -1; i < height+1; i++) {
            for (int j = -1; j < width+1; j++) {
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

    public void SetBorder(Environment environment) {

        for (int i = -1; i < height+1; i++) {
            for (int j = -1; j < width+1; j++) {
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

    public enum Location {
        Right, Up, Left, Down
    }

    /* --- Dictionaries --- */
    public Dictionary<Location, Vector2> loc_vec = new Dictionary<Location, Vector2>() {
        { Location.Down, new Vector2(0.5f, 0f) },
        // Note: technically this is the up arrow, but the coordinates on the y axis are backwards.
        { Location.Left, new Vector2(0f, 0.5f) },
        { Location.Right, new Vector2(1f, 0.5f) },
        { Location.Up, new Vector2(0.5f, 1f) }
    };

    public void AddExit(int size, Location location, int offset) {

        Vector2 vector = loc_vec[location] * (size);
        if (vector.x == 0f) {
            vector.x = -1f;
        }
        if (vector.y == 0f) {
            vector.y = -1f;
        }


        Vector3Int tilePosition = GridToTileMap((int)vector.y, (int)vector.x);
        print(tilePosition);
        borderMap.SetTile(tilePosition, null);

    }

    /* --- Tilemap Construction --- */
    // Converts a grid coordinate to tile map position.
    public static Vector3Int GridToTileMap(int i, int j) {
        return new Vector3Int(j, i, 0);
    }

    public Vector3 GridToWorld(Vector2Int gridPosition) {
        return new Vector3(gridPosition.x + 0.5f, height - gridPosition.y - 0.5f, 0f) + transform.position;
    }

    public static Vector3 SnapToGrid(Vector3 position) {
        float snappedX = Mathf.Floor(position.x) + 0.5f;
        float snappedY = Mathf.Round(position.y);
        return new Vector3(snappedX, snappedY, 0f);
    }

}
