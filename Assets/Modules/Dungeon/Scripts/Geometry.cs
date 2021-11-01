using System.Collections;
using System.Collections.Generic;
using UnityEngine;
using UnityEngine.Tilemaps;

/* --- Enumerations --- */
using Shape = Map.Shape;

public class Geometry : MonoBehaviour {

    public static void Carve(Shape shape, int offset, Tilemap tilemap, int height, int width) {
        switch (shape) {
            case (Shape.Square):
                return;
            case (Shape.Screw):
                Screw(tilemap, height, width);
                Rotate(offset, tilemap, height, width);
                return;
            case (Shape.Corridor):
                Corridor(tilemap, height, width);
                Rotate(offset, tilemap, height, width);
                return;
            case (Shape.Island):
                Island(tilemap, height, width);
                Rotate(offset, tilemap, height, width);
                return;
            case (Shape.Hallway):
                if (offset >= 2) {
                    Hallway(tilemap, height, width);
                    Rotate(offset - 2, tilemap, height, width);
                }
                else {
                    InvertedHallway(tilemap, height, width);
                    Rotate(offset - 2, tilemap, height, width);
                }
                return;
            default:
                return;
        }
    }

    private static void Screw(Tilemap tilemap, int height, int width) {
        for (int i = 0; i < height; i++) {
            for (int j = 0; j < height; j++) {
                //
            }
        }
    }

    private static void Corridor(Tilemap tilemap, int height, int width) {
        for (int i = -1; i < height+1; i++) {
            for (int j = -1; j < width+1; j++) {
                if (i == 4) {
                    tilemap.SetTile(new Vector3Int(j, i, 0), null);
                }
            }
        }
    }

    private static void Island(Tilemap tilemap, int height, int width) {
        for (int i = -1; i < height + 1; i++) {
            for (int j = -1; j < width + 1; j++) {
                if (i == 3 && j >= 3) {
                    tilemap.SetTile(new Vector3Int(j, i, 0), null);
                }
                else if (j == 3 && i >= 3) {
                    tilemap.SetTile(new Vector3Int(j, i, 0), null);
                }
            }
        }
    }

    private static void Hallway(Tilemap tilemap, int height, int width) {
        for (int i = -1; i < height + 1; i++) {
            for (int j = -1; j < width + 1; j++) {
                if (i == 3) {
                    tilemap.SetTile(new Vector3Int(j, i, 0), null);
                }
            }
        }
    }

    private static void InvertedHallway(Tilemap tilemap, int height, int width) {
        for (int i = -1; i < height + 1; i++) {
            for (int j = -1; j < width + 1; j++) {
                if (i < 2 || i > 4) {
                    tilemap.SetTile(new Vector3Int(j, i, 0), null);
                }
            }
        }
    }

    private static void Rotate(int rotations, Tilemap tilemap, int height, int width) {

        for (int n = 0; n < rotations; n++) {

            // Store the tilemap as an array.
            TileBase[][] tiles = new TileBase[height + 2][];
            for (int i = -1; i < height + 1; i++) {
                tiles[i + 1] = new TileBase[width + 2];
                for (int j = -1; j < width + 1; j++) {
                    tiles[i + 1][j + 1] = tilemap.GetTile(new Vector3Int(j, i, 0));
                }
            }

            TileBase[][] newTiles = new TileBase[height + 2][];
            for (int i = -1; i < height + 1; i++) {
                newTiles[i + 1] = new TileBase[width + 2];
                for (int j = -1; j < width + 1; j++) {

                    // 0, 0 -> height, 0 -> height, height -> 0, 0
                    // 0, 1 -> 1, height -> height, height-1 -> height-1, 0
                    // i, j -> j, height - i -> height -i, height -j -> height - j, i


                    // newTiles[i + 1][j + 1] = tiles[j + 1][width + 2 - (i + 1) - 1];
                    newTiles[i + 1][j + 1] = tiles[height + 2 - (j + 1) - 1][i + 1];
                }
            }

            for (int i = -1; i < height + 1; i++) {
                for (int j = -1; j < width + 1; j++) {
                    tilemap.SetTile(new Vector3Int(j, i, 0), newTiles[i + 1][j + 1]);
                }
            }

        }
    }

}
