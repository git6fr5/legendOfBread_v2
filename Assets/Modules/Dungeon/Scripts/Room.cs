/* --- Libraries --- */
using System.Collections;
using System.Collections.Generic;
using UnityEngine;
using UnityEngine.Tilemaps;

/// <summary>
/// Room.
/// </summary>
public class Room : MonoBehaviour {

    /* --- Static Variables --- */
    public static Vector2 offset; // Stores the transforms offset for grid-snapping.

    /* --- Enumerations --- */
    public enum Location {
        Right, Up, Left, Down
    }

    /* --- Dictionaries --- */
    // Location of the exit to its scaled vector position
    public Dictionary<Location, Vector2> loc_vec = new Dictionary<Location, Vector2>() {
        { Location.Down, new Vector2(0.5f, 0f) },
        // Note: technically this is the up arrow, but the coordinates on the y axis are backwards.
        { Location.Left, new Vector2(0f, 0.5f) },
        { Location.Right, new Vector2(1f, 0.5f) },
        { Location.Up, new Vector2(0.5f, 1f) }
    };

    // Location of the exit to the ID of the exit.
    public Dictionary<Location, Vector2Int> loc_id = new Dictionary<Location, Vector2Int>() {
        { Location.Down, new Vector2Int(0, 1) },
        // Note: technically this is the up arrow, but the coordinates on the y axis are backwards.
        { Location.Left, new Vector2Int(-1, 0) },
        { Location.Right, new Vector2Int(1, 0) },
        { Location.Up, new Vector2Int(0, -1) }
    };

    /* --- Components --- */
    [SerializeField] public Tilemap floorMap; // The map that will display the floor tiles.
    [SerializeField] public Tilemap borderMap; // The map that will display the borders.

    /* --- Properties --- */
    // Dimensions.
    [SerializeField] [ReadOnly] public int height;
    [SerializeField] [ReadOnly] public int width;
    // Settings.
    [SerializeField] [ReadOnly] public int id;
    [SerializeField] [ReadOnly] public bool isDifficult;
    [SerializeField] [ReadOnly] public List<Entity> entities; // The list of currently loaded entities.
    [SerializeField] [ReadOnly] public List<Exit> exits; // The list of currently loaded exits.

    void Start() {
        // Set up these variables.
        offset = new Vector2( (Mathf.Abs(transform.position.x + 0.5f)) % 1f, Mathf.Abs((transform.position.y - 0.5f)) % 1f );
    }

    /* --- Methods --- */
    public Exit AddDoor(Map map, Loader.LDtkTileData exitData, Exit exitBase) {

        bool construct = CheckDoorType(map, exitData);

        Vector3Int tilePosition = new Vector3Int((int)exitData.gridPosition.x, (int)exitData.gridPosition.y, 0);
        if (construct) {
            borderMap.SetTile(tilePosition, null);
        }


        print("Loading exit");
        if (exitBase != null) {
            Exit newExit = Instantiate(exitBase, (Vector3)tilePosition, Quaternion.identity, transform).GetComponent<Exit>();
            newExit.id = GetDoorID(exitData);
            newExit.transform.localPosition = (Vector3)tilePosition + new Vector3(newExit.id.x, -newExit.id.y, 0f) * (Exit.offset - height) + new Vector3(0.5f, 0.5f, 0f);
            newExit.gameObject.SetActive(construct);

            // The exit data.
            newExit.exitData = exitData;
            newExit.index = exitData.index;

            // exit type = vectorID.x
            if ((Map.Door)exitData.vectorID.x == Map.Door.Key) {
                bool isUnlocked = false;
                for (int i = 0; i < map.unlockedExits.Count; i++) {
                    if (exitData.index == map.unlockedExits[i]) {
                        isUnlocked = true;
                        break;
                    }
                }
                newExit.isLocked = !isUnlocked;
            }

            exits.Add(newExit);
        }
        return null;
    }

    private bool CheckDoorType(Map map, Loader.LDtkTileData exitData) {

        if (map == null) {
            return true;
        }

        print("CHECKING DOOR TYPE" + (Map.Door)exitData.vectorID.x + map.doorSwitch.ToString());

        if ((Map.Door)exitData.vectorID.x == Map.Door.Regular) {
            return true;
        }
        else if ((Map.Door)exitData.vectorID.x == Map.Door.On && map.doorSwitch == Map.Switch.On) {
            return true;
        }
        else if ((Map.Door)exitData.vectorID.x == Map.Door.Off && map.doorSwitch == Map.Switch.Off) {
            return true;
        }
        else if ((Map.Door)exitData.vectorID.x == Map.Door.Item) { // Actually puzzle doors.
            return true;
        }
        else if ((Map.Door)exitData.vectorID.x == Map.Door.Key) {
            return true;
        }
        return false;
    }

    private Vector2Int GetDoorID(Loader.LDtkTileData exitData) {
        Vector2 _id = (exitData.gridPosition - new Vector2(3.5f, 3.5f));
        Vector2Int id;
        if (Mathf.Abs(_id.y) > Mathf.Abs(_id.x)) {
            id = new Vector2Int(0, -(int)Mathf.Sign(_id.y));
        }
        else {
            id = new Vector2Int((int)Mathf.Sign(_id.x), 0);
        }
        return id;
    }

    /* --- Grid Methods --- */
    public Vector3 GridToWorld(Vector2Int gridPosition) {
        return new Vector3(gridPosition.x + 0.5f, height - gridPosition.y - 0.5f, 0f) + transform.position;
    }

    public static Vector3 SnapToGrid(Vector3 position) {
        float snappedX = Mathf.Round(position.x) + offset.x;
        float snappedY = Mathf.Round(position.y) + offset.y;
        return new Vector3(snappedX, snappedY, 0f);
    }

}
