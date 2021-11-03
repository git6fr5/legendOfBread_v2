/* --- Libraries --- */
using System.Collections;
using System.Collections.Generic;
using UnityEngine;

/// <summary>
/// Handles all the logic for the minimap.
/// </summary>
public class Minimap : MonoBehaviour {

    /* --- Static Variables --- */
    private static float Scale = 3f;

    /* --- Dictionaries --- */
    public Dictionary<Map.Door, Sprite> door_sprite = null;

    /* --- Components --- */
    [Space(5), Header("Minimap Bases")]
    public Transform squareBase;
    public Transform playerBase;
    public Transform doorBase;

    /* --- Sprites --- */
    [Space(5), Header("Sprites")]
    public Sprite regularDoor;
    public Sprite onDoor;
    public Sprite offDoor;
    public Sprite keyDoor;
    public Sprite itemDoor;

    /* --- Properties --- */
    [SerializeField] [ReadOnly] private List<Transform> locations = new List<Transform>();
    [SerializeField] [ReadOnly] private List<Transform> doors = new List<Transform>();

    /* --- Methods --- */
    void RefreshDoorSprites() {
        // Reset the dictionary.
        door_sprite = new Dictionary<Map.Door, Sprite>();

        // Set the sprite to represent each type of door.
        door_sprite.Add(Map.Door.Regular, regularDoor);
        door_sprite.Add(Map.Door.On, onDoor);
        door_sprite.Add(Map.Door.Off, offDoor);
        door_sprite.Add(Map.Door.Key, keyDoor);
        door_sprite.Add(Map.Door.Item, itemDoor);

    }

    // Refreshes the minimap.
    public void Refresh(Vector2Int location, Map.MapData mapData, int size, int locationGridSize, int doorGridSize) {
        
        // Reset the minimap.
        Reset();

        // Cache the scaling factors.
        float gridScaleFactor = Scale / ((float)size - 1); // Used for the placing the door in the correct position on the grid.
        float offsetScaleFactor = (float)locationGridSize / (float)doorGridSize; // Used for the placing the door at the correct offset.

        // Add the locations.
        for (int i = 0; i < mapData.shapes.Count; i++) {
            // if (mapData.loc_room.ContainsKey(mapData.shapes[i].gridPosition)) {
            Transform newLocation = AddLocation(mapData.shapes[i], gridScaleFactor);
            locations.Add(newLocation);
            // }
        }

        // Add the pathways.
        for (int i = 0; i < mapData.doors.Count; i++) {
            Transform newDoor = AddDoor(mapData.doors[i], mapData.doorSwitch, gridScaleFactor, offsetScaleFactor);
            doors.Add(newDoor);
        }

        SetPlayerLocation(location, gridScaleFactor);
    }

    // Resets the minimap.
    public void Reset() {
        // Reset the grid squares.
        for (int i = 0; i < locations.Count; i++) {
            Destroy(locations[i].gameObject);
            locations[i] = null;
        }
        locations = new List<Transform>();
        // Reset the door squares.
        for (int i = 0; i < doors.Count; i++) {
            Destroy(doors[i].gameObject);
            doors[i] = null;
        }
        doors = new List<Transform>();
    }

    // Sets the player's location on the minimap.
    public void SetPlayerLocation(Vector2Int gridPosition, float gridScaleFactor) {
        // Set the location of the player on the minimap.
        playerBase.position = new Vector3(squareBase.position.x + gridPosition.x * gridScaleFactor, squareBase.position.y - gridPosition.y * gridScaleFactor, 0f);
        playerBase.gameObject.SetActive(true);
    }

    // Sets the location of the rooms on the minimap.
    public Transform AddLocation(Loader.LDtkTileData locationData, float gridScaleFactor) {
        // Get the position to put the square at.
        Vector2Int gridPosition = locationData.gridPosition;
        Vector3 scaledGridPosition = new Vector3(squareBase.position.x + gridPosition.x * gridScaleFactor, squareBase.position.y - gridPosition.y * gridScaleFactor, 0f);
        // Instantiate the square.
        Transform newLocation = Instantiate(squareBase, scaledGridPosition, Quaternion.identity, transform);
        newLocation.gameObject.SetActive(true);
        return newLocation;
    }

    // Adds the doorways to the minimap.
    public Transform AddDoor(Loader.LDtkTileData doorData, Map.Switch doorSwitch, float gridScaleFactor, float offsetScaleFactor) {

        // Get the position to put it on the grid at.
        Vector2Int gridPosition = doorData.gridPosition;
        Vector3 scaledGridPosition = new Vector3(squareBase.position.x + gridPosition.x * gridScaleFactor, squareBase.position.y - gridPosition.y * gridScaleFactor, 0f);

        // Get the offset on the grid.
        Vector2Int offsetPosition = doorData.offsetPosition;
        if (offsetPosition.x == 7) { offsetPosition += new Vector2Int(-8, 0); }
        Vector3 scaledOffsetPosition = new Vector3((offsetPosition.x - 3) * gridScaleFactor / offsetScaleFactor, (offsetPosition.y - 3) * gridScaleFactor / offsetScaleFactor, 0f);
        
        // Adjust the grid position by the offset position.
        scaledGridPosition -= scaledOffsetPosition;

        // Instantiate the doorway.
        Transform newDoor = Instantiate(doorBase, scaledGridPosition, Quaternion.identity, transform);
        newDoor.gameObject.SetActive(true);

        //Set the door sprite.
        if (door_sprite == null) { RefreshDoorSprites(); }
        Map.Door door = (Map.Door)doorData.vectorID.x;
        if (doorSwitch == Map.Switch.Off) {
            if (door == Map.Door.On) { door = Map.Door.Off; }
            else if (door == Map.Door.Off) { door = Map.Door.On; }
        }
        newDoor.GetComponent<SpriteRenderer>().sprite = door_sprite[door];

        return newDoor;

    }

}
