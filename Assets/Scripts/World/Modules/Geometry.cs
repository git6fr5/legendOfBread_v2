using System.Collections;
using System.Collections.Generic;
using UnityEngine;
using UnityEngine.Tilemaps;

using DIRECTION = Compass.Direction;

public class Geometry : MonoBehaviour {

    /* --- Enums --- */
    // The various shapes.
    public enum Shape {
        Empty,
        Square,
        Ellipse,
        VerticalStripes,
        HorizontalStripes,
        UpHall,
        DownHall,
        count
    }

    /* --- Grid Construction --- */
    // Constructs the given shape through a border.
    public static int[][] Grid(
        Shape shape, int vertical, int horizontal, int vertBorder = 0,
        int horBorder = 0, int backgroundTile = (int)DIRECTION.Empty,
        int fillTile = (int)DIRECTION.Center) {

        switch (shape) {
            case Shape.Empty:
                return Empty(backgroundTile, fillTile, vertical, horizontal);
            case Shape.Square:
                return Square(backgroundTile, fillTile, vertical, horizontal, vertBorder, horBorder);
            case Shape.Ellipse:
                return Ellipse(backgroundTile, fillTile, vertical, horizontal, vertBorder, horBorder);
            case Shape.VerticalStripes:
                return VerticalStrips(backgroundTile, fillTile, vertical, horizontal, vertBorder, horBorder);
            case Shape.HorizontalStripes:
                return HorizontalStrips(backgroundTile, fillTile, vertical, horizontal, vertBorder, horBorder);
            case Shape.UpHall:
                return HorizontalStrips(backgroundTile, fillTile, vertical, horizontal, vertBorder, horBorder, 1);
            case Shape.DownHall:
                return HorizontalStrips(backgroundTile, fillTile, vertical, horizontal, vertBorder, horBorder, -1);
            default:
                return Empty(backgroundTile, fillTile, vertical, horizontal);
        }
    }


    public static int[][] RandomizeGrid(int vertical, int horizontal, int rows, int seed) {

        int floorSeed = int.Parse(seed.ToString().Substring(3, 2));
        int row = floorSeed % 4;

        int[][] grid = new int[vertical][];
        for (int i = 0; i < vertical; i++) {
            grid[i] = new int[horizontal];
            for (int j = 0; j < horizontal; j++) {
                int index = GameRules.PrimeRandomizerID(floorSeed, new int[] { i, j }) % rows;
                index = rows * row + index;
                grid[i][j] = index;
            }
        }
        return grid;
    }

    // Creates an empty grid.
    public static int[][] Empty(int backgroundTile, int fillTile, int vertical, int horizontal) {
        // Initialize the grid.
        int[][] empty = new int[vertical][];
        for (int i = 0; i < vertical; i++) {
            empty[i] = new int[horizontal];
            for (int j = 0; j < horizontal; j++) {
                empty[i][j] = backgroundTile;
            }
        }
        return empty;
    }

    // Creates a square border.
    public static int[][] Square(int backgroundTile, int fillTile, int vertical, int horizontal, int vertBorder, int horBorder) {
        // Initialize the grid.
        int[][] square = new int[vertical][];
        for (int i = 0; i < vertical; i++) {
            square[i] = new int[horizontal];
        }
        // Fill in the borders.
        for (int i = 0; i < vertical; i++) {
            for (int j = 0; j < vertBorder; j++) {
                square[i][j] = fillTile;
                square[i][vertical - (j + 1)] = fillTile;
            }
        }
        for (int i = 0; i < horizontal; i++) {
            for (int j = 0; j < horBorder; j++) {
                square[j][i] = fillTile;
                square[horizontal - (j + 1)][i] = fillTile;
            }
        }
        return square;
    }

    // Creates an elliptical border.
    public static int[][] Ellipse(int backgroundTile, int fillTile, int vertical, int horizontal, int vertBorder, int horBorder) {
        // Initialize the grid.
        int[][] ellipse = new int[vertical][];
        for (int i = 0; i < ellipse.Length; i++) {
            ellipse[i] = new int[horizontal];
            for (int j = 0; j < ellipse[i].Length; j++) {
                ellipse[i][j] = backgroundTile;
            }
        }
        // The major and minor axis radii.
        float a = (float)(horizontal-horBorder) / 2;
        float b = (float)(vertical-vertBorder) / 2;
        // Draw the ellipse.
        for (int i = 0; i < vertical; i++) {
            for (int j = 0; j < horizontal; j++) {
                float x = (float)j - a;
                float y = (float)i - b;
                float ellipticalBoundary = (x * x / (a * a)) + (y * y / (b * b));
                if (Mathf.Abs(ellipticalBoundary) >= 1) { 
                    ellipse[i][j] = fillTile; 
                }
            }
        }
        return ellipse;
    }

    public static int[][] VerticalStrips(int backgroundTile, int fillTile, int vertical, int horizontal, int vertBorder, int horBorder, int offset = 0) {
        // Initialize the grid.
        int[][] strips = Square(backgroundTile, fillTile, vertical, horizontal, vertBorder, horBorder);

        // Draw a line through the middle.
        int width = (horizontal - horBorder * 2);
        int[] lines;
        if (width % 2 == 1) {
            lines = new int[] { horBorder + (int)Mathf.Floor(horizontal / 2) - 1 };
        }
        else {
            lines = new int[] { horBorder + (int)Mathf.Floor(horizontal / 2) - 1, horBorder + (int)Mathf.Floor(horizontal / 2) };
        }
        foreach( int line in lines) {
            for (int i = 0; i < vertical; i++) {
                strips[i][line - offset] = fillTile;
            }
        }

        return strips;
    }

