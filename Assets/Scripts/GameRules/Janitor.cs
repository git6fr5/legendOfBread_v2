using System.Collections;
using System.Collections.Generic;
using UnityEngine;
using UnityEngine.Tilemaps;

using ORIENTATION = Compass.ORIENTATION;

public class Janitor : MonoBehaviour {

    /* --- Enums --- */
    // The ordered layout of the tiles.
    public enum TILE {
        EMPTY,
        CENTER,
        RIGHT,
        UP, UP_RIGHT,
        LEFT, LEFT_RIGHT, LEFT_UP, LEFT_UP_RIGHT,
        DOWN, DOWN_RIGHT, DOWN_UP, DOWN_UP_RIGHT, DOWN_LEFT, DOWN_LEFT_RIGHT, DOWN_LEFT_UP,
        DOWN_LEFT_UP_RIGHT,
        count
    };

    /* --- Border Cleaning --- */
    // The organization format.
    static TILE[] inputOrder = new TILE[] {
        TILE.LEFT_UP, TILE.UP, TILE.UP_RIGHT,
        TILE.LEFT, TILE.CENTER, TILE.RIGHT,
        TILE.DOWN_LEFT, TILE.DOWN, TILE.DOWN_RIGHT,
    };

    /* --- Methods --- */
    // Reorder the border tiles with respect to the input order.
    public static TileBase[] BorderOrder(TileBase[] tiles) {
        TileBase[] tempTiles = new TileBase[(int)TILE.count + 1];
        tempTiles[0] = null;
        for (int i = 0; i < inputOrder.Length; i++) {
            // Put the tile currently at "i", at the correct index
            int nextTileIndex = (int)inputOrder[i];
            tempTiles[nextTileIndex] = tiles[i];
        }
        return tempTiles;
    }

    // Iterate through the grid and clean each cell
    public static int[][] CleanBorder(int[][] grid, int sizeVertical, int sizeHorizontal, int borderVertical, int borderHorizontal) {
        for (int i = borderVertical - 1; i < sizeVertical - (borderVertical - 1); i++) {
            for (int j = borderHorizontal - 1; j < sizeHorizontal - (borderHorizontal - 1); j++) {
                if (grid[i][j] != (int)TILE.CENTER) {
                    grid[i][j] = CleanBorderCell(grid, sizeVertical, sizeHorizontal, i, j);
                }
            }
        }
        return grid;
    }

    // The cleaning logic flow.
    static int CleanBorderCell(int[][] grid, int sizeVertical, int sizeHorizontal, int i, int j) {
        int val = grid[i][j];
        // Itterates through adjacent tiles and checks if they are filled.
        if (CellIsFilled(grid, i + 1, j)) { val += 8; }
        if (CellIsFilled(grid, i - 1, j)) { val += 2; }
        if (CellIsFilled(grid, i, j + 1)) { val += 1; }
        if (CellIsFilled(grid, i, j - 1)) { val += 4; }
        if (val != 0) { val += 1; }
        return val;
    }

    // Checks if a cell is filled with a center tile.
    static bool CellIsFilled(int[][] grid, int i, int j) {
        if (Geometry.IsValid(new int[] { i, j }, grid) && grid[i][j] == (int)TILE.CENTER) {
            return true;
        }
        return false;
    }

    /* --- Exit Cleaning --- */
    public static int[][] AddExits(int[][] grid, TILE exits, int border) {

        List<ORIENTATION> orientations = TileToOrientations(exits);
        for (int k = 0; k < orientations.Count; k++) {
            Vector2 direction = Compass.OrientationVectors[orientations[k]];
            int i; int j;
            if (direction.x != 0) {
                j = (int)(((direction.x + 1) / 2) * (grid[0].Length - (border + 1)));
                if (j == 0) { j = border; }
                i = (int)Mathf.Floor(grid.Length / 2);
            }
            else {
                i = (int)(((-direction.y + 1) / 2) * (grid[0].Length - (border + 1)));
                if (i== 0) { i = border; }
                j = (int)Mathf.Floor(grid[0].Length / 2);
            }
            int[] coord = new int[] { i, j };
            grid[i][j] = (int)TILE.EMPTY;
        }
        return grid;

    }

    public static List<ORIENTATION> TileToOrientations(TILE tile) {
        List<ORIENTATION> orientations = new List<ORIENTATION>();
        for (int i = 0; i < (int)ORIENTATION.count; i++) {
            int check = ((int)tile-1) % (int)Mathf.Pow(2, i+1);
            if (check >= (int)Mathf.Pow(2, i)) {
                orientations.Add((ORIENTATION)i);
            }
        }
        return orientations;
    }

}
