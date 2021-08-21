using System.Collections;
using System.Collections.Generic;
using UnityEngine;
using UnityEngine.Tilemaps;

using SHAPE = Geometry.SHAPE;
using DIRECTION = Compass.DIRECTION;
using CHALLENGE = Room.CHALLENGE;

public class MapEditor : MonoBehaviour {

    public enum CHANNEL {
        SHAPE,
        EXIT,
        CHALLENGE,
        count
    }

    public CHANNEL channel;

    public Map map;
    public Tilemap shapeMap;
    public Tilemap exitMap;
    public Tilemap challengeMap;

    public TileBase[] shapeTiles;
    public TileBase[] exitTiles;
    public TileBase[] challengeTiles;

    public int[] prevCoord;

    // Runs once on compilation.
    void Awake() {
        transform.position = map.transform.position;
    }

    void Start() {
        PrintEdit();
    }

    // Runs once every frame.
    void Update() {
        switch (channel) {
            case (CHANNEL.SHAPE):
                EditShapes();
                break;
            case (CHANNEL.EXIT):
                EditExits();
                break;
            case (CHANNEL.CHALLENGE):
                EditChallenges();
                break;
        }
    }

    /* --- IO Stream --- */
    // Opens a CSV file as a room.
    public void Open(string filename) {
        map.Open(filename);
        PrintEdit();
    }

    // Saves the room to a CSV.
    public void Save(string filename) {
        List<int[][]> channels = new List<int[][]>() {
            map.shapeGrid,
            map.exitAndRotationsGrid,
            map.challengeGrid,
        };
        IO.SaveCSV(channels, Map.path, filename);
    }

    // Print the edits to this map.
    void PrintEdit(bool printAll = false) {
        // For now, print only to the mob grid.
        Geometry.PrintGridToMap(map.shapeGrid, shapeMap, shapeTiles);
        Geometry.PrintGridToMap(map.challengeGrid, challengeMap, challengeTiles);
        Geometry.PrintGridToMap(map.exitAndRotationsGrid, exitMap, exitTiles);

    }

    /* --- Editing --- */
    // Edit the shape of the rooms in the map.
    void EditShapes() {
        if (Input.GetMouseButtonDown(0)) {
            map.shapeGrid = Geometry.IncrementPoint(map.transform, map.shapeGrid, (int)SHAPE.count);
            PrintEdit();
        }
        if (Input.GetMouseButtonDown(1)) {
            map.shapeGrid = Geometry.EditPoint(map.transform, map.shapeGrid, (int)SHAPE.EMPTY);
            PrintEdit();
        }
    }

    // Edit how the rooms connect with each other.
    void EditExits() {
        if (Input.GetMouseButtonDown(0)) {
            Vector2 mousePos = Camera.main.ScreenToWorldPoint(Input.mousePosition);
            prevCoord = Geometry.PointToGrid(mousePos, map.transform);
        }
        if (Input.GetMouseButtonUp(0)) {
            Vector2 mousePos = Camera.main.ScreenToWorldPoint(Input.mousePosition);
            int[] coord = Geometry.PointToGrid(mousePos, transform);
            if (Geometry.IsValid(coord, map.shapeGrid) && Geometry.IsValid(prevCoord, map.shapeGrid)) {
                map.exitAndRotationsGrid[coord[0]][coord[1]] = Compass.GetNewPath(map.exitAndRotationsGrid[coord[0]][coord[1]], coord, prevCoord);
                map.exitAndRotationsGrid[prevCoord[0]][prevCoord[1]] = Compass.GetNewPath(map.exitAndRotationsGrid[prevCoord[0]][prevCoord[1]], prevCoord, coord);
                PrintEdit();
            }
        }
    }

    // Edit the different types of challenges in the map.
    void EditChallenges() {
        if (Input.GetMouseButtonDown(0)) {
            map.challengeGrid = Geometry.IncrementPoint(map.transform, map.challengeGrid, (int)CHALLENGE.count);
            PrintEdit();
        }
        if (Input.GetMouseButtonDown(1)) {
            map.challengeGrid = Geometry.EditPoint(map.transform, map.challengeGrid, (int)CHALLENGE.EMPTY);
            PrintEdit();
        }
    }

    /* --- Additional Construction --- */
    // Sets the shape of the room.
    public void SelectChannel(CHANNEL selectedChannel) {
        print("selecting channel");
        channel = (CHANNEL)(((int)channel + 1) % (int)CHANNEL.count);
        // channel = selectedChannel; // Until we set the buttons up.
    }

}