    public static int[][] HorizontalStrips(int backgroundTile, int fillTile, int vertical, int horizontal, int vertBorder, int horBorder, int offset = 0) {
        // Initialize the grid.
        int[][] strips = Square(backgroundTile, fillTile, vertical, horizontal, vertBorder, horBorder);

        // Draw a line through the middle.
        int width = (vertical - vertBorder * 2);
        int[] lines;
        if (width % 2 == 1) {
            lines = new int[] { vertBorder + (int)Mathf.Floor(vertical / 2) - 1 };
        }
        else {
            lines = new int[] { vertBorder + (int)Mathf.Floor(vertical / 2) - 1, vertBorder + (int)Mathf.Floor(vertical / 2) };
        }
        foreach (int line in lines) {
            for (int i = 0; i < horizontal; i++) {
                strips[line - offset][i] = fillTile;
            }
        }

        return strips;
    }

    /* --- Points --- */
    public static int[][] IncrementPoint(Transform gridTransform, int[][] grid, int maxValue) {
        Vector2 mousePos = Camera.main.ScreenToWorldPoint(Input.mousePosition);
        int[] coord = Geometry.PointToGrid(mousePos, gridTransform);
        if (Geometry.IsValid(coord, grid)) {
            grid[coord[0]][coord[1]] = (grid[coord[0]][coord[1]] + 1) % maxValue;
        }
        return grid;
    }

    public static int[][] EditPoint(Transform gridTransform, int[][] grid, int value) {
        Vector2 mousePos = Camera.main.ScreenToWorldPoint(Input.mousePosition);
        int[] coord = Geometry.PointToGrid(mousePos, gridTransform);
        if (Geometry.IsValid(coord, grid)) {
            grid[coord[0]][coord[1]] = value;
        }
        return grid;
    }

    public static int[][] EditInteriorPoint(Transform gridTransform, int[][] grid, int[][] borderGrid, int value) {
        Vector2 mousePos = Camera.main.ScreenToWorldPoint(Input.mousePosition);
        int[] coord = Geometry.PointToGrid(mousePos, gridTransform);
        if (Geometry.IsValid(coord, grid) && borderGrid[coord[0]][coord[1]] == (int)DIRECTION.Empty) {
            grid[coord[0]][coord[1]] = value;
        }
        return grid;
    }

    /* --- Tilemap Construction --- */
    // Set the tiles in a tilemap according to the grid.
    public static void PrintGridToMap(int[][] grid, Tilemap tilemap, TileBase[] tiles, int border = 0) {
        for (int i = border; i < grid.Length - border; i++) {
            for (int j = border; j < grid[0].Length - border; j++) {
                PrintTileToMap(grid, tilemap, tiles, i, j);
            }
        }
    }

    // Set the tile based on the grid value.
    static void PrintTileToMap(int[][] grid, Tilemap tilemap, TileBase[] tiles, int i, int j) {
        if (grid[i][j] < tiles.Length) {
            Vector3Int tilePosition = GridToTileMap(i, j);
            TileBase tile = tiles[grid[i][j]];
            tilemap.SetTile(tilePosition, tile);
        }
    }

    // Converts a grid coordinate to tile map position.
    public static Vector3Int GridToTileMap(int i, int j) {
        return new Vector3Int(j, -(i + 1), 0);
    }

    /* --- Conversions --- */
    // TODO: Fix these conversions.
    // Transforms a given point in world space to grid coordinates 
    public static int[] PointToGrid(Vector2 point, Transform gridTransform) {
        int i = (int)(-(Mathf.Floor(point.y + 0.49f) - gridTransform.position.y ));
        int j = (int)(Mathf.Floor(point.x + 0.49f) - gridTransform.position.x);
        //print(i + ", " + j);
        return new int[] { i, j };
    }

    // Transforms a given coordinate to world space
    public static Vector3 GridToPosition(int[] coordinate, Transform gridTransform) {
        return new Vector3(gridTransform.position.x + coordinate[1] + 0.5f, gridTransform.position.y - (coordinate[0] + 0.5f), 0);
    }

    /* --- Searching --- */
    public static List<int[]> AdjacentEmptyTiles(int[] coordinate, int[][] grid, bool includeCenter) {
        List<int[]> adjacentEmptyTiles = new List<int[]>();
        if (includeCenter) {
            adjacentEmptyTiles.Add(coordinate);
        }
        for (int i = -1; i <= 1; i++) {
            for (int j = -1; j <= 1; j++) {
                int[] newCoordinate = new int[] { coordinate[0] + i, coordinate[1] + j };
                if (IsValid(newCoordinate, grid) && (i != 0 || j != 0)) {
                    if (grid[newCoordinate[0]][newCoordinate[1]] == (int)DIRECTION.Empty) {
                        adjacentEmptyTiles.Add(newCoordinate);
                    }
                }
            }
        }
        return adjacentEmptyTiles;
    }

    public static bool IsValid(int[] coordinate, int[][] grid) {
        return (coordinate[0] < grid.Length && coordinate[0] >= 0 && coordinate[1] < grid[0].Length && coordinate[1] >= 0);
    }

    public static bool WithinBorder(int[] coordinate, int[][] grid, int borderSize) {
        return (coordinate[0] < grid.Length - (borderSize + 1) && coordinate[0] >= (borderSize + 1) && coordinate[1] < grid[0].Length - (borderSize + 1) && coordinate[1] >= (borderSize + 1));
    }

    public static int[][] RotateClockwise(int[][] grid) {
        int[][] rotatedGrid = new int[grid[0].Length][];
        for (int i = 0; i < grid.Length; i++) {
            rotatedGrid[i] = new int[grid.Length];
            for (int j = 0; j < grid[0].Length; j++) {
                rotatedGrid[i][j] = grid[j][grid.Length - i - 1];
            }
        }
        return rotatedGrid;
    }


}
