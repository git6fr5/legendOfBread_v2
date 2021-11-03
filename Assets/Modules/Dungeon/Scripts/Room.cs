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

    /* --- Components --- */
    [SerializeField] public Tilemap floorMap; // The map that will display the floor tiles.
    [SerializeField] public Tilemap borderMap; // The map that will display the borders.
    [SerializeField] public Exit doorBase;

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
    public Exit AddDoor(Map.Switch doorSwitch, Loader.LDtkTileData doorData, List<int> unlockedExits) {

        bool construct = CheckDoorType(doorSwitch, doorData);

        // Clear the tile space.
        Vector3Int tilePosition = new Vector3Int((int)doorData.offsetPosition.x, (int)doorData.offsetPosition.y, 0);
        if (construct) {
            borderMap.SetTile(tilePosition, null);
        }

        if (doorBase != null) {

            // Instantiate the new door.
            Exit newExit = Instantiate(doorBase, (Vector3)tilePosition, Quaternion.identity, transform).GetComponent<Exit>();
            newExit.id = GetDoorID(doorData);
            newExit.transform.localPosition = (Vector3)tilePosition + new Vector3(newExit.id.x, -newExit.id.y, 0f) * (Exit.offset - height) + new Vector3(0.5f, 0.5f, 0f);
            newExit.gameObject.SetActive(construct);

            // The exit data.
            newExit.exitData = doorData;
            newExit.index = doorData.index;

            // exit type = vectorID.x
            if ((Map.Door)doorData.vectorID.x == Map.Door.Key) {
                bool isUnlocked = false;
                for (int i = 0; i < unlockedExits.Count; i++) {
                    if (doorData.index == unlockedExits[i]) {
                        isUnlocked = true;
                        break;
                    }
                }
                newExit.isLocked = !isUnlocked;
            }

            newExit.@lock.transform.position += -Vector3.up * (Exit.offset - 7);
            newExit.transform.eulerAngles = Vector3.forward * 90f * doorData.rotation;
            // newExit.@lock.transform.eulerAngles = Vector3.zero;

            exits.Add(newExit);
        }

        return null;
    }

    private bool CheckDoorType(Map.Switch doorSwitch, Loader.LDtkTileData door) {

        if ((Map.Door)door.vectorID.x == Map.Door.Regular) {
            return true;
        }
        else if ((Map.Door)door.vectorID.x == Map.Door.On && doorSwitch == Map.Switch.On) {
            return true;
        }
        else if ((Map.Door)door.vectorID.x == Map.Door.Off && doorSwitch == Map.Switch.Off) {
            return true;
        }
        else if ((Map.Door)door.vectorID.x == Map.Door.Item) { // Actually puzzle doors.
            return true;
        }
        else if ((Map.Door)door.vectorID.x == Map.Door.Key) {
            return true;
        }
        return false;
    }

    private Vector2Int GetDoorID(Loader.LDtkTileData exitData) {
        Vector2 _id = (exitData.offsetPosition - new Vector2(3.5f, 3.5f));
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
