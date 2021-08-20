using System.Collections;
using System.Collections.Generic;
using UnityEngine;
using UnityEngine.Tilemaps;

using ORIENTATION = Compass.ORIENTATION;
using EXIT = Compass.EXIT;
using SHAPE = Geometry.SHAPE;
using DIRECTION = Compass.DIRECTION;

[RequireComponent(typeof(Grid))]
public class Room : MonoBehaviour {

    /* --- Enums --- */
    public enum CHALLENGE {
        MOBS,
        TRAPS,
        count
    }


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
    public SHAPE shape;
    public EXIT exits;
    public CHALLENGE challenge;
    public int[][] mobGrid;
    public int[][] trapGrid;

    /* --- Variables --- */
    // Dimensions.
    public int size = 11;
    public int border = 1;
    // File Management.
    public static string path = "DataFiles/Rooms/";
    public static string identifierFilename = "identifierFiles";

    /* --- Unity --- */
    // Runs once on compilation
    void Awake() {
        Reset();
        Construct();
    }

    /* --- Files --- */
    // Reads and constructs a room from a file.
    public void Open(string filename) {
        List<int[][]> channels = IO.OpenCSV(path, filename);
        int[] identifiers = IO.FindInListFile(filename);
        shape = (SHAPE)identifiers[0];
        exits = (EXIT)identifiers[1];
        challenge = (CHALLENGE)identifiers[2]; 
        mobGrid = channels[0];
        trapGrid = channels[1];
        Construct();
    }

    /* --- Methods --- */
    // Resets the room state
    public void Reset() {
        floorGrid = Geometry.Grid(SHAPE.EMPTY, size, size);
        mobGrid = Geometry.Grid(SHAPE.EMPTY, size, size);
        trapGrid = Geometry.Grid(SHAPE.EMPTY, size, size);
    }

    // Constructs the room setting.
    public void Construct() {
        borderGrid = Geometry.Grid(shape, size, size, border, border);
        borderGrid = Janitor.AddExits(borderGrid, exits, border);
        PrintRoom();
    }

    // Prints the grids to the tilemaps.
    public void PrintRoom() {
        borderGrid = Janitor.CleanBorder(borderGrid, size, size, border, border);
        Geometry.PrintGridToMap(floorGrid, floorMap, environment.floor);
        Geometry.PrintGridToMap(borderGrid, borderMap, environment.border);
    }

}
