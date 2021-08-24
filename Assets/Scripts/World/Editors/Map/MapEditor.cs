using System.Collections;
using System.Collections.Generic;
using UnityEngine;
using UnityEngine.Tilemaps;

using SHAPE = Geometry.SHAPE;
using DIRECTION = Compass.DIRECTION;
using ORIENTATION = Compass.ORIENTATION;
using CHALLENGE = Room.CHALLENGE;
using ENTRANCE = Map.ENTRANCE;

public class MapEditor : MonoBehaviour {

    public enum CHANNEL {
        SHAPE,
        NODE,
        CHALLENGE,
        ENTRANCE,
        count
    }

    public CHANNEL channel;

    public Map map;
    public Tilemap shapeMap;
    public Tilemap challengeMap;
    public Tilemap entranceMap;

    public TileBase[] shapeTiles;
    public TileBase[] challengeTiles;
    public TileBase[] entranceTiles;

    public int[] prevCoord;
    public NodeSelector nullNode;
    public List<NodeSelector> nodes;

    // Runs once on compilation.
    void Awake() {
        transform.position = map.transform.position;
    }

    void Start() {
        CreateNodes();
        PrintEdit();
    }

    // Runs once every frame.
    void Update() {
        switch (channel) {
            case (CHANNEL.SHAPE):
                EditShapes();
                break;
            case (CHANNEL.NODE):
                EditExits();
                break;
            case (CHANNEL.CHALLENGE):
                EditChallenges();
                break;
            case (CHANNEL.ENTRANCE):
                EditEntrance();
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
            map.nodeGrid,
            map.challengeGrid,
            map.entranceGrid
        };
        IO.SaveCSV(channels, Map.path, filename);
    }

    /* --- Selecting --- */
    // Sets the shape of the room.
    public void SelectChannel(CHANNEL selectedChannel) {
        channel = selectedChannel; // Until we set the buttons up.
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
        //
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

    // Adds or removes an entrance from the map
    void EditEntrance() {
        if (Input.GetMouseButtonDown(0)) {
            Vector2 mousePos = Camera.main.ScreenToWorldPoint(Input.mousePosition);
            int[] coord = Geometry.PointToGrid(mousePos, transform);
            if (Geometry.IsValid(coord, map.shapeGrid)) {
                print("hello");
                map.entranceGrid[map.entrance[0]][map.entrance[1]] = (int)ENTRANCE.EMPTY;
                map.entranceGrid[coord[0]][coord[1]] = (int)ENTRANCE.ENTRANCE;
                map.entrance = coord;
            }
            PrintEdit();
        }
        if (Input.GetMouseButtonDown(1)) {
            Vector2 mousePos = Camera.main.ScreenToWorldPoint(Input.mousePosition);
            int[] coord = Geometry.PointToGrid(mousePos, transform);
            if (Geometry.IsValid(coord, map.shapeGrid)) {
                map.entranceGrid[map.exit[0]][map.exit[1]] = (int)ENTRANCE.EMPTY;
                map.entranceGrid[coord[0]][coord[1]] = (int)ENTRANCE.EXIT;
                map.exit = coord;
            }
            PrintEdit();
        }
    }

    // Print the edits to this map.
    void PrintEdit(bool printAll = false) {
        // For now, print only to the mob grid.
        Geometry.PrintGridToMap(map.shapeGrid, shapeMap, shapeTiles);
        Geometry.PrintGridToMap(map.challengeGrid, challengeMap, challengeTiles);
        Geometry.PrintGridToMap(map.entranceGrid, entranceMap, entranceTiles);
    }

    /* --- Nodes --- */
    void CreateNodes() {
        nodes = new List<NodeSelector>();
        int nodesPerSide = 3;
        for (int i = 0; i < map.size; i++) {
            int sides = 2;
            if (i == map.size - 1) {
                sides = 1;
            }
            for (int j = 0; j < map.size; j++) {
                int min_side = 0;
                if (j == map.size - 1) {
                    min_side = 1;
                }
                float x_mid = j - (int)((float)map.size / 2) + 1;
                float y_mid = i - (int)((float)map.size / 2);
                for (int k = min_side; k < sides; k++) {
                    Vector3 orientation = (Vector3)Compass.OrientationVectors[(ORIENTATION)k];
                    Vector3 rotation = Vector3.right;
                    if (k % 2 == 0) {
                        rotation = Vector3.up;
                    }
                    else {
                        rotation = Vector3.right;
                    }
                    for (int l = 0; l < nodesPerSide; l++) {
                        print(orientation);
                        Vector3 offset = orientation / 2f + rotation * ((((float)l) / 2f) - 0.5f) / 2f;
                        print(offset);
                        NodeSelector node = Instantiate(nullNode.gameObject, new Vector3(x_mid, y_mid, 0) + offset, Quaternion.identity, transform).GetComponent<NodeSelector>();
                        node.gameObject.SetActive(true);
                        node.id0 = new int[] { map.size -(i + (int)orientation.y), j };
                        node.id1 = new int[] { map.size - i, j + (int)orientation.x };
                        nodes.Add(node);
                    }
                }
            }
        }
    }

}
