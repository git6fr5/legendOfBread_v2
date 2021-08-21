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

    // Reorder the border tiles with respect to the input order.
    public static TileBase[] BorderOrder(TileBase[] tiles) {
        TileBase[] tempTiles = new TileBase[(int)DIRECTION.fullCount + 1];
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
                if (grid[i][j] != (int)DIRECTION.CENTER && grid[i][j] < (int)DIRECTION.count) {
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
        print(exits);
        List<ORIENTATION> orientations = Compass.ExitToOrientations(exits);
        int[][] exitCoords = ExitCoordinates(grid, orientations, border);
        for (int i = 0; i < exitCoords.Length; i++) {
            // Exit Orientiation = Value - (Direction.Count - 1)
            grid[exitCoords[i][0]][exitCoords[i][1]] = (int)DIRECTION.count + (int)orientations[i] + 1;
        }
        return grid;
    }

    public static int[][] RotateExitID(int[][] grid) {
        for (int i = 0; i < grid.Length; i++) {
            for (int j = 0; j < grid[0].Length; j++) {
                if (grid[i][j] > (int)DIRECTION.count) {
                    // Exit Orientiation = Value - (Direction.Count - 1)
                    int orientation = grid[i][j] - (int)DIRECTION.count - 1;
                    orientation = (orientation + 1) % (int)ORIENTATION.count;
                    grid[i][j] = orientation + (int)DIRECTION.count + 1;
                }
            }
        }
        return grid;
    }

    public static Exitbox[] AddExitboxes(Exitbox nullExit, Transform gridTransform, int[][] grid) {

        List<int[]> exitCoords = new List<int[]>();
        List<int> orientations = new List<int>();
        for (int i = 0; i < grid.Length; i++) {
            for (int j = 0; j < grid[0].Length; j++) {
                if (grid[i][j] > (int)DIRECTION.count) {
                    exitCoords.Add(new int[] { i, j });
                    // Exit Orientiation = Value - (Direction.Count - 1)
                    orientations.Add(grid[i][j] - (int)DIRECTION.count - 1);
                }
            }
        }

        Exitbox[] roomExits = new Exitbox[exitCoords.Count];
        for (int i = 0; i < exitCoords.Count; i++) {
            Vector3 position = Geometry.GridToPosition(exitCoords[i], gridTransform);
            Exitbox exit = Instantiate(nullExit.gameObject, position, Quaternion.identity).GetComponent<Exitbox>();
            exit.gameObject.SetActive(true);
            Vector3 idVec = Compass.OrientationVectors[(ORIENTATION)orientations[i]];
            exit.id = new int[] { -(int)idVec.y, (int)idVec.x };

            exit.transform.localRotation = Compass.OrientationAngles[(ORIENTATION)orientations[i]];

            roomExits[i] = exit;
        }
        return roomExits;
    }

    static int[][] ExitCoordinates(int[][] grid, List<ORIENTATION> orientations, int border) {
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
    // Load already instantiated objects
    public static void LoadControllers(Controller[] controllers, bool activate) {
        for (int i = 0; i < controllers.Length; i++) {
            if (controllers[i] != null && !controllers[i].state.isDead) {
                controllers[i].gameObject.SetActive(activate);
            }
        }
    }

    // Load a set of new objects on the grid.
    public static Controller[] LoadNewControllers(Transform gridTransform, int[][] grid, Controller[] orderedControllers) {
        // Find out where challenges are and place them.
        List<Controller> controllers = new List<Controller>();
        for (int i = 0; i < grid.Length; i++) {
            for (int j = 0; j < grid[0].Length; j++) {
                // Instantiate the appropriate controller by its index.
                int index = grid[i][j];
                // Check that its a valid index.
                if (index < orderedControllers.Length && orderedControllers[index] != null) {
                    controllers.Add(InstantiateController(new int[] { i, j }, gridTransform, orderedControllers[index]));
                }
            }
        }
        return controllers.ToArray();
    }

    // Load a new object.
    public static void LoadNewController(Controller controllerPrefab, Vector3 position) {
        // Instantiate the new controller.
        Controller controller = Instantiate(controllerPrefab, position, Quaternion.identity, GameObject.FindWithTag(GameRules.roomTag).transform).GetComponent<Controller>();

        // Update the dungeon information.
        Dungeon dungeon = GameObject.FindWithTag(GameRules.dungeonTag)?.GetComponent<Dungeon>();
        if (dungeon != null) {
            string str_id = Dungeon.GetIDString(dungeon.id);
            Controller[] currControllers = dungeon.controllerDirectory[str_id];
            Controller[] newControllers = new Controller[currControllers.Length + 1];
            for (int i = 0; i < currControllers.Length; i++) {
                newControllers[i] = currControllers[i];
            }
            newControllers[currControllers.Length] = controller;
            dungeon.controllerDirectory[str_id] = newControllers;
        }
    }

    static Controller InstantiateController(int[] coord, Transform gridTransform, Controller controllerPrefab) {
        Vector3 position = (Vector3)Geometry.GridToPosition(coord, gridTransform);
        Controller controller = Instantiate(controllerPrefab, position, Quaternion.identity, gridTransform).GetComponent<Controller>();
        controller.gameObject.SetActive(true);
        return controller;
    }

}
