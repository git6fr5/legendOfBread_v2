using System.Collections;
using System.Collections.Generic;
using UnityEngine;
using UnityEngine.Tilemaps;

using ORIENTATION = Compass.ORIENTATION;
using DIRECTION = Compass.DIRECTION;
using EXIT = Compass.EXIT;

public class Janitor : MonoBehaviour {

    /* --- Border Cleaning --- */
    // The organization format.
    static DIRECTION[] inputOrder = new DIRECTION[] {
        DIRECTION.LEFT_UP, DIRECTION.UP, DIRECTION.UP_RIGHT,
        DIRECTION.LEFT, DIRECTION.CENTER, DIRECTION.RIGHT,
        DIRECTION.DOWN_LEFT, DIRECTION.DOWN, DIRECTION.DOWN_RIGHT,
    };

    /* --- Methods --- */
    // Reorder the border tiles with respect to the input order.
    public static TileBase[] BorderOrder(TileBase[] tiles) {
        TileBase[] tempTiles = new TileBase[(int)DIRECTION.count + 1];
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
                if (grid[i][j] != (int)DIRECTION.CENTER && grid[i][j] != (int)DIRECTION.DOWN_LEFT_UP_RIGHT) {
                    grid[i][j] = CleanBorderCell(grid, sizeVertical, sizeHorizontal, i, j);
                }
            }
        }
        return grid;
    }

    // The cleaning logic flow.
    static int CleanBorderCell(int[][] grid, int sizeVertical, int sizeHorizontal, int i, int j) {
        int val = 0;
        // Itterates through adjacent tiles and checks if they are filled.
        if (CellIsFilled(grid, i + 1, j)) { val += 8; }
        if (CellIsFilled(grid, i - 1, j)) { val += 2; }
        if (CellIsFilled(grid, i, j + 1)) { val += 1; }
        if (CellIsFilled(grid, i, j - 1)) { val += 4; }
        return val;
    }

    // Checks if a cell is filled with a center tile.
    static bool CellIsFilled(int[][] grid, int i, int j) {
        // || grid[i][j] == (int)DIRECTION.DOWN_LEFT_UP_RIGHT)
        if (Geometry.IsValid(new int[] { i, j }, grid) && grid[i][j] == (int)DIRECTION.CENTER) {
            return true;
        }
        return false;
    }

    /* --- Exit Cleaning --- */
    public static int[][] AddExits(int[][] grid, EXIT exits, int border) {
        int[][] exitCoords = ExitCoordinates(grid, exits, border);
        for (int i = 0; i < exitCoords.Length; i++) {
            grid[exitCoords[i][0]][exitCoords[i][1]] = (int)DIRECTION.DOWN_LEFT_UP_RIGHT;
        }
        return grid;
    }

    public static Exit[] AddExitObjects(Exit nullExit, Transform gridTransform, int[][] grid) {

        List<int[]> exitCoords = new List<int[]>();
        for (int i = 0; i < grid.Length; i++) {
            for (int j = 0; j < grid[0].Length; j++) {
                if (grid[i][j] == (int)DIRECTION.DOWN_LEFT_UP_RIGHT) {
                    exitCoords.Add(new int[] { i, j });
                }
            }
        }

        Exit[] roomExits = new Exit[exitCoords.Count];
        for (int i = 0; i < exitCoords.Count; i++) {
            Vector3 position = Geometry.GridToPosition(exitCoords[i], gridTransform);
            Exit exit = Instantiate(nullExit.gameObject, position, Quaternion.identity).GetComponent<Exit>();

            int x = 0; int y = 0;
            if (exitCoords[i][0] < Mathf.Floor(grid.Length / 3)) {
                y = -1;
            }
            else if (exitCoords[i][0] >= Mathf.Floor(2 * grid.Length / 3)) {
                y = 1;
            }
            if (exitCoords[i][1] < Mathf.Floor(grid[0].Length / 3)) {
                x = -1;
            }
            else if (exitCoords[i][1] > Mathf.Ceil(2 * grid[0].Length / 3)) {
                x = 1;
            }

            exit.id = new int[] { y, x };
            roomExits[i] = exit;
        }
        return roomExits;
    }

    static int[][] ExitCoordinates(int[][] grid, EXIT exits, int border) {
        List<ORIENTATION> orientations = Compass.ExitToOrientations(exits);
        int[][] exitCoords = new int[orientations.Count][];
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
                if (i == 0) { i = border; }
                j = (int)Mathf.Floor(grid[0].Length / 2);
            }
            exitCoords[k] = new int[] { i, j };
        }
        return exitCoords;
    }

    /* --- Object Loading --- */
    // Load a set of controllers based on the grid.
    public static Controller[] LoadControllers(Transform gridTransform, int[][] grid, Controller[] controllers) {
        // Find out where challenges are and place them.
        List<Controller> loadedControllers = new List<Controller>();
        for (int i = 0; i < grid.Length; i++) {
            for (int j = 0; j < grid[0].Length; j++) {
                // Instantiate the appropriate controller by its index.
                int index = grid[i][j];
                // Check that its a valid index.
                if (index < controllers.Length && controllers[index] != null) {
                    Vector3 position = (Vector3)Geometry.GridToPosition(new int[] { i, j }, gridTransform);
                    Controller controller = Instantiate(controllers[index].gameObject, position, Quaternion.identity, gridTransform).GetComponent<Controller>();
                    controller.gameObject.SetActive(true);
                    loadedControllers.Add(controller);
                }
            }
        }
        return loadedControllers.ToArray();
    }


}
