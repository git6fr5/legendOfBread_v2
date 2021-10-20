using System.Collections;
using System.Collections.Generic;
using UnityEngine;
using UnityEngine.Tilemaps;

using ORIENTATION = Compass.Orientation;
using DIRECTION = Compass.Direction;
using Vitality = State.Vitality;

public class Janitor : MonoBehaviour {

    /* --- Border Cleaning --- */
    // The organization format.
    static DIRECTION[] inputOrder = new DIRECTION[] {
        DIRECTION.LeftUp, DIRECTION.Up, DIRECTION.UpRight,
        DIRECTION.Left, DIRECTION.Center, DIRECTION.Right,
        DIRECTION.DownLeft, DIRECTION.Down, DIRECTION.DownRight,
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
                if (grid[i][j] != (int)DIRECTION.Center && grid[i][j] < (int)DIRECTION.count) {
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
        if (Geometry.IsValid(new int[] { i, j }, grid) && grid[i][j] == (int)DIRECTION.Center) {
            return true;
        }
        return false;
    }

    /* --- Exit --- */
    public static int[][] AddExits(DIRECTION direction, int[][] grid, int border) {
        List<ORIENTATION> orientations = Compass.DirectionToOrientations(direction);
        for (int i = 0; i < orientations.Count; i++) {
            // Exit Orientiation = Value - (Direction.Count - 1)
            int[] exitCoord = OrientationToCoordinate(orientations[i], grid.Length, border);
            grid[exitCoord[0]][exitCoord[1]] = (int)DIRECTION.count + (int)orientations[i] + 1;
        }
        return grid;
    }

    public static int[][] NewAddExits(int[][] grid, List<ORIENTATION> orientations, int border) {
        for (int i = 0; i < orientations.Count; i++) {
            // Exit Orientiation = Value - (Direction.Count - 1)
            int[] exitCoord = OrientationToCoordinate(orientations[i], grid.Length, border);
            grid[exitCoord[0]][exitCoord[1]] = (int)DIRECTION.count + (int)orientations[i] + 1;
        }
        return grid;
    }

    public static Exitbox[] AddExitboxes(Exitbox nullExit, Transform gridTransform, int[][] grid) {
        List<Exitbox> exitboxes = new List<Exitbox>();
        for (int i = 0; i < grid.Length; i++) {
            for (int j = 0; j < grid[0].Length; j++) {
                if (grid[i][j] > (int)DIRECTION.count) {
                    Exitbox exit = AddExitbox(nullExit, grid, gridTransform, new int[] { i, j });
                    exitboxes.Add(exit);
                }
            }
        }
        return exitboxes.ToArray();
    }

    static Exitbox AddExitbox(Exitbox nullExit, int[][] grid, Transform gridTransform, int[] coord) {
        Vector3 position = Geometry.GridToPosition(coord, gridTransform);
        Exitbox exit = Instantiate(nullExit.gameObject, position, Quaternion.identity).GetComponent<Exitbox>();
        exit.gameObject.SetActive(true);

        ORIENTATION exitOrientation = (ORIENTATION) (grid[coord[0]][coord[1]] - (int)DIRECTION.count - 1);
        Vector3 vec_id = Compass.OrientationVectors[exitOrientation];
        exit.id = new int[] { -(int)vec_id.y, (int)vec_id.x };
        exit.transform.localRotation = Compass.OrientationAngles[exitOrientation];
        return exit;
    }

    static int[] OrientationToCoordinate(ORIENTATION orientation, int length, int border) {
        Vector2 vector = Compass.OrientationVectors[orientation];
        vector.x = (vector.x + 1f) / 2f;
        vector.y = (-vector.y + 1f) / 2f;
        vector = vector * (length - 2*border - 1) + new Vector2(border, border);
        return new int[2] { (int)vector.y, (int)vector.x };
    }

    /* --- Object Loading --- */
    // Load already instantiated objects
    public static void LoadControllers(Controller[] controllers, bool activate) {
        for (int i = 0; i < controllers.Length; i++) {
            if (controllers[i] != null && controllers[i].state.vitality != Vitality.Dead) {
                controllers[i].gameObject.SetActive(activate);
                controllers[i].transform.position = controllers[i].origin;
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
    public static Controller LoadNewController(Controller controllerPrefab, Vector3 position) {
        // Instantiate the new controller.
        Controller controller = Instantiate(controllerPrefab, position, Quaternion.identity, GameObject.FindWithTag(GameRules.roomTag)?.transform).GetComponent<Controller>();

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

        return controller;
    }

    static Controller InstantiateController(int[] coord, Transform gridTransform, Controller controllerPrefab) {
        Vector3 position = (Vector3)Geometry.GridToPosition(coord, gridTransform);
        Controller controller = Instantiate(controllerPrefab, position, Quaternion.identity, gridTransform).GetComponent<Controller>();
        controller.gameObject.SetActive(true);
        return controller;
    }

}
