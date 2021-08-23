using System.Collections;
using System.Collections.Generic;
using System.IO;
using UnityEngine;
using UnityEngine.UI;
using UnityEngine.Tilemaps;

using DIRECTION = Compass.DIRECTION;
using SHAPE = Geometry.SHAPE;

public class Map : MonoBehaviour {
   
    /* --- Enums --- */
    // The data that the map needs for each room.
    public enum Channel {
        SHAPE,
        EXIT,
        CHALLENGE,
        ROTATION,
        channelCount
    };

    /* --- COMPONENTS --- */
    public Minimap minimap;

    /* --- VARIABLES --- */
    // Files.
    public static string path = "DataFiles/Maps/";
    // Dimensions.
    public int[][] shapeGrid;
    public int[][] nodeGrid;
    public int[][] challengeGrid;
    [Range(1, 8)] public int size = 7;

    /* --- Unity --- */
    // Runs once on compilation
    void Awake() {
        Reset();
    }


    public void Open(string filename) {
        List<int[][]> channels = IO.OpenCSV(path, filename);
        shapeGrid = channels[0];
        nodeGrid = channels[1];
        challengeGrid = channels[2];
    }

    /* --- Methods --- */
    // Resets the room state
    public void Reset() {
        shapeGrid = Geometry.Grid(SHAPE.EMPTY, size, size);
        nodeGrid = Geometry.Grid(SHAPE.EMPTY, size, size);
        challengeGrid = Geometry.Grid(SHAPE.EMPTY, size, size);
    }

}
