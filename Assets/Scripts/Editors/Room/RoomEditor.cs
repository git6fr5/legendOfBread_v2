using System.Collections;
using System.Collections.Generic;
using UnityEngine;
using UnityEngine.Tilemaps;

using SHAPE = Geometry.SHAPE;
using DIRECTION = Compass.DIRECTION;
using EXIT = Compass.EXIT;

using CHALLENGE = Room.CHALLENGE;

public class RoomEditor : MonoBehaviour {

    /* --- Components --- */
    public Room room;
    public Tilemap mobMap;
    public Tilemap trapMap;
    public ChallengeSelector challengeSelector;
    public ValueSelector nullValue;

    /* --- Variables --- */
    // Has to be sprite tiles to be able to access the sprite later on.
    SpriteTile[] challengeTiles;
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
        GetChannelTiles();
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
    }

    /* --- IO Stream --- */
    // Opens a CSV file as a room.
    public void Open(string filename) {
        room.Open(filename);
        PrintEdit(room.mobGrid, mobMap);
    }

    // Saves the room to a CSV.
    public void Save(string filename) {
        int[] identifiers = new int[] { (int)room.shape, (int)room.exits, (int)room.challenge };
        IO.EditListFile(identifiers, Room.path, filename);
        List<int[][]> challenges = new List<int[][]>() {
            room.mobGrid,
            room.trapGrid
        };

        IO.SaveCSV(challenges, Room.path, filename);
    }

    /* --- Additional Construction --- */
    // Sets the shape of the room.
    public void SelectShape(SHAPE shape) {
        room.shape = (SHAPE)(((int)room.shape + 1) % (int)SHAPE.count);
        // room.shape = shape; // Until we set the buttons up.
        room.Construct();
    }

    // Sets the exits for the room.
    public void SelectExits(EXIT exits) {
        room.exits = (EXIT)(((int)room.exits + 1) % (int)EXIT.count);
        // room.exits = exits; // Until we set the buttons up.
        room.Construct();
    }

    // Select a new challenge.
    public void SelectChannel(CHALLENGE selectedChallenge) {
        room.challenge = (CHALLENGE)(((int)room.challenge + 1) % (int)CHALLENGE.count);
        // challenge = selectedChannel;
        GetChannelTiles();
        // Create the selections for the new tiles.
        CreateValueSelectors();
    }

    /* --- Editing --- */
    // Edit the mob grid.
    void EditMobs() {
        if (Input.GetMouseButtonDown(0)) {
            room.mobGrid = Geometry.EditInteriorPoint(room.transform, room.mobGrid, room.borderGrid, value);
            PrintEdit(room.mobGrid, mobMap);
        }
        else if (Input.GetMouseButtonDown(1)) {
            room.mobGrid = Geometry.EditInteriorPoint(room.transform, room.mobGrid, room.borderGrid, (int)DIRECTION.EMPTY);
            PrintEdit(room.mobGrid, mobMap);
        }
    }

    // Edit the trap grid.
    void EditTraps() {
        //
    }

    // Print the edits to this map.
    void PrintEdit(int[][] grid, Tilemap map) {
        Geometry.PrintGridToMap(grid, map, challengeTiles);
    }

    // Get the respective challenge tiles.
    void GetChannelTiles() {
        switch (room.challenge) {
            case (CHALLENGE.MOBS):
                challengeTiles = room.environment.ControllersToTileBase(room.environment.mobs);
                break;
            case (CHALLENGE.TRAPS):
                challengeTiles = room.environment.ControllersToTileBase(room.environment.traps);
                break;
            default:
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
        valueSelector.transform.localPosition = new Vector3(j % 3 + 11, -Mathf.Floor(j / 3) + 3, 0); // There's a better way to do this.
        valueSelector.spriteRenderer.sprite = challengeTile.newSprite;
        valueSelector.index = i;
        return valueSelector;
    }

    // Set the the brush value.
    public void SelectValue(int index) {
        value = index;
    }

}
