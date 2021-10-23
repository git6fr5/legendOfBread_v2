using System.Collections;
using System.Collections.Generic;
using UnityEngine;
using UnityEngine.Tilemaps;

using SHAPE = Geometry.Shape;
using DIRECTION = Compass.Direction;
using CHALLENGE = Room.CHALLENGE;

public class RoomEditor : MonoBehaviour {

    /* --- Components --- */
    public Room room;
    public Tilemap mobMap;
    public Tilemap trapMap;
    public ChallengeSelector challengeSelector;
    public ValueSelector nullValue;
    public ValueSelector nullShape;

    public Sprite[] shapeSprites;

    /* --- Variables --- */
    // Has to be sprite tiles to be able to access the sprite later on.
    public SpriteTile[] challengeTiles;
    List<ValueSelector> valueSelections;
    int value;

    /* --- Unity --- */
    // Runs once on compilation.
    void Awake() {
        transform.position = room.transform.position;
    }

    // Runs once before the first frame.
    void Start() {
        room.challenge = CHALLENGE.MOBS;
        GetChallengeTiles();
        CreateValueSelectors();
    }

    // Runs once every frame.
    void Update() {
        switch (room.challenge) {
            case (CHALLENGE.MOBS):
                EditMobs();
                break;
            case (CHALLENGE.TRAPS):
                EditTraps();
                break;
            default:
                break;
        }
        HighlightValueSelector();
    }

    /* --- IO Stream --- */
    // Opens a CSV file as a room.
    public void Open(string filename) {
        room.Open(filename);
        PrintEdit(room.mobGrid, mobMap);
        PrintEdit(room.trapGrid, trapMap);
    }

    // Saves the room to a CSV.
    public void Save(string filename) {
        int[] identifiers = new int[] { (int)room.shape, (int)room.challenge };
        IO.EditListFile(identifiers, Room.path, filename);
        List<int[][]> challenges = new List<int[][]>() {
            room.mobGrid,
            room.trapGrid
        };

        IO.SaveCSV(challenges, Room.path, filename);
    }

    /* --- Selections --- */
    // Sets the shape of the room.
    public void CreateShapeSelectors() {
        // Reset the value selections.
        if (valueSelections != null) {
            for (int i = valueSelections.Count - 1; i >= 0; i--) {
                Destroy(valueSelections[i].gameObject);
            }
        }
        valueSelections = new List<ValueSelector>();
        // Create the new selections.
        for (int i = 0; i < shapeSprites.Length; i++) {
            ValueSelector newValueSelector = NewShape(shapeSprites[i], i);
            valueSelections.Add(newValueSelector);
        }
    }

    // Select a new challenge.
    public void SelectChannel(CHALLENGE selectedChallenge) {
        room.challenge = (CHALLENGE)(((int)room.challenge + 1) % (int)CHALLENGE.count);
        // challenge = selectedChannel;
        GetChallengeTiles();
        // Create the selections for the new tiles.
        CreateValueSelectors();
        // Reset the value
        value = 0;
        // Reset the room
        // room.Reset();
        // PrintEdit(room.mobGrid, mobMap);
        // PrintEdit(room.trapGrid, trapMap);
    }

    // Set the the brush value.
    public void SelectValue(int index) {
        value = index;
    }

    // Sets the shape of the room.
    public void SelectShape(int index) {
        room.shape = (SHAPE)(index % (int)SHAPE.count);
        value = index;
        // room.shape = shape; // Until we set the buttons up.
        room.Construct();
    }

    /* --- Editing --- */
    // Print the edits to this map.
    void PrintEdit(int[][] grid, Tilemap map) {
        Geometry.PrintGridToMap(grid, map, challengeTiles);
    }

    // Edit the mob grid.
    void EditMobs() {
        if (Input.GetMouseButtonDown(0)) {
            room.mobGrid = Geometry.EditInteriorPoint(room.transform, room.mobGrid, room.borderGrid, value);
            PrintEdit(room.mobGrid, mobMap);
        }
        else if (Input.GetMouseButtonDown(1)) {
            room.mobGrid = Geometry.EditInteriorPoint(room.transform, room.mobGrid, room.borderGrid, (int)DIRECTION.Empty);
            PrintEdit(room.mobGrid, mobMap);
        }
    }

    // Edit the trap grid.
    void EditTraps() {
        if (Input.GetMouseButtonDown(0)) {
            room.trapGrid = Geometry.EditInteriorPoint(room.transform, room.trapGrid, room.borderGrid, value);
            PrintEdit(room.trapGrid, trapMap);
        }
        else if (Input.GetMouseButtonDown(1)) {
            room.trapGrid = Geometry.EditInteriorPoint(room.transform, room.trapGrid, room.borderGrid, (int)DIRECTION.Empty);
            PrintEdit(room.trapGrid, trapMap);
        }
    }

    // Get the respective challenge tiles.
    void GetChallengeTiles() {
        switch (room.challenge) {
            case (CHALLENGE.MOBS):
                challengeTiles = room.environment.ControllersToTileBase(room.environment.mobs);
                break;
            case (CHALLENGE.TRAPS):
                challengeTiles = room.environment.ControllersToTileBase(room.environment.traps);
                break;
            default:
                challengeTiles = new SpriteTile[0];
                break;
        }
    }

    // Create the selections for the new tiles.
    void CreateValueSelectors() {
        // Reset the value selections.
        if (valueSelections != null) {
            for (int i = valueSelections.Count - 1; i >= 0; i--) {
                Destroy(valueSelections[i].gameObject);
            }
        }
        valueSelections = new List<ValueSelector>();
        // Create the new selections.
        int j = 0;
        for (int i = 0; i < challengeTiles.Length; i++) {
            if (challengeTiles[i] != null) {
                ValueSelector newValueSelector = NewValue(challengeTiles[i], i, j);
                valueSelections.Add(newValueSelector);
                j++;
            }
        }
    }

    // Create a new brush selectable.
    ValueSelector NewValue(SpriteTile challengeTile, int i , int j) {
        ValueSelector valueSelector = Instantiate(nullValue.gameObject, 
            Vector3.zero, Quaternion.identity, challengeSelector.transform).GetComponent<ValueSelector>();
        // Set the selection parameters.
        valueSelector.gameObject.SetActive(true);
        valueSelector.transform.localPosition = nullValue.transform.localPosition + new Vector3(j % 3, -Mathf.Floor(j / 3), 0);
        valueSelector.spriteRenderer.sprite = challengeTile.newSprite;
        valueSelector.index = i;
        return valueSelector;
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

    // Create a new brush selectable.
    ValueSelector NewShape(Sprite sprite, int index) {
        ValueSelector valueSelector = Instantiate(nullShape.gameObject,
            Vector3.zero, Quaternion.identity, challengeSelector.transform).GetComponent<ValueSelector>();
        // Set the selection parameters.
        valueSelector.gameObject.SetActive(true);
        valueSelector.transform.localPosition = nullShape.transform.localPosition + new Vector3(index % 3, -Mathf.Floor(index / 3), 0);
        valueSelector.GetComponent<SpriteRenderer>().sprite = sprite;
        valueSelector.index = index;
        return valueSelector;
    }

}
