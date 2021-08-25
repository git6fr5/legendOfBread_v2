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

    public Sprite[] shapeSprites;
    public Sprite[] challengeSprites;
    public Sprite[] entranceSprites;

    public int[] prevCoord;
    public NodeSelector nullNode;
    public List<NodeSelector> nodes;

    public ValueSelector nullValue;
    List<ValueSelector> valueSelections;
    SpriteTile[] shapeTiles;
    SpriteTile[] challengeTiles;
    SpriteTile[] entranceTiles;
    int value;

    // Runs once on compilation.
    void Awake() {
        transform.position = map.transform.position;
    }

    void Start() {
        shapeTiles = CreateTiles(shapeTiles, shapeSprites);
        challengeTiles = CreateTiles(challengeTiles, challengeSprites);
        entranceTiles = CreateTiles(entranceTiles, entranceSprites);

        map.entranceGrid[map.entrance[0]][map.entrance[1]] = (int)ENTRANCE.ENTRANCE;
        map.entranceGrid[map.exit[0]][map.exit[1]] = (int)ENTRANCE.EXIT;

        SelectChannel(CHANNEL.SHAPE);
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
        HighlightValueSelector();
    }

    /* --- IO Stream --- */
    // Opens a CSV file as a room.
    public void Open(string filename) {
        map.Open(filename);
        ActivateNodes();
        PrintEdit();
    }

    // Saves the room to a CSV.
    public void Save(string filename) {
        NodeGrid();
        List<int[][]> channels = new List<int[][]>() {
            map.shapeGrid,
            map.challengeGrid,
            map.entranceGrid,
            map.nodeGrid
        };
        IO.SaveCSV(channels, Map.path, filename);
    }

    /* --- Selecting --- */
    // Sets the shape of the room.
    public void SelectChannel(CHANNEL selectedChannel) {
        channel = selectedChannel; // Until we set the buttons up.
        CreateChannelValueSelectors(channel);
        SetValue(0);
    }

    /* --- Editing --- */
    // Edit the shape of the rooms in the map.
    void EditShapes() {
        if (Input.GetMouseButtonDown(0)) {
            map.shapeGrid = Geometry.EditPoint(map.transform, map.shapeGrid, value);
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
            map.challengeGrid = Geometry.EditPoint(map.transform, map.challengeGrid, value);
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
                        Vector3 offset = orientation / 2f + rotation * ((((float)l) / 2f) - 0.5f) / 2f;
                        NodeSelector node = Instantiate(nullNode.gameObject, new Vector3(x_mid, y_mid, 0) + offset, Quaternion.identity, transform).GetComponent<NodeSelector>();
                        node.gameObject.SetActive(true);
                        node.id0 = new int[] { map.size -(i + (int)orientation.y), j };
                        node.id1 = new int[] { map.size - i, j + (int)orientation.x };
                        node.offset = l;
                        nodes.Add(node);
                    }
                }
            }
        }
    }

    int[][] NodeGrid() {
        List<NodeSelector> activeNodes = new List<NodeSelector>();
        for (int i = 0; i < nodes.Count; i++) {
            if (nodes[i].isActive) {
                activeNodes.Add(nodes[i]);
            }
        }

        map.nodeGrid = new int[activeNodes.Count][];
        for (int i = 0; i < activeNodes.Count; i++) {
            int[] nodeArray = new int[] { 
                activeNodes[i].id0[0], activeNodes[i].id0[1], 
                activeNodes[i].id1[0], activeNodes[i].id1[1], 
                activeNodes[i].offset, (int)activeNodes[i].access 
            };
            map.nodeGrid[i] = nodeArray;
        }

        print(map.nodeGrid.Length);
        return map.nodeGrid;
    }

    void ActivateNodes() {
        print(map.nodeGrid.Length);
        for (int i = 0; i < nodes.Count; i++) {
            for (int j = 0; j < map.nodeGrid.Length; j++) {
                NodeSelector node = nodes[i]; int[] nodeGrid = map.nodeGrid[j];
                if (node.id0[0] == nodeGrid[0] && node.id0[1] == nodeGrid[1] && node.id1[0] == nodeGrid[2] && node.id1[1] == nodeGrid[3] && node.offset == nodeGrid[4]) {
                    print("found active node");
                    node.SetActive();
                    node.access = (NodeSelector.ACCESS)nodeGrid[5];
                }
            }
        }
    }

    /* --- Value Painting --- */
    void CreateChannelValueSelectors(CHANNEL channel) {
        switch (channel) {
            case CHANNEL.SHAPE:
                CreateValueSelectors(shapeSprites);
                break;
            case CHANNEL.CHALLENGE:
                CreateValueSelectors(challengeSprites);
                break;
            case CHANNEL.ENTRANCE:
                CreateValueSelectors(entranceSprites);
                break;
            default:
                ResetValueSelectors();
                break;
        }
    }

    void CreateValueSelectors(Sprite[] sprites) {
        ResetValueSelectors();
        // Create the new selections.
        for (int i = 0; i < sprites.Length; i++) {
            ValueSelector newValueSelector = NewValue(sprites[i], i);
            valueSelections.Add(newValueSelector);
        }
    }

    SpriteTile[] CreateTiles(SpriteTile[] valueTiles, Sprite[] sprites) {
        if (valueTiles != null) {
            for (int i = valueTiles.Length - 1; i >= 0; i--) {
                Destroy(valueTiles[i]);
            }
        }
        print(sprites.Length);
        valueTiles = new SpriteTile[sprites.Length];
        for (int i = 0; i < sprites.Length; i++) {
            SpriteTile newTile = ScriptableObject.CreateInstance<SpriteTile>();
            newTile.newSprite = sprites[i];
            valueTiles[i] = newTile;
        }
        return valueTiles;
    }

    // Create a new brush selectable.
    ValueSelector NewValue(Sprite sprite, int index) {
        ValueSelector valueSelector = Instantiate(nullValue.gameObject,
            Vector3.zero, Quaternion.identity, nullValue.transform.parent).GetComponent<ValueSelector>();
        // Set the selection parameters.
        valueSelector.gameObject.SetActive(true);
        valueSelector.transform.localPosition = new Vector3(index % 3, -Mathf.Floor(index / 3), 0);
        valueSelector.GetComponent<SpriteRenderer>().sprite = sprite;
        valueSelector.index = index;
        return valueSelector;
    }

    void ResetValueSelectors() {
        // Reset the value selections.
        if (valueSelections != null) {
            for (int i = valueSelections.Count - 1; i >= 0; i--) {
                Destroy(valueSelections[i].gameObject);
            }
        }
        valueSelections = new List<ValueSelector>();
    }

    public void SetValue(int index) {
        value = index;
    }

    void HighlightValueSelector() {
        for (int i = 0; i < valueSelections.Count; i++) {
            if (valueSelections[i].index == value) {
                valueSelections[i].spriteRenderer.material.SetFloat("_Opacity", 1f);
            }
            else {
                valueSelections[i].spriteRenderer.material.SetFloat("_Opacity", 0.5f);
            }
        }
    }

}
