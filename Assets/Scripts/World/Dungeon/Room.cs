using System.Collections;
using System.Collections.Generic;
using UnityEngine;
using UnityEngine.Tilemaps;

using ORIENTATION = Compass.ORIENTATION;
using SHAPE = Geometry.SHAPE;
using TILE = Janitor.TILE;

[RequireComponent(typeof(Grid))]
public class Room : MonoBehaviour {

    /* --- Components --- */
    // Maps
    public Tilemap floorMap;
    public Tilemap borderMap;
    // Grids
    public int[][] floorGrid;
    public int[][] borderGrid;
    // Environment
    public Environment environment;

    // Constructors
    public TILE exits;
    public SHAPE shape;
    public int[][] mobGrid;

    /* --- Variables --- */
    int size = 11;
    int border = 1;

    /* --- Unity --- */
    // Runs once on compilation
    void Awake() {
        Reset();
        Construct();
    }

    /* --- Methods --- */
    // Resets the room state
    public void Reset() {
        floorGrid = Geometry.Grid(SHAPE.EMPTY, size, size);
        mobGrid = Geometry.Grid(SHAPE.EMPTY, size, size);
    }

    // Constructs the room setting.
    public void Construct() {
        borderGrid = Geometry.Grid(shape, size, size, border, border);
        borderGrid = Janitor.CleanBorder(borderGrid, size, size, border, border);
        borderGrid = Janitor.AddExits(borderGrid, exits, border);
        // roomObjectDictionary[id] = ObjectLoader.LoadObjects(mobs);
        PrintRoom();
    }

    // Prints the grids to the tilemaps.
    void PrintRoom() {
        Geometry.PrintGridToMap(floorGrid, floorMap, environment.floor);
        Geometry.PrintGridToMap(borderGrid, borderMap, environment.border);
    }

}
