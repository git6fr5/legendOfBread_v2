using System.Collections;
using System.Collections.Generic;
using UnityEngine;
using UnityEngine.Tilemaps;

using SHAPE = Geometry.SHAPE;
using TILE = Janitor.TILE;

public class RoomEditor : MonoBehaviour {

    /* --- Enums --- */
    public enum CHANNEL { 
        MOBS,
        TRAPS,
        count
    }

    /* --- Components --- */
    public Room room;
    public Tilemap mobMap;
    public ChannelSelector channelSelector;
    public ValueSelector nullValue;

    /* --- Variables --- */
    // Has to be sprite tiles to be able to access the sprite later on.
    SpriteTile[] channelTiles;
    List<ValueSelector> valueSelections;
    int value;
    CHANNEL channel;

    /* --- Unity --- */
    // Runs once on compilation.
    void Awake() {
        transform.position = room.transform.position;
        channelTiles = new SpriteTile[0];
    }

    // Runs once every frame.
    void Update() {
        if (Input.GetMouseButtonDown(0)) {
            Vector2 mousePos = Camera.main.ScreenToWorldPoint(Input.mousePosition);
            int[] coord = Geometry.PointToGrid(mousePos, room.transform);
            if (Geometry.IsValid(coord, room.mobGrid) && room.borderGrid[coord[0]][coord[1]] == (int)TILE.EMPTY) {
                switch (channel) {
                    case (CHANNEL.MOBS):
                        room.mobGrid[coord[0]][coord[1]] = value;
                        break;
                    default:
                        break;
                }
            }
            PrintEdit();
        }
    }

    /* --- Additional Construction --- */
    public void SelectShape(SHAPE shape) {
        room.shape = shape;
        room.Construct();
    }

    public void SetExits(TILE exits) {
        room.exits = exits;
        room.Construct();
    }

    /* --- Editing --- */
    // Print the edits to this map.
    void PrintEdit() {
        // For now, print only to the mob grid.
        Geometry.PrintGridToMap(room.mobGrid, mobMap, channelTiles);
    }

    // Select a new layer.
    public void SelectChannel(CHANNEL selectedChannel) {
        channel = selectedChannel;
        switch (channel) {
            case (CHANNEL.MOBS):
                channelTiles = room.environment.ControllersToTileBase(room.environment.mobs);
                break;
            default:
                break;
        }
        // Create the selections for the new tiles.
        CreateValueSelectors();
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
        for (int i = 0; i < channelTiles.Length; i++) {
            if (channelTiles[i] != null) {
                ValueSelector newValueSelector = NewValue(channelTiles[i], i, j);
                valueSelections.Add(newValueSelector);
                j++;
            }
        }
    }

    // Create a new brush selectable.
    ValueSelector NewValue(SpriteTile channelTile, int i , int j) {
        ValueSelector valueSelector = Instantiate(nullValue.gameObject, 
            Vector3.zero, Quaternion.identity, channelSelector.transform).GetComponent<ValueSelector>();
        // Set the selection parameters.
        valueSelector.transform.localPosition = new Vector3(j % 3, -Mathf.Floor(j / 3) - 1, 0);
        valueSelector.spriteRenderer.sprite = channelTile.newSprite;
        valueSelector.index = i;
        return valueSelector;
    }

    // Set the the brush value.
    public void SelectValue(int index) {
        value = index;
    }

}
