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
    public enum Lock {
        Key,
        Off,
        Item,
        None
    }

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

    bool playerHasEntered = false;
    List<Exit> puzzleLocks = new List<Exit>();

    void Update() {
        //bool roomIsCleared = true;
        //if (playerHasEntered) {
        //    for (int i = 0; i < entities.Count; i++) {
        //        if (entities[i] != null) {
        //            if (entities[i].GetComponent<Mob>() != null) {
        //                Mob mob = entities[i].GetComponent<Mob>();
        //                if (mob.state.vitality != State.Vitality.Dead) {
        //                    roomIsCleared = false;
        //                }
        //            }
        //        }
        //    }
        //}

        //if (!roomIsCleared) {
        //    for (int i = 0; i < originallyUnlockedDoors.Count; i++) {
        //        originallyUnlockedDoors[i].lockType = Lock.Item;
        //    }
        //}
        //else {
        //    for (int i = 0; i < originallyUnlockedDoors.Count; i++) {
        //        originallyUnlockedDoors[i].lockType = Lock.None;
        //    }
        //}

        bool isCleared = true;
        for (int i = 0; i < entities.Count; i++) {
            if (entities[i] != null) {
                if (entities[i].GetComponent<Sensor>() != null) {
                    Sensor sensor = entities[i].GetComponent<Sensor>();
                    if (!sensor.isActivated) {
                        isCleared = false;
                    }
                }
            }
        }

        print(isCleared);
        if (isCleared) {
            for (int i = 0; i < puzzleLocks.Count; i++) {
                if (puzzleLocks[i].lockType == Lock.Item) {
                    puzzleLocks[i].lockType = Lock.None;
                }
            }
        }
        else {
            for (int i = 0; i < puzzleLocks.Count; i++) {
                if (puzzleLocks[i].lockType == Lock.None) {
                    puzzleLocks[i].lockType = Lock.Item;
                }
            }
        }


    }

    void OnTriggerStay2D(Collider2D collider) {

    }

    void OnTriggerEnter2D(Collider2D collider) {
        Player player = collider.GetComponent<Hurtbox>()?.controller?.GetComponent<Player>();
        if (player != null) {
            print("player entered");
            playerHasEntered = true;
        }
    }

    void OnTriggerExit2D(Collider2D collider) {
        Player player = collider.GetComponent<Hurtbox>()?.controller?.GetComponent<Player>();
        if (player != null) {
            print("player exited");
        }
    }

    /* --- Methods --- */
    public Exit AddDoor(Map.Switch doorSwitch, Loader.LDtkTileData doorData, List<int> unlockedExits) {

        Lock lockType = CheckDoorType(doorSwitch, doorData, unlockedExits);

        // Clear the tile space.
        Vector3Int tilePosition = new Vector3Int((int)doorData.offsetPosition.x, (int)doorData.offsetPosition.y, 0);
        borderMap.SetTile(tilePosition, null);

        if (doorBase != null) {

            // Instantiate the new door.
            Exit newExit = Instantiate(doorBase, (Vector3)tilePosition, Quaternion.identity, transform).GetComponent<Exit>();
            newExit.id = GetDoorID(doorData);
            newExit.transform.localPosition = (Vector3)tilePosition + new Vector3(newExit.id.x, -newExit.id.y, 0f) * (Exit.offset - height) + new Vector3(0.5f, 0.5f, 0f);
            newExit.gameObject.SetActive(true);

            // The exit data.
            newExit.exitData = doorData;
            newExit.index = doorData.index;

            newExit.lockType = lockType;
            
            newExit.@lock.transform.position += -Vector3.up * (Exit.offset - 7);
            newExit.transform.eulerAngles = Vector3.forward * 90f * doorData.rotation;
            // newExit.@lock.transform.eulerAngles = Vector3.zero;

            if (lockType == Lock.Item) { puzzleLocks.Add(newExit); }
            exits.Add(newExit);
        }

        return null;
    }

    private Lock CheckDoorType(Map.Switch doorSwitch, Loader.LDtkTileData doorData, List<int> unlockedExits) {

        if ((Map.Door)doorData.vectorID.x == Map.Door.Regular) {
            return Lock.None;
        }
        else if ((Map.Door)doorData.vectorID.x == Map.Door.On && doorSwitch != Map.Switch.On) {
            return Lock.Off;
        }
        else if ((Map.Door)doorData.vectorID.x == Map.Door.Off && doorSwitch != Map.Switch.Off) {
            return Lock.Off;
        }
        else if ((Map.Door)doorData.vectorID.x == Map.Door.Item) { // Actually puzzle doors.
            return Lock.Item;
        }
        else if ((Map.Door)doorData.vectorID.x == Map.Door.Key) {
            for (int i = 0; i < unlockedExits.Count; i++) {
                if (doorData.index == unlockedExits[i]) {
                    return Lock.None;
                }
            }
            return Lock.Key;
        }
        return Lock.None;
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
