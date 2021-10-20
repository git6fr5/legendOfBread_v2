using System.Collections;
using System.Collections.Generic;
using System.IO;
using UnityEngine;
using UnityEngine.UI;
using UnityEngine.Tilemaps;

using DIRECTION = Compass.Direction;
using SHAPE = Geometry.Shape;

public class Map : MonoBehaviour {

    /* --- Enums --- */
    public enum ENTRANCE {
        EMPTY,
        ENTRANCE,
        EXIT,
        count
    }

    /* --- COMPONENTS --- */
    public Minimap minimap;

    /* --- VARIABLES --- */
    // Files.
    public static string path = "DataFiles/Maps/";
    // Dimensions.
    public int[][] shapeGrid;
    public int[][] nodeGrid;
    public int[][] challengeGrid;
    public int[][] entranceGrid;
    [Range(1, 8)] public int size = 7;
    // Entrances
    public int[] entrance = new int[] { 0, 0 };
    public int[] exit = new int[] { 6, 6 };

    /* --- Unity --- */
    // Runs once on compilation
    void Awake() {
        Reset();
    }


    public void Open(string filename) {
        List<int[][]> channels = IO.OpenCSV(path, filename);
        shapeGrid = channels[0];
        challengeGrid = channels[1];
        entranceGrid = channels[2];
        nodeGrid = channels[3];
        FindEntrances();
        // print(entrance[0]);
        // print(entrance[1]);
    }

    /* --- Methods --- */
    // Resets the room state
    public void Reset() {
        shapeGrid = Geometry.Grid(SHAPE.Empty, size, size);
        nodeGrid = new int[][] { };
        challengeGrid = Geometry.Grid(SHAPE.Empty, size, size);
        entranceGrid = Geometry.Grid(SHAPE.Empty, size, size);
    }

    public void FindEntrances() {
        for (int i = 0; i < entranceGrid.Length; i++) {
            for (int j = 0; j < entranceGrid[0].Length; j++) {
                if (entranceGrid[i][j] == (int)ENTRANCE.ENTRANCE) {
                    entrance = new int[] { i, j };
                }
                else if (entranceGrid[i][j] == (int)ENTRANCE.EXIT) {
                    exit = new int[] { i, j };
                }
            }
        }
    }

}
